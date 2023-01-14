/// Defines operators for functions.
module internal Emik.Morsels.Randomizer

open System

/// Shuffles the sequence based on the function.
let shuffle fn xs = xs |> Seq.sortBy (fun _ -> fn Int32.MinValue Int32.MaxValue)

/// Picks the random element from the sequence based on the function.
#if NETFRAMEWORK && !NET40_OR_GREATER
let pickRandom fn xs = xs |> shuffle fn |> Seq.nth 0
#else
let pickRandom fn xs = xs |> shuffle fn |> Seq.item 0
#endif

/// Picks the random index from the sequence based on the function.
let pickIndex fn (xs : _[]) = fn (xs.GetLowerBound 0) xs.Length

/// Picks the random index from the sequence based on the function.
let pickIndex2 fn (xs : _[,]) =
    (fn (xs.GetLowerBound 0) (xs.GetLength 0), fn (xs.GetLowerBound 1) (xs.GetLength 1))

/// Picks the random index from the sequence based on the function.
let pickIndex3 fn (xs : _[,,]) =
    (fn (xs.GetLowerBound 0) (xs.GetLength 0),
     fn (xs.GetLowerBound 1) (xs.GetLength 1),
     fn (xs.GetLowerBound 2) (xs.GetLength 2))

/// Picks the random index from the sequence based on the function.
let pickIndex4 fn (xs : _[,,,]) =
    (fn (xs.GetLowerBound 0) (xs.GetLength 0),
     fn (xs.GetLowerBound 1) (xs.GetLength 1),
     fn (xs.GetLowerBound 2) (xs.GetLength 2),
     fn (xs.GetLowerBound 3) (xs.GetLength 3))

/// Makes the boolean generator out of the number generator.
let toBoolRng fn = (fun _ -> fn 0 2 = 0)

/// Attempts to pick a random element from a sequence based on a function.
/// Returns None if the sequence is empty.
#if NETFRAMEWORK && !NET40_OR_GREATER
let tryPickRandom fn xs = if xs |> Seq.isEmpty then None else Some (xs |> shuffle fn |> Seq.nth 0)
#else
let tryPickRandom fn xs = if xs |> Seq.isEmpty then None else Some (xs |> shuffle fn |> Seq.item 0)
#endif
