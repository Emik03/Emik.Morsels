/// Defines operators for functions.
module Emik.Morsels.Randomizer

/// Shuffles a sequence based on a function.
let shuffle next xs = xs |> Seq.sortBy (fun _ -> next ())

/// Picks a random element from a sequence based on a function.
let pick next xs = xs |> shuffle next |> Seq.nth 0
