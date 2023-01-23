// SPDX-License-Identifier: MPL-2.0
/// Extension methods for option types.
module internal Emik.Morsels.OptionExtensions

type Microsoft.FSharp.Core.Option<'a> with

    /// Gets the value, or the fallback.
    member this.getOr f =
        match this with
        | Some x -> x
        | None -> f

    /// Gets the value, or invokes the callback.
    member this.getOr f =
        match this with
        | Some x -> x
        | None -> f ()
