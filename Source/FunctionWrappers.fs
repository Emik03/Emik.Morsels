// SPDX-License-Identifier: MPL-2.0
/// Declares higher-order functions that decorate pre-existing functions.
module internal Emik.Morsels.FunctionWrappers

open System.Collections.Generic
open System.Diagnostics

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

/// Discards the result of the function and instead gives the execution time.
let time f =
    let watch = Stopwatch ()
    watch.Start ()
    let _ = f ()
    watch.Stop ()
    watch.Elapsed
