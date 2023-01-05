/// Defines operators for functions.
module Emik.Morsels.Randomizer

open System

/// Shuffles a sequence based on a function.
let shuffle fn xs = xs |> Seq.sortBy (fun _ -> fn Int32.MinValue Int32.MaxValue)

/// Picks a random element from a sequence based on a function.
let pick fn xs = xs |> shuffle fn |> Seq.nth 0

/// Attempts to pick a random element from a sequence based on a function.
/// Returns None if the sequence is empty.
let tryPick fn xs =
    if xs |> Seq.isEmpty then None else Some (xs |> shuffle fn |> Seq.nth 0)
