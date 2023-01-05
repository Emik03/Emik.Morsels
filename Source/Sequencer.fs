/// Defines functions for creating sequences.
module Emik.Morsels.Sequencer

/// Gets the sequence from the 2D array.
let toSeq (a : 'a[,]) =
    seq {
        for y in a.GetLowerBound 0 .. a.GetLength 0 - 1 do
            for x in a.GetLowerBound 1 .. a.GetLength 1 - 1 do
                yield a[y, x]
    }

/// Gets the sequence from the 3D array.
let toSeq3 (a : 'a[,,]) =
    seq {
        for z in a.GetLowerBound 0 .. a.GetLength 0 - 1 do
            for y in a.GetLowerBound 1 .. a.GetLength 1 - 1 do
                for x in a.GetLowerBound 2 .. a.GetLength 2 - 1 do
                    yield a[z, y, x]
    }

/// Gets the sequence from the 4D array.
let toSeq4 (a : 'a[,,,]) =
    seq {
        for w in a.GetLowerBound 0 .. a.GetLength 0 - 1 do
            for z in a.GetLowerBound 1 .. a.GetLength 1 - 1 do
                for y in a.GetLowerBound 2 .. a.GetLength 2 - 1 do
                    for x in a.GetLowerBound 3 .. a.GetLength 3 - 1 do
                        yield a[w, z, y, x]
    }
