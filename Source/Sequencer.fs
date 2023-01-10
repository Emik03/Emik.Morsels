/// Defines functions for creating sequences.
module internal Emik.Morsels.Sequencer

/// Determines whether any element in the sequence is equal to the argument.
let contains i s = s |> Seq.exists ((=) i)

/// Determines whether the coordinates are in range for a 2D array.
let inBounds y x a =
    Array2D.base1 a <= y && Array2D.base2 a <= x && y < Array2D.length1 a && x < Array2D.length2 a

/// Determines whether the coordinates are in range for a 3D array.
let inBounds3 z y x a =
    z < Array3D.length1 a
    && y < Array3D.length2 a
    && x < Array3D.length3 a
    && a.GetLowerBound 0 <= z
    && a.GetLowerBound 1 <= y
    && a.GetLowerBound 2 <= x

/// Determines whether the coordinates are in range for a 4D array.
let inBounds4 w z y x a =
    w < Array4D.length1 a
    && z < Array4D.length2 a
    && y < Array4D.length3 a
    && x < Array4D.length4 a
    && a.GetLowerBound 0 <= w
    && a.GetLowerBound 1 <= z
    && a.GetLowerBound 2 <= y
    && a.GetLowerBound 3 <= x

/// Gets the jagged array from the 2D array.
let toJagged (a : _[,]) =
    [| for y in a.GetLowerBound 0 .. a.GetLength 0 - 1 do
           yield [| for x in a.GetLowerBound 1 .. a.GetLength 1 - 1 -> a[y, x] |] |]

/// Gets the jagged array from the 3D array.
let toJagged3 (a : _[,,]) =
    [| for z in a.GetLowerBound 0 .. a.GetLength 0 - 1 do
           yield
               [| for y in a.GetLowerBound 1 .. a.GetLength 1 - 1 do
                      yield [| for x in a.GetLowerBound 2 .. a.GetLength 2 - 1 -> a[z, y, x] |] |] |]

/// Gets the jagged array from the 4D array.
let toJagged4 (a : _[,,,]) =
    [| for w in a.GetLowerBound 0 .. a.GetLength 0 - 1 do
           yield
               [| for z in a.GetLowerBound 1 .. a.GetLength 1 - 1 do
                      yield
                          [| for y in a.GetLowerBound 2 .. a.GetLength 2 - 1 do
                                 yield
                                     [| for x in a.GetLowerBound 3 .. a.GetLength 3 - 1 ->
                                            a[w, z, y, x] |] |] |] |]

/// Gets the sequence from the 2D array.
let toSeq (a : _[,]) =
    seq {
        for y in a.GetLowerBound 0 .. a.GetLength 0 - 1 do
            for x in a.GetLowerBound 1 .. a.GetLength 1 - 1 do
                yield a[y, x]
    }

/// Gets the sequence from the 3D array.
let toSeq3 (a : _[,,]) =
    seq {
        for z in a.GetLowerBound 0 .. a.GetLength 0 - 1 do
            for y in a.GetLowerBound 1 .. a.GetLength 1 - 1 do
                for x in a.GetLowerBound 2 .. a.GetLength 2 - 1 do
                    yield a[z, y, x]
    }

/// Gets the sequence from the 4D array.
let toSeq4 (a : _[,,,]) =
    seq {
        for w in a.GetLowerBound 0 .. a.GetLength 0 - 1 do
            for z in a.GetLowerBound 1 .. a.GetLength 1 - 1 do
                for y in a.GetLowerBound 2 .. a.GetLength 2 - 1 do
                    for x in a.GetLowerBound 3 .. a.GetLength 3 - 1 do
                        yield a[w, z, y, x]
    }

/// Returns the corresponding index if in range for a 2D array, else None.
let tryGet y x a = if a |> inBounds y x then Some (a[y, x]) else None

/// Returns the corresponding index if in range for a 3D array, else None.
let tryGet3 z y x a = if a |> inBounds3 z y x then Some (a[z, y, x]) else None

/// Returns the corresponding index if in range for a 4D array, else None.
let tryGet4 w z y x a = if a |> inBounds4 w z y x then Some (a[w, z, y, x]) else None

/// Removes the item from the list. Only the first occurence of the item will be removed.
let remove n list =
    let rec go n list acc =
        match list with
        | h :: tl when h = n -> acc @ tl
        | h :: tl -> h :: acc |> go n tl
        | [] -> acc

    go n list []
