/// Defines functions for creating sequences.
module Emik.Morsels.Sequencer

/// Determines whether the coordinates are in range for a 2D array.
let inBounds x y a =
    Array2D.base1 a <= y
    && Array2D.base2 a <= x
    && y < Array2D.length1 a
    && x < Array2D.length2 a

/// Determines whether the coordinates are in range for a 3D array.
let inBounds3 x y z a =
    z < Array3D.length1 a
    && y < Array3D.length2 a
    && x < Array3D.length3 a
    && a.GetLowerBound 0 <= z
    && a.GetLowerBound 1 <= y
    && a.GetLowerBound 2 <= x

/// Determines whether the coordinates are in range for a 4D array.
let inBounds4 x y z w a =
    w < Array4D.length1 a
    && z < Array4D.length2 a
    && y < Array4D.length3 a
    && x < Array4D.length4 a
    && a.GetLowerBound 0 <= w
    && a.GetLowerBound 1 <= z
    && a.GetLowerBound 2 <= y
    && a.GetLowerBound 3 <= x

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

/// Returns the corresponding index if in range for a 2D array, else None.
let tryGet x y a = if inBounds x y a then Some (a[y, x]) else None

/// Returns the corresponding index if in range for a 3D array, else None.
let tryGet3 x y z a = if inBounds3 x y z a then Some (a[z, y, x]) else None

/// Returns the corresponding index if in range for a 4D array, else None.
let tryGet4 x y z w a =
    if inBounds4 x y z w a then Some (a[w, z, y, x]) else None
