// SPDX-License-Identifier: MPL-2.0

// ReSharper disable ArrangeStaticMemberQualifier CheckNamespace ClassNeverInstantiated.Global EmptyNamespace RedundantUsingDirective
#pragma warning disable CA1019, GlobalUsingsAnalyzer, MA0047, MA0048, SA1114, SA1216, SA1402, SA1403, SA1649

#if !NET35
global using NotNullAttribute = System.Diagnostics.CodeAnalysis.NotNullAttribute;
#endif
using static System.AttributeTargets;

namespace System.Diagnostics.CodeAnalysis
{
#if NETFRAMEWORK || NETSTANDARD && !NETSTANDARD2_1
    /// <summary>Specifies that null is allowed as an input even if the corresponding type disallows it.</summary>
    [AttributeUsage(Field | Parameter | Property)]
    sealed class AllowNullAttribute : Attribute { }

    /// <summary>Specifies that null is disallowed as an input even if the corresponding type allows it.</summary>
    [AttributeUsage(Field | Parameter | Property)]
    sealed class DisallowNullAttribute : Attribute { }

    /// <summary>Specifies that an output may be null even if the corresponding type disallows it.</summary>
    [AttributeUsage(Field | Parameter | Property | ReturnValue)]
    sealed class MaybeNullAttribute : Attribute { }

    /// <summary>Specifies that an output will not be null even if the corresponding type allows it. Specifies that an input argument was not null when the call returns.</summary>
    [AttributeUsage(Field | Parameter | Property | ReturnValue)]
    sealed class NotNullAttribute : Attribute { }

    /// <summary>Specifies that when a method returns <see cref="ReturnValue"/>, the parameter may be null even if the corresponding type disallows it.</summary>
    [AttributeUsage(Parameter)]
    sealed class MaybeNullWhenAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaybeNullWhenAttribute"/> class
        /// with the specified return value condition.
        /// </summary>
        /// <param name="returnValue">
        /// The return value condition. If the method returns this value, the associated parameter may be null.
        /// </param>
        public MaybeNullWhenAttribute(bool returnValue) => ReturnValue = returnValue;

        /// <summary>
        /// Gets a value indicating whether the return value condition is <see langword="true"/> or <see langword="false"/>.
        /// </summary>
        public bool ReturnValue { get; }
    }

    /// <summary>
    /// Specifies that when a method returns <see cref="ReturnValue"/>,
    /// the parameter will not be null even if the corresponding type allows it.
    /// </summary>
    [AttributeUsage(Parameter)]
    sealed class NotNullWhenAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotNullWhenAttribute"/> class
        /// with the specified return value condition.
        /// </summary>
        /// <param name="returnValue">
        /// The return value condition. If the method returns this value, the associated parameter will not be null.
        /// </param>
        public NotNullWhenAttribute(bool returnValue) => ReturnValue = returnValue;

        /// <summary>
        /// Gets a value indicating whether the return value condition is <see langword="true"/> or <see langword="false"/>.
        /// </summary>
        public bool ReturnValue { get; }
    }

    /// <summary>Specifies that the output will be non-null if the named parameter is non-null.</summary>
    [AttributeUsage(Parameter | Property | ReturnValue, AllowMultiple = true)]
    sealed class NotNullIfNotNullAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotNullIfNotNullAttribute"/> class
        /// with the associated parameter name.
        /// </summary>
        /// <param name="parameterName">
        /// The associated parameter name.  The output will be non-null if the argument to the parameter specified is non-null.
        /// </param>
        public NotNullIfNotNullAttribute(string parameterName) => ParameterName = parameterName;

        /// <summary>Gets the associated parameter name.</summary>
        public string ParameterName { get; }
    }

    /// <summary>Applied to a method that will never return under any circumstance.</summary>
    [AttributeUsage(Method, Inherited = false)]
    sealed class DoesNotReturnAttribute : Attribute { }

    /// <summary>Specifies that the method will not return if the associated Boolean parameter is passed the specified value.</summary>
    [AttributeUsage(Parameter)]
    sealed class DoesNotReturnIfAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoesNotReturnIfAttribute"/> class
        /// with the specified parameter value.
        /// </summary>
        /// <param name="parameterValue">
        /// The condition parameter value. Code after the method will be considered unreachable by diagnostics if the argument to
        /// the associated parameter matches this value.
        /// </param>
        public DoesNotReturnIfAttribute(bool parameterValue) => ParameterValue = parameterValue;

        /// <summary>
        /// Gets a value indicating whether the condition parameter value
        /// is <see langword="true"/> or <see langword="false"/>.
        /// </summary>
        public bool ParameterValue { get; }
    }
#endif
#if NETFRAMEWORK || NETSTANDARD
    /// <summary>Specifies that the method or property will ensure that the listed field and property members have not-null values.</summary>
    [AttributeUsage(Method | Property, Inherited = false, AllowMultiple = true)]
    sealed class MemberNotNullAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemberNotNullAttribute"/> class with a field or property member.
        /// </summary>
        /// <param name="member">
        /// The field or property member that is promised to be not-null.
        /// </param>
        public MemberNotNullAttribute(string member) => Members = new[] { member };

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberNotNullAttribute"/> class
        /// with the list of field and property members.
        /// </summary>
        /// <param name="members">
        /// The list of field and property members that are promised to be not-null.
        /// </param>
        public MemberNotNullAttribute(params string[] members) => Members = members;

        /// <summary>Gets field or property member names.</summary>
        public string[] Members { get; }
    }

    /// <summary>Specifies that the method or property will ensure that the listed field and property members have not-null values when returning with the specified return value condition.</summary>
    [AttributeUsage(Method | Property, Inherited = false, AllowMultiple = true)]
    sealed class MemberNotNullWhenAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemberNotNullWhenAttribute"/> class
        /// with the specified return value condition and a field or property member.
        /// </summary>
        /// <param name="returnValue">
        /// The return value condition. If the method returns this value, the associated parameter will not be null.
        /// </param>
        /// <param name="member">
        /// The field or property member that is promised to be not-null.
        /// </param>
        public MemberNotNullWhenAttribute(bool returnValue, string member)
        {
            ReturnValue = returnValue;
            Members = new[] { member };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberNotNullWhenAttribute"/> class
        /// with the specified return value condition and list of field and property members.
        /// </summary>
        /// <param name="returnValue">
        /// The return value condition. If the method returns this value, the associated parameter will not be null.
        /// </param>
        /// <param name="members">
        /// The list of field and property members that are promised to be not-null.
        /// </param>
        public MemberNotNullWhenAttribute(bool returnValue, params string[] members)
        {
            ReturnValue = returnValue;
            Members = members;
        }

        /// <summary>
        /// Gets a value indicating whether the return value condition is <see langword="true"/> or <see langword="false"/>.
        /// </summary>
        public bool ReturnValue { get; }

        /// <summary>Gets field or property member names.</summary>
        public string[] Members { get; }
    }
#endif
#if !NET7_0_OR_GREATER
    /// <summary>
    /// Specifies that this constructor sets all required members for the current type,
    /// and callers do not need to set any required members themselves.
    /// </summary>
    [AttributeUsage(Constructor)]
    sealed class SetsRequiredMembersAttribute : Attribute { }
#endif
}

namespace System.Runtime.CompilerServices
{
#if !NET5_0_OR_GREATER
    /// <summary>
    /// Reserved to be used by the compiler for tracking metadata.
    /// This class should not be used by developers in source code.
    /// </summary>
    [
#if !(NETFRAMEWORK && !NET40_OR_GREATER || NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
        ExcludeFromCodeCoverage,
#endif
        DebuggerNonUserCode]
    static class IsExternalInit { }
#endif
#if NET20 || NET30
    /// <summary>
    /// Indicates that a method is an extension method, or that a class or assembly contains extension methods.
    /// </summary>
    [AttributeUsage(Method | Class | AttributeTargets.Assembly)]
    sealed class ExtensionAttribute : Attribute { }
#endif
#if !NET5_0_OR_GREATER
    /// <summary>
    /// Used to indicate to the compiler that a method should be called in its containing module's initializer.
    /// </summary>
    /// <remarks><para>
    /// When one or more valid methods with this attribute are found in a compilation,
    /// the compiler will emit a module initializer that calls each of the attributed methods.<br />
    /// Certain requirements are imposed on any method targeted with this attribute:
    /// </para><list type="bullet">
    /// <item><description>The method must be <see langword="static"/>.</description></item>
    /// <item><description>
    /// The method must be an ordinary member method, as opposed to a property accessor,
    /// constructor, local function, and so on.
    /// </description></item>
    /// <item><description>The method must be parameterless..</description></item>
    /// <item><description>The method must return <see langword="void"/>.</description></item>
    /// <item><description>The method must not be generic or be contained in a generic type.</description></item>
    /// <item><description>
    /// The method's effective accessibility must be <see langword="internal"/> or <see langword="public"/>.
    /// </description></item>
    /// </list><para>
    /// For more information, see
    /// https://github.com/dotnet/runtime/blob/main/docs/design/specs/Ecma-335-Augments.md#module-initializer.
    /// </para></remarks>
    [AttributeUsage(Method, Inherited = false)]
    sealed class ModuleInitializerAttribute : Attribute { }
#endif
#if NETFRAMEWORK
    /// <inheritdoc />
    [AttributeUsage(Parameter)]
    sealed class CallerFilePathAttribute : Attribute { }

    /// <inheritdoc />
    [AttributeUsage(Parameter)]
    sealed class CallerLineNumberAttribute : Attribute { }

    /// <inheritdoc />
    [AttributeUsage(Parameter)]
    sealed class CallerMemberNameAttribute : Attribute { }
#endif
#if NETFRAMEWORK || NETSTANDARD
    /// <summary>Indicates that a parameter captures the expression passed for another parameter as a string.</summary>
    /// <remarks><para>This attribute is implemented in the compiler for C# 10 and later versions only.</para></remarks>
    [AttributeUsage(Parameter)]
    sealed class CallerArgumentExpressionAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="CallerArgumentExpressionAttribute"/> class.</summary>
        /// <param name="parameterName">The name of the parameter whose expression should be captured as a string.</param>
        public CallerArgumentExpressionAttribute([InvokerParameterName] string parameterName) =>
            ParameterName = parameterName;

        /// <summary>Gets the name of the parameter whose expression should be captured as a string.</summary>
        [Pure]
        public string ParameterName { get; }
    }
#endif
#if !NET5_0_OR_GREATER
    /// <summary>
    /// Used to indicate to the compiler that the <c>.locals init</c> flag should not be set in method headers.
    /// </summary>
    /// <remarks><para>
    /// This attribute is unsafe because it may reveal uninitialized memory to the application in certain
    /// instances (e.g., reading from uninitialized stackalloc'd memory). If applied to a method directly,
    /// the attribute applies to that method and all nested functions (lambdas, local functions) below it.
    /// If applied to a type or module, it applies to all methods nested inside. This attribute is intentionally
    /// not permitted on assemblies. Use at the module level instead to apply to multiple type declarations.
    /// </para></remarks>
    [AttributeUsage(
        AttributeTargets.Module | Class | Struct | Interface | Constructor | Method | Property | AttributeTargets.Event,
        Inherited = false
    )]
    sealed class SkipLocalsInitAttribute : Attribute { }
#endif
#if !NET7_0_OR_GREATER
    /// <summary>
    /// Indicates that compiler support for a particular feature is
    /// required for the location where this attribute is applied.
    /// </summary>
    [AttributeUsage(All, AllowMultiple = true, Inherited = false)]
    sealed class CompilerFeatureRequiredAttribute : Attribute
    {
        /// <summary>The <see cref="FeatureName"/> used for the ref structs C# feature.</summary>
        public const string RefStructs = nameof(RefStructs);

        /// <summary>The <see cref="FeatureName"/> used for the required members C# feature.</summary>
        public const string RequiredMembers = nameof(RequiredMembers);

        /// <summary>Initializes a new instance of the <see cref="CompilerFeatureRequiredAttribute"/> class.</summary>
        /// <param name="featureName">The name of the compiler feature.</param>
        public CompilerFeatureRequiredAttribute(string featureName) => FeatureName = featureName;

        /// <summary>Gets the name of the compiler feature.</summary>
        [Pure]
        public string FeatureName { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the compiler can choose to allow access to the location
        /// where this attribute is applied if it does not understand <see cref="FeatureName"/>.
        /// </summary>
        [Pure]
        public bool IsOptional { get; set; }
    }

    /// <summary>Specifies that a type has required members or that a member is required.</summary>
    [AttributeUsage(Constructor)]
    sealed class RequiredMemberAttribute : Attribute { }
#endif
}
