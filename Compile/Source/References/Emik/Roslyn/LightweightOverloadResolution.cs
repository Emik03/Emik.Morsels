// SPDX-License-Identifier: MPL-2.0
#if ROSLYN
// ReSharper disable CheckNamespace RedundantNameQualifier UseSymbolAlias
namespace Emik.Morsels;

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/// <summary>
/// Helper type that allows signature help to make better decisions about which overload the user is likely choosing
/// when the compiler itself bails out and gives a generic list of options.
/// </summary>
/// <remarks><para>
/// This implementation is based on a modified version of
/// <a href="https://github.com/dotnet/roslyn/blob/main/src/Features/CSharp/Portable/SignatureHelp/LightweightOverloadResolution.cs">
/// <c>Microsoft.CodeAnalysis.CSharp.SignatureHelp.LightweightOverloadResolution</c>
/// </a>.
/// </para></remarks>
readonly struct LightweightOverloadResolution(
    SemanticModel semanticModel,
    int position,
    SeparatedSyntaxList<ArgumentSyntax> arguments
)
{
    /// <summary>Contains the resulting overload.</summary>
    /// <param name="Method">The <see cref="IMethodSymbol"/> to use.</param>
    /// <param name="ParameterIndex">The index of the parameter to highlight.</param>
    public record struct Overload(IMethodSymbol? Method, int ParameterIndex)
    {
        /// <summary>Gets the instance indicating that no overload was found.</summary>
        [Pure]
        public static Overload None => new(null, -1);
    }

    /// <summary>Performs the overload resolution.</summary>
    /// <param name="symbolInfo">The <see cref="SymbolInfo"/> to use.</param>
    /// <param name="candidates">The different overloads.</param>
    /// <returns>The overload to use.</returns>
    // If the compiler told us the correct overload or we only have one choice,
    // but we need to find out the parameter to highlight given cursor position.
    public Overload RefineOverloadAndPickParameter(SymbolInfo symbolInfo, ImmutableArray<IMethodSymbol> candidates) =>
        symbolInfo.Symbol is IMethodSymbol method
            ? TryFindParameterIndexIfCompatibleMethod(method)
            : GuessCurrentSymbolAndParameter(candidates);

    /// <summary>Finds the parameter index if the method is compatible.</summary>
    /// <param name="method">The <see cref="IMethodSymbol"/> to check.</param>
    /// <returns>The index of the parameter if the method is compatible, <c>-1</c> otherwise.</returns>
    public int FindParameterIndexIfCompatibleMethod(IMethodSymbol method) =>
        TryFindParameterIndexIfCompatibleMethod(method) is (not null, var parameterIndex) ? parameterIndex : -1;

    /// <summary>Determines whether the expression is empty.</summary>
    /// <param name="expression">The expression to check.</param>
    /// <returns>Whether the parameter <paramref name="expression"/> is empty.</returns>
    static bool IsEmptyArgument(SyntaxNode expression) => expression.Span.IsEmpty;

    /// <summary>Deals with a partial invocation and finds the respective overload.</summary>
    /// <param name="methodGroup">The different overloads.</param>
    /// <returns>The overload to use.</returns>
    Overload GuessCurrentSymbolAndParameter(ImmutableArray<IMethodSymbol> methodGroup)
    {
        if (arguments is [])
            return Overload.None;

        foreach (var method in methodGroup)
        {
            var (candidateMethod, parameterIndex) = TryFindParameterIndexIfCompatibleMethod(method);

            if (candidateMethod is not null)
                return new(candidateMethod, parameterIndex);
        }

        // Note: Providing no recommendation if no arguments allows the model to keep the last implicit choice.
        return Overload.None;
    }

    /// <summary>
    /// Simulates overload resolution with the arguments provided so far
    /// and determines if you might be calling this overload.
    /// </summary>
    /// <returns>
    /// Returns true if an overload is acceptable. In that case, we output the parameter that
    /// should be highlighted given the cursor's position in the partial invocation.
    /// </returns>
    Overload TryFindParameterIndexIfCompatibleMethod(IMethodSymbol method)
    {
        // Map the arguments to their corresponding parameters.
        var argumentToParameterMap =
            arguments.Count <= 256 ? stackalloc int[arguments.Count] : new int[arguments.Count];

        argumentToParameterMap.Fill(-1);

        if (!TryPrepareArgumentToParameterMap(method, argumentToParameterMap))
            return Overload.None;

        // verify that the arguments are compatible with their corresponding parameters
        var parameters = method.Parameters;

        for (var argumentIndex = 0; argumentIndex < arguments.Count; argumentIndex++)
        {
            var parameterIndex = argumentToParameterMap[argumentIndex];

            if (parameterIndex < 0)
                continue;

            var parameter = parameters[parameterIndex];
            var argument = arguments[argumentIndex];

            if (IsCompatibleArgument(argument, parameter))
                continue;

            // We found a corresponding argument for this parameter. If it's not compatible (say, a string passed
            // to an int parameter), then this is not a suitable overload.
            return Overload.None;
        }

        // find the parameter at the cursor position
        var argumentIndexToSave = GetArgumentIndex();
        var foundParameterIndex = -1;

        if (argumentIndexToSave >= 0)
        {
            foundParameterIndex = argumentToParameterMap[argumentIndexToSave];

            if (foundParameterIndex < 0)
                foundParameterIndex = FirstUnspecifiedParameter(argumentToParameterMap);
        }

        System.Diagnostics.Debug.Assert(
            foundParameterIndex < parameters.Length,
            "foundParameterIndex < parameters.Length"
        );

        return new(method, foundParameterIndex);
    }

    /// <summary>Determines if the given argument is compatible with the given parameter.</summary>
    /// <param name="argument">The argument to check.</param>
    /// <param name="parameter">The parameter to check.</param>
    /// <returns>
    /// <see langword="true"/> if the argument is compatible with the parameter, <see langword="false"/> otherwise.
    /// </returns>
    bool IsCompatibleArgument(ArgumentSyntax argument, IParameterSymbol parameter)
    {
        var parameterRefKind = parameter.RefKind;

        if (parameterRefKind is RefKind.None)
        {
            // An argument left empty is considered to match any parameter
            // M(1, $$)
            // M(1, , 2$$)
            if (IsEmptyArgument(argument.Expression))
                return true;

            var type = parameter.Type;

            if (parameter.IsParams &&
                type is IArrayTypeSymbol arrayType &&
                semanticModel.ClassifyConversion(argument.Expression, arrayType.ElementType).IsImplicit)
                return true;

            return semanticModel.ClassifyConversion(argument.Expression, type).IsImplicit;
        }

        var argumentRefKind = argument.GetRefKind();

        if (parameterRefKind == argumentRefKind)
            return true;

        // A by-value argument matches an `in` parameter
        return parameterRefKind is RefKind.In && argumentRefKind is RefKind.None;
    }

    /// <summary>Highlights the first unspecified parameter.</summary>
    /// <param name="argumentToParameterMap">The input map.</param>
    /// <returns>The index of the first unspecified parameter.</returns>
    int FirstUnspecifiedParameter(Span<int> argumentToParameterMap)
    {
        var specified = arguments.Count <= 1024 ? stackalloc bool[arguments.Count] : new bool[arguments.Count];
        specified.Clear();

        for (var i = 0; i < arguments.Count; i++)
        {
            var parameterIndex = argumentToParameterMap[i];

            if (parameterIndex >= 0 && parameterIndex < arguments.Count)
                specified[parameterIndex] = true;
        }

        for (var i = 0; i < specified.Length; i++)
            if (!specified[i])
                return i;

        return 0;
    }

    /// <summary>Find the parameter index corresponding to each argument provided.</summary>
    /// <param name="method">The method to prepare.</param>
    /// <param name="argumentToParameterMap">The output map.</param>
    /// <returns>Whether or not the method could be prepared.</returns>
#pragma warning disable MA0051 // ReSharper disable once CognitiveComplexity
    bool TryPrepareArgumentToParameterMap(IMethodSymbol method, Span<int> argumentToParameterMap)
#pragma warning restore MA0051
    {
        System.Diagnostics.Debug.Assert(
            argumentToParameterMap.Length == arguments.Count,
            "argumentToParameterMap.Length == arguments.Count"
        );

        var currentParameterIndex = 0;
        var seenOutOfPositionArgument = false;
        var inParams = false;

        for (var argumentIndex = 0; argumentIndex < arguments.Count; argumentIndex++)
        {
            // Went past the number of parameters this method takes, and this is a non-params method.  There's no
            // way this could ever match.
            if (argumentIndex >= method.Parameters.Length && !inParams)
                return false;

            var argument = arguments[argumentIndex];

            if (argument is { NameColon.Name.Identifier.ValueText: var argumentName })
            {
                // If this was a named argument but the method has no parameter with that name, there's definitely
                // no match. Note: this is C# only, so we don't need to worry about case matching.
                var namedParameterIndex = 0;

                for (;
                    namedParameterIndex < method.Parameters.Length &&
                    method.Parameters[namedParameterIndex].Name == argumentName;
                    namedParameterIndex++) { }

                if (namedParameterIndex == method.Parameters.Length)
                    return false;

                if (namedParameterIndex != currentParameterIndex)
                    seenOutOfPositionArgument = true;

                AddArgumentToParameterMapping(argumentIndex, namedParameterIndex, argumentToParameterMap);
            }
            else if (IsEmptyArgument(argument.Expression))
            {
                // We count the empty argument as a used position
                if (!seenOutOfPositionArgument)
                    AddArgumentToParameterMapping(argumentIndex, currentParameterIndex, argumentToParameterMap);
            }
            else if (seenOutOfPositionArgument) // Unnamed arguments are not allowed after an out-of-position argument
                return false;
            else // Normal argument.
                AddArgumentToParameterMapping(argumentIndex, currentParameterIndex, argumentToParameterMap);
        }

        return true;

        void AddArgumentToParameterMapping(
            int argumentIndex,
            int parameterIndex,
            Span<int> argumentToParameterMap
        )
        {
            System.Diagnostics.Debug.Assert(parameterIndex >= 0, "parameterIndex >= 0");

            System.Diagnostics.Debug.Assert(
                parameterIndex < method.Parameters.Length,
                "parameterIndex < method.Parameters.Length"
            );

            inParams |= method.Parameters[parameterIndex].IsParams;
            argumentToParameterMap[argumentIndex] = parameterIndex;

            // Increment our current parameter index if we're still processing parameters in sequential order.
            if (!seenOutOfPositionArgument && !inParams)
                currentParameterIndex++;
        }
    }

    /// <summary>
    /// Given the cursor position, find which argument is active.
    /// This will be useful to later find which parameter should be highlighted.
    /// </summary>
    int GetArgumentIndex()
    {
        // `$$,` points to the argument before the separator
        // but `,$$` points to the argument following the separator
        for (var i = 0; i < arguments.Count - 1; i++)
            if (position <= arguments.GetSeparator(i).Span.Start)
                return i;

        return arguments.Count - 1;
    }
}
#endif
