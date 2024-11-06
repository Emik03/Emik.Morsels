// SPDX-License-Identifier: MPL-2.0
namespace System.Runtime.Versioning
#if NETFRAMEWORK && !NET40_OR_GREATER

open System

/// Identifies the version of .NET that a particular assembly was compiled against.
[<AttributeUsage(AttributeTargets.Assembly); Sealed>]
type private TargetFrameworkAttribute (frameworkName : string) =
    inherit Attribute ()
    let mutable _frameworkDisplayName = ""
    let _frameworkName = frameworkName

    /// Gets the display name of the .NET version against which an assembly was built.
    member this.FrameworkDisplayName
        with get () = _frameworkDisplayName
        and set value = _frameworkDisplayName <- value

    /// Gets the name of the .NET version against which a particular assembly was compiled.
    member this.FrameworkName = _frameworkName
#endif
