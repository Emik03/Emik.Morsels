#if NETFRAMEWORK && !NET40_OR_GREATER
namespace System.Runtime.Versioning

open System

/// Identifies the version of .NET that a particular assembly was compiled against.
[<AttributeUsage(AttributeTargets.Assembly); Sealed>]
type TargetFrameworkAttribute (frameworkName : string) =
    inherit Attribute ()
    let mutable _frameworkDisplayName = null
    let _frameworkName = frameworkName

    /// Gets the display name of the .NET version against which an assembly was built.
    member this.FrameworkDisplayName
        with get () = _frameworkName
        and set (value : string) = _frameworkDisplayName <- value

    /// Gets the name of the .NET version against which a particular assembly was compiled.
    member this.FrameworkName = _frameworkName
#endif
