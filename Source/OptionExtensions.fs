/// Extension methods for option types.
module internal Emik.Morsels.OptionExtensions

type Microsoft.FSharp.Core.Option<'a> with

    /// Gets the value, or the fallback.
    member this.orGet f =
        match this with
        | Some x -> x
        | None -> f

    /// Gets the value, or invokes the callback.
    member this.orGet f =
        match this with
        | Some x -> x
        | None -> f ()
