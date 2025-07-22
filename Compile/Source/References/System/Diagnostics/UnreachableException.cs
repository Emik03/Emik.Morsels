// SPDX-License-Identifier: MPL-2.0
#if !NET7_0_OR_GREATER
// ReSharper disable once CheckNamespace
namespace System.Diagnostics;
#pragma warning disable CA1064
/// <summary>Exception thrown when the program executes an instruction that was thought to be unreachable.</summary>
#if !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
[Serializable]
#endif
sealed class UnreachableException : Exception
{
    const string Arg = "The program executed an instruction that was thought to be unreachable.";

    /// <summary>
    /// Initializes a new instance of the <see cref="UnreachableException"/> class with the default error message.
    /// </summary>
    public UnreachableException()
        : base(Arg) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnreachableException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public UnreachableException(string? message)
        : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnreachableException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public UnreachableException(string? message, Exception? innerException)
        : base(message, innerException) { }
#if !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    UnreachableException(SerializationInfo info, StreamingContext context)
#pragma warning disable SYSLIB0051
        : base(info, context) { }
#pragma warning restore SYSLIB0051
#endif
}
#endif
