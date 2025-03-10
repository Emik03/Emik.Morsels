// SPDX-License-Identifier: MPL-2.0
#if !NETSTANDARD || NETSTANDARD2_0_OR_GREATER
#if NETFRAMEWORK || NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER && !NET5_0_OR_GREATER
#pragma warning disable GlobalUsingsAnalyzer
using SecurityAction = System.Security.Permissions.SecurityAction;

#pragma warning restore GlobalUsingsAnalyzer
#endif

// ReSharper disable once CheckNamespace
namespace Emik.Morsels;
#if NETFRAMEWORK || NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER && !NET5_0_OR_GREATER
using static SecurityAction;
using static SecurityPermissionFlag;
#endif

/// <summary>Provides methods for exiting the program.</summary>
static partial class Exit
{
    /// <remarks><para>This method represents the exit code 0, indicating success.</para></remarks>
    /// <inheritdoc cref="With{T}"/>
    [ContractAnnotation("=> halt"),
     DoesNotReturn,
     SecuritySafeCritical,
#if NETFRAMEWORK || NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER && !NET5_0_OR_GREATER
     SecurityPermission(Demand, Flags = UnmanagedCode),
#endif
    ]
    public static Exception Success(string? message = null) => With<Exception>(message, 0);

    /// <remarks><para>This method represents the exit code 1, indicating failure.</para></remarks>
    /// <inheritdoc cref="With{T}"/>
    [ContractAnnotation("=> halt"),
     DoesNotReturn,
     SecuritySafeCritical,
#if NETFRAMEWORK || NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER && !NET5_0_OR_GREATER
     SecurityPermission(Demand, Flags = UnmanagedCode),
#endif
    ]
    public static Exception Failure(string? message = null) => With<Exception>(message, 1);

    /// <remarks><para>This method represents the exit code 2, indicating invalid parameters.</para></remarks>
    /// <inheritdoc cref="With{T}"/>
    [ContractAnnotation("=> halt"),
     DoesNotReturn,
     SecuritySafeCritical,
#if NETFRAMEWORK || NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER && !NET5_0_OR_GREATER
     SecurityPermission(Demand, Flags = UnmanagedCode),
#endif
    ]
    public static Exception Usage(string? message = null) => With<Exception>(message, 2);

    /// <typeparam name="T">Only used for type coercion.</typeparam>
    /// <inheritdoc cref="Success"/>
    [ContractAnnotation("=> halt"),
     DoesNotReturn,
     SecuritySafeCritical,
#if NETFRAMEWORK || NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER && !NET5_0_OR_GREATER
     SecurityPermission(Demand, Flags = UnmanagedCode),
#endif
    ]
    public static T Success<T>(string? message = null) => With<T>(message, 0);

    /// <typeparam name="T">Only used for type coercion.</typeparam>
    /// <inheritdoc cref="Failure"/>
    [ContractAnnotation("=> halt"),
     DoesNotReturn,
     SecuritySafeCritical,
#if NETFRAMEWORK || NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER && !NET5_0_OR_GREATER
     SecurityPermission(Demand, Flags = UnmanagedCode),
#endif
    ]
    public static T Failure<T>(string? message = null) => With<T>(message, 1);

    /// <typeparam name="T">Only used for type coercion.</typeparam>
    /// <inheritdoc cref="Usage"/>
    [ContractAnnotation("=> halt"),
     DoesNotReturn,
     SecuritySafeCritical,
#if NETFRAMEWORK || NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER && !NET5_0_OR_GREATER
     SecurityPermission(Demand, Flags = UnmanagedCode),
#endif
    ]
    public static T Usage<T>(string? message = null) => With<T>(message, 2);

    /// <summary>Terminates this process and returns the exit code to the operating system.</summary>
    /// <typeparam name="T">Only used for type coercion.</typeparam>
    /// <param name="message">The message to print into the standard output/error, if specified.</param>
    /// <param name="exitCode">The exit code.</param>
    /// <exception cref="SecurityException">
    /// The caller does not have sufficient security permission to perform this function.
    /// </exception>
    /// <returns>This method does not return. Specified to allow <see langword="throw"/> expressions.</returns>
    [ContractAnnotation("=> halt"), DoesNotReturn, SecuritySafeCritical,
#if NETFRAMEWORK || NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER && !NET5_0_OR_GREATER
     SecurityPermission(Demand, Flags = UnmanagedCode),
#endif
    ]
    static T With<T>(string? message, byte exitCode)
    {
        if (message is not null)
            (exitCode is 0 ? Console.Out : Console.Error).WriteLine(message);

        Environment.Exit(exitCode);
        throw Unreachable;
    }
}
#endif
