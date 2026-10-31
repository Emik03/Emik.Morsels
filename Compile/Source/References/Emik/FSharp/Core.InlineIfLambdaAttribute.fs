// SPDX-License-Identifier: MPL-2.0
namespace FSharp.Core
#if NETFRAMEWORK && !NET40_OR_GREATER

open System

/// This function should be statically inlined throughout the body of the function of method.
[<AttributeUsage(AttributeTargets.Parameter); Sealed>]
type internal InlineIfLambdaAttribute () = inherit Attribute ()
#endif
