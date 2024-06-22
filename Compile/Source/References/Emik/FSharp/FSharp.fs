// SPDX-License-Identifier: MPL-2.0
/// Extension methods for boolean types.
module internal Emik.Morsels.FSharp

open System
open System.Collections.Generic
open System.Diagnostics

/// Combines two predicate functions together in eager AND form.
let inline (<&&>) f g x = f x && g x

/// Combines two predicate functions together in eager OR form.
let inline (<||>) f g x = f x || g x

/// Converts the 2-tupled function with the curried equivalent.
let inline curry fn a b = fn (a, b)

/// Converts the 3-tupled function with the curried equivalent.
let inline curry3 fn a b c = fn (a, b, c)

/// Converts the 4-tupled function with the curried equivalent.
let inline curry4 fn a b c d = fn (a, b, c, d)

/// Converts the 5-tupled function with the curried equivalent.
let inline curry5 fn a b c d e = fn (a, b, c, d, e)

/// Converts the 6-tupled function with the curried equivalent.
let inline curry6 fn a b c d e f = fn (a, b, c, d, e, f)

/// Converts the 7-tupled function with the curried equivalent.
let inline curry7 fn a b c d e f g = fn (a, b, c, d, e, f, g)

/// Maps the 2 arguments into a tuple.
let inline tuple a b = a, b

/// Maps the 3 arguments into a tuple.
let inline tuple3 a b c = a, b, c

/// Maps the 4 arguments into a tuple.
let inline tuple4 a b c d = a, b, c, d

/// Maps the 5 arguments into a tuple.
let inline tuple5 a b c d e = a, b, c, d, e

/// Maps the 6 arguments into a tuple.
let inline tuple6 a b c d e f = a, b, c, d, e, f

/// Maps the 7 arguments into a tuple.
let inline tuple7 a b c d e f g = a, b, c, d, e, f, g

/// Maps the 8 arguments into a tuple.
let inline tuple8 a b c d e f g h = a, b, c, d, e, f, g, h

/// Converts the 2-argument curried function with the tupled equivalent.
let inline uncurry fn (a, b) = fn a b

/// Converts the 3-argument curried function with the tupled equivalent.
let inline uncurry3 fn (a, b, c) = fn a b c

/// Converts the 4-argument curried function with the tupled equivalent.
let inline uncurry4 fn (a, b, c, d) = fn a b c d

/// Converts the 5-argument curried function with the tupled equivalent.
let inline uncurry5 fn (a, b, c, d, e) = fn a b c d e

/// Converts the 6-argument curried function with the tupled equivalent.
let inline uncurry6 fn (a, b, c, d, e, f) = fn a b c d e f

/// Converts the 7-argument curried function with the tupled equivalent.
let inline uncurry7 fn (a, b, c, d, e, f, g) = fn a b c d e f g

/// Converts the 8-argument curried function with the tupled equivalent.
let inline uncurry8 fn (a, b, c, d, e, f, g, h) = fn a b c d e f g h

/// Invokes the function.
let inline invoke fn = fn ()

/// Gets the function that ignores the second argument and returns the first.
let inline ignored a _ = a

/// Gets the function that ignores the passed value with false.
let inline fals _ = false

/// Gets the function that ignores the passed value with true.
let inline tru _ = true

/// Drops the first element from the tuple.
let inline dropLeft (_, b, c) = b, c

/// Drops the second element from the tuple.
let inline dropMiddle (a, _, c) = a, c

/// Drops the third element from the tuple.
let inline dropRight (a, b, _) = a, b

/// Wraps the function around a dictionary such that subsequent
/// calls to the returned memoized function will retrieve a cached
/// result if the argument given has been evaluated before.
/// The function argument is assumed to be deterministic.
let inline memoize fn =
    let dict = Dictionary<_, _> ()

    let inline fn n =
        match dict.TryGetValue n with
        | true, v -> v
        | _ ->
            let temp = fn n
            dict.Add (n, temp)
            temp

    fn

/// Computes the execution time of the function.
let inline time fn =
    let watch = Stopwatch ()
    watch.Start ()
    let x = fn ()
    watch.Stop ()
    x, watch.Elapsed

/// Shuffles the sequence based on the function.
let inline shuffle fn xs =
    let inline fn _ = fn Int32.MinValue Int32.MaxValue
    xs |> Seq.sortBy fn

/// Picks the random element from the sequence based on the function.
#if NETFRAMEWORK && !NET40_OR_GREATER
let inline pickRandom fn xs = xs |> shuffle fn |> Seq.nth 0
#else
let inline pickRandom fn xs = xs |> shuffle fn |> Seq.item 0
#endif

/// Picks the random index from the sequence based on the function.
let inline pickIndex fn (xs : _[]) = xs.GetLowerBound 0 |> fn <| xs.Length

/// Picks the random index from the sequence based on the function.
let inline pickIndex2 fn (xs : _[,]) =
    xs.GetLowerBound 0 |> fn <| Array2D.length1 xs, xs.GetLowerBound 1 |> fn <| Array2D.length2 xs

/// Picks the random index from the sequence based on the function.
let inline pickIndex3 fn (xs : _[,,]) =
    xs.GetLowerBound 0 |> fn <| Array3D.length1 xs, xs.GetLowerBound 1 |> fn <| Array3D.length2 xs,
    xs.GetLowerBound 2 |> fn <| Array3D.length3 xs

/// Picks the random index from the sequence based on the function.
let inline pickIndex4 fn (xs : _[,,,]) =
    xs.GetLowerBound 0 |> fn <| Array4D.length1 xs, xs.GetLowerBound 1 |> fn <| Array4D.length2 xs,
    xs.GetLowerBound 2 |> fn <| Array4D.length3 xs, xs.GetLowerBound 3 |> fn <| Array4D.length4 xs

/// Makes the boolean generator out of the number generator.
let inline toBoolRng fn _ = fn 0 2 = 0

/// Attempts to pick a random element from a sequence based on a function.
/// Returns None if the sequence is empty.
#if NETFRAMEWORK && !NET40_OR_GREATER
let inline tryPickRandom fn xs =
    if xs |> Seq.isEmpty then None else xs |> shuffle fn |> Seq.nth 0 |> Some
#else
let inline tryPickRandom fn xs =
    if xs |> Seq.isEmpty then None else xs |> shuffle fn |> Seq.item 0 |> Some
#endif

/// Determines whether any element in the sequence is equal to the argument.
let inline contains i s = s |> Seq.exists ((=) i)

/// Determines whether the coordinates are in range for a 2D array.
let inline inBounds y x a =
    Array2D.base1 a <= y && Array2D.base2 a <= x && y < Array2D.length1 a && x < Array2D.length2 a

/// Determines whether the coordinates are in range for a 3D array.
let inline inBounds3 z y x a =
    z < Array3D.length1 a && y < Array3D.length2 a && x < Array3D.length3 a &&
    a.GetLowerBound 0 <= z && a.GetLowerBound 1 <= y && a.GetLowerBound 2 <= x

/// Determines whether the coordinates are in range for a 4D array.
let inline inBounds4 w z y x a =
    w < Array4D.length1 a && z < Array4D.length2 a &&
    y < Array4D.length3 a && x < Array4D.length4 a &&
    a.GetLowerBound 0 <= w && a.GetLowerBound 1 <= z &&
    a.GetLowerBound 2 <= y && a.GetLowerBound 3 <= x

/// Gets the jagged array from the 2D array.
let inline toJagged (a : _[,]) =
    [| for y in a.GetLowerBound 0 .. a.GetLength 0 - 1 do
           yield [| for x in a.GetLowerBound 1 .. a.GetLength 1 - 1 -> a[y, x] |] |]

/// Gets the jagged array from the 3D array.
let inline toJagged3 (a : _[,,]) =
    [| for z in a.GetLowerBound 0 .. a.GetLength 0 - 1 do
           yield
               [| for y in a.GetLowerBound 1 .. a.GetLength 1 - 1 do
                      yield [| for x in a.GetLowerBound 2 .. a.GetLength 2 - 1 ->
                                   a[z, y, x] |] |] |]

/// Gets the jagged array from the 4D array.
let inline toJagged4 (a : _[,,,]) =
    [| for w in a.GetLowerBound 0 .. a.GetLength 0 - 1 do
           yield
               [| for z in a.GetLowerBound 1 .. a.GetLength 1 - 1 do
                      yield
                          [| for y in a.GetLowerBound 2 .. a.GetLength 2 - 1 do
                                 yield
                                     [| for x in a.GetLowerBound 3 .. a.GetLength 3 - 1 ->
                                            a[w, z, y, x] |] |] |] |]

/// Gets the sequence from the 2D array.
let inline toSeq (a : _[,]) =
    seq {
        for y in a.GetLowerBound 0 .. a.GetLength 0 - 1 do
            for x in a.GetLowerBound 1 .. a.GetLength 1 - 1 do
                yield a[y, x]
    }

/// Gets the sequence from the 3D array.
let inline toSeq3 (a : _[,,]) =
    seq {
        for z in a.GetLowerBound 0 .. a.GetLength 0 - 1 do
            for y in a.GetLowerBound 1 .. a.GetLength 1 - 1 do
                for x in a.GetLowerBound 2 .. a.GetLength 2 - 1 do
                    yield a[z, y, x]
    }

/// Gets the sequence from the 4D array.
let inline toSeq4 (a : _[,,,]) =
    seq {
        for w in a.GetLowerBound 0 .. a.GetLength 0 - 1 do
            for z in a.GetLowerBound 1 .. a.GetLength 1 - 1 do
                for y in a.GetLowerBound 2 .. a.GetLength 2 - 1 do
                    for x in a.GetLowerBound 3 .. a.GetLength 3 - 1 do
                        yield a[w, z, y, x]
    }

/// Returns the corresponding index if in range for a 2D array, else None.
let inline tryGet y x a = if a |> inBounds y x then Some (a[y, x]) else None

/// Returns the corresponding index if in range for a 3D array, else None.
let inline tryGet3 z y x a = if a |> inBounds3 z y x then Some (a[z, y, x]) else None

/// Returns the corresponding index if in range for a 4D array, else None.
let inline tryGet4 w z y x a = if a |> inBounds4 w z y x then Some (a[w, z, y, x]) else None

/// Removes the item from the list. Only the first occurence of the item will be removed.
let inline remove n list =
    let rec go n list acc =
        match list with
        | h :: tl when h = n -> acc @ tl
        | h :: tl -> h :: acc |> go n tl
        | [] -> acc

    go n list []

type Boolean with
    /// Maps the boolean to Some(f) if true, or None otherwise.
    member inline this.some f = if this then Some f else None

    /// Maps the boolean to Some(f()) if true, or None otherwise.
    member inline this.some f = if this then Some (f ()) else None

type Option<'a> with
    /// Gets the value, or the fallback.
    member inline this.getOr f =
        match this with
        | Some x -> x
        | None -> f

    /// Gets the value, or invokes the callback.
    member inline this.getOr f =
        match this with
        | Some x -> x
        | None -> f ()
