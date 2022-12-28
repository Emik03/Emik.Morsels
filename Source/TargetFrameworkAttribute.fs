#if NETFRAMEWORK && !NET40_OR_GREATER
namespace System.Runtime.Versioning

open System

[<AttributeUsage(AttributeTargets.Assembly); Sealed>]
type TargetFrameworkAttribute (frameworkName : string) =
    inherit Attribute ()
    let mutable _frameworkDisplayName = null
    let _frameworkName = frameworkName
    member this.FrameworkName = _frameworkName

    member this.FrameworkDisplayName
        with get () = _frameworkName
        and set (value : string) = _frameworkDisplayName <- value

#endif
