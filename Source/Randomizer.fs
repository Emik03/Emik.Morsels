/// Defines operators for functions.
module Emik.Morsels.Randomizer

/// Shuffles a sequence based on a function.
let shuffle next xs = xs |> Seq.sortBy (fun _ -> next ())

/// Picks a random element from a sequence based on a function.
let pick next xs = xs |> shuffle next |> Seq.nth 0

/// Attempts to pick a random element from a sequence based on a function.
/// Returns None if the sequence is empty.
let tryPick next xs =
    if xs |> Seq.isEmpty then None else Some (xs |> shuffle next |> Seq.nth 0)
