/// Declares a simple memoize function.
module Emik.Morsels.Memoize

open System.Collections.Generic

/// Wraps the function around a dictionary such that subsequent
/// calls to the returned memoized function will retrieve a cached
/// result if the argument given has been evaluated before.
/// The function argument is assumed to be deterministic.
let memoize f =
    let dict = Dictionary<_, _> ()

    let fn n =
        match dict.TryGetValue n with
        | true, v -> v
        | _ ->
            let temp = f n
            dict.Add (n, temp)
            temp

    fn
