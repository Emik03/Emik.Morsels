// SPDX-License-Identifier: MPL-2.0
#pragma warning disable CS8632, MA0048, SA1629, SYSLIB0003, GlobalUsingsAnalyzer
#if NETFRAMEWORK || NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER && !NET5_0_OR_GREATER
using SecurityAction = System.Security.Permissions.SecurityAction;
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
    /// <inheritdoc cref="With"/>
    [ContractAnnotation("=> halt"),
     DoesNotReturn,
     SecuritySafeCritical,
#if NETFRAMEWORK || NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER && !NET5_0_OR_GREATER
     SecurityPermission(Demand, Flags = UnmanagedCode),
#endif
    ]
    public static Exception Success(string? message = null) => throw With(0, message);

    /// <remarks><para>This method represents the exit code 1, indicating failure.</para></remarks>
    /// <inheritdoc cref="With"/>
    [ContractAnnotation("=> halt"),
     DoesNotReturn,
     SecuritySafeCritical,
#if NETFRAMEWORK || NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER && !NET5_0_OR_GREATER
     SecurityPermission(Demand, Flags = UnmanagedCode),
#endif
    ]
    public static Exception Failure(string? message = null) => throw With(1, message);

    /// <remarks><para>This method represents the exit code 2, indicating invalid parameters.</para></remarks>
    /// <inheritdoc cref="With"/>
    [ContractAnnotation("=> halt"),
     DoesNotReturn,
     SecuritySafeCritical,
#if NETFRAMEWORK || NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER && !NET5_0_OR_GREATER
     SecurityPermission(Demand, Flags = UnmanagedCode),
#endif
    ]
    public static Exception Usage(string? message = null) => throw With(2, message);

    /// <typeparam name="T">Only used for type coercion.</typeparam>
    /// <inheritdoc cref="Success"/>
    [ContractAnnotation("=> halt"),
     DoesNotReturn,
     SecuritySafeCritical,
#if NETFRAMEWORK || NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER && !NET5_0_OR_GREATER
     SecurityPermission(Demand, Flags = UnmanagedCode),
#endif
    ]
    public static T Success<T>(string? message = null) => throw With(0, message);

    /// <typeparam name="T">Only used for type coercion.</typeparam>
    /// <inheritdoc cref="Failure"/>
    [ContractAnnotation("=> halt"),
     DoesNotReturn,
     SecuritySafeCritical,
#if NETFRAMEWORK || NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER && !NET5_0_OR_GREATER
     SecurityPermission(Demand, Flags = UnmanagedCode),
#endif
    ]
    public static T Failure<T>(string? message = null) => throw With(1, message);

    /// <typeparam name="T">Only used for type coercion.</typeparam>
    /// <inheritdoc cref="Usage"/>
    [ContractAnnotation("=> halt"),
     DoesNotReturn,
     SecuritySafeCritical,
#if NETFRAMEWORK || NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER && !NET5_0_OR_GREATER
     SecurityPermission(Demand, Flags = UnmanagedCode),
#endif
    ]
    public static T Usage<T>(string? message = null) => throw With(2, message);

    /// <summary>Terminates this process and returns the exit code to the operating system.</summary>
    /// <param name="message">The message to print into the standard output/error, if specified.</param>
    /// <exception cref="SecurityException">
    /// The caller does not have sufficient security permission to perform this function.
    /// </exception>
    /// <returns>This method does not return. Specified to allow <see keyword="throw"/> expressions.</returns>
    [ContractAnnotation("=> halt"),
     DoesNotReturn,
     SecuritySafeCritical,
#if NETFRAMEWORK || NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_0_OR_GREATER && !NET5_0_OR_GREATER
     SecurityPermission(Demand, Flags = UnmanagedCode),
#endif
    ]
#pragma warning disable CS1573
    static Exception With(byte exitCode, string? message)
#pragma warning restore CS1573
    {
        if (message is not null)
            (exitCode is 0 ? Console.Out : Console.Error).WriteLine(message);

        Environment.Exit(exitCode);
        throw Unreachable;
    }
}
