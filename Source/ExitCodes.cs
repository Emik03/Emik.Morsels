// SPDX-License-Identifier: MPL-2.0
#pragma warning disable CS8632, MA0048, SA1629, GlobalUsingsAnalyzer
using SecurityAction = System.Security.Permissions.SecurityAction;

namespace Emik.Morsels;

using static SecurityAction;
using static SecurityPermissionFlag;
using static UnitGenerateOptions;

/// <summary>Provides constants for exit codes.</summary>
[UnitOf(typeof(byte), ImplicitOperator | ParseMethod)]
readonly partial struct ExitCodes
{
    /// <summary>Gets the exit code determining that the application exited successfully.</summary>
    public static ExitCodes Success { get; } = new(0);

    /// <summary>Gets the exit code determining that the application exited due to an error.</summary>
    public static ExitCodes Failure { get; } = new(1);

    /// <summary>Gets the exit code determining that the wrong arguments were given.</summary>
    public static ExitCodes Usage { get; } = new(2);

    /// <summary>Gets a value indicating whether this instance represents a success exit.</summary>
    public bool IsSuccess => value is 0;

    /// <summary>Gets the writer corresponding to this exit code.</summary>
    public TextWriter Writer => IsSuccess ? Console.Out : Console.Error;

    /// <summary>Terminates this process and returns an exit code to the operating system.</summary>
    /// <param name="message">The message to print into the standard output/error, if specified.</param>
    /// <exception cref="SecurityException">
    /// The caller does not have sufficient security permission to perform this function.
    /// </exception>
    [ContractAnnotation("=> halt"),
     DoesNotReturn,
     SecuritySafeCritical,
#if NETFRAMEWORK || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
     SecurityPermission(Demand, Flags = UnmanagedCode),
    ]
#endif
    public void Exit(string? message = null)
    {
        if (message is not null)
            Writer.WriteLine(message);

        Environment.Exit(value);
    }

    /// <summary>Terminates this process and returns an exit code to the operating system.</summary>
    /// <typeparam name="T">Only used for type coercion.</typeparam>
    /// <param name="message">The message to print into the standard output/error, if specified.</param>
    /// <exception cref="SecurityException">
    /// The caller does not have sufficient security permission to perform this function.
    /// </exception>
    /// <returns>This method does not return.</returns>
    [ContractAnnotation("=> halt"),
     DoesNotReturn,
     SecuritySafeCritical,
#if NETFRAMEWORK || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
     SecurityPermission(Demand, Flags = UnmanagedCode),
    ]
#endif
    public T Exit<T>(string? message = null)
    {
        Exit(message);
        throw Unreachable;
    }
}
