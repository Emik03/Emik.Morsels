// <copyright file="UnreachableException.cs" company="Emik">
// Copyright (c) Emik. This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>
// ReSharper disable once CheckNamespace
#if !NET7_0_OR_GREATER
#pragma warning disable CA1064
namespace System.Diagnostics;

/// <summary>Exception thrown when the program executes an instruction that was thought to be unreachable.</summary>
[Serializable]
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

    UnreachableException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
#endif
