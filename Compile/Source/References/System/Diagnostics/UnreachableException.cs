// SPDX-License-Identifier: MPL-2.0
// ReSharper disable once CheckNamespace EmptyNamespace
namespace System.Diagnostics;
#if !NET7_0_OR_GREATER
/// <summary>Exception thrown when the program executes an instruction that was thought to be unreachable.</summary>
[Serializable]
sealed class UnreachableException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnreachableException"/> class with the default error message.
    /// </summary>
    public UnreachableException()
        : base("The program executed an instruction that was thought to be unreachable.") { }

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

    UnreachableException(SerializationInfo info, StreamingContext context)
#pragma warning disable SYSLIB0051
        : base(info, context) { }
#pragma warning restore SYSLIB0051
}
#endif
