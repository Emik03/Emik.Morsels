// SPDX-License-Identifier: MPL-2.0
/// Extension methods for boolean types.
module internal Emik.Morsels.FSharp

open System
open System.Collections.Generic
open System.Diagnostics

/// Combines two predicate functions together in eager AND form.
let inline (<&&>) ([<InlineIfLambda>] fn) ([<InlineIfLambda>] gn) x = fn x && gn x

/// Combines two predicate functions together in eager OR form.
let inline (<||>) ([<InlineIfLambda>] fn) ([<InlineIfLambda>] gn) x = fn x || gn x

/// Converts the 2-tupled function with the curried equivalent.
let inline curry ([<InlineIfLambda>] fn) a b = fn (a, b)

/// Converts the 3-tupled function with the curried equivalent.
let inline curry3 ([<InlineIfLambda>] fn) a b c = fn (a, b, c)

/// Converts the 4-tupled function with the curried equivalent.
let inline curry4 ([<InlineIfLambda>] fn) a b c d = fn (a, b, c, d)

/// Converts the 5-tupled function with the curried equivalent.
let inline curry5 ([<InlineIfLambda>] fn) a b c d e = fn (a, b, c, d, e)

/// Converts the 6-tupled function with the curried equivalent.
let inline curry6 ([<InlineIfLambda>] fn) a b c d e f = fn (a, b, c, d, e, f)

/// Converts the 7-tupled function with the curried equivalent.
let inline curry7 ([<InlineIfLambda>] fn) a b c d e f g = fn (a, b, c, d, e, f, g)

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
let inline uncurry ([<InlineIfLambda>] fn) (a, b) = fn a b

/// Converts the 3-argument curried function with the tupled equivalent.
let inline uncurry3 ([<InlineIfLambda>] fn) (a, b, c) = fn a b c

/// Converts the 4-argument curried function with the tupled equivalent.
let inline uncurry4 ([<InlineIfLambda>] fn) (a, b, c, d) = fn a b c d

/// Converts the 5-argument curried function with the tupled equivalent.
let inline uncurry5 ([<InlineIfLambda>] fn) (a, b, c, d, e) = fn a b c d e

/// Converts the 6-argument curried function with the tupled equivalent.
let inline uncurry6 ([<InlineIfLambda>] fn) (a, b, c, d, e, f) = fn a b c d e f

/// Converts the 7-argument curried function with the tupled equivalent.
let inline uncurry7 ([<InlineIfLambda>] fn) (a, b, c, d, e, f, g) = fn a b c d e f g

/// Converts the 8-argument curried function with the tupled equivalent.
let inline uncurry8 ([<InlineIfLambda>] fn) (a, b, c, d, e, f, g, h) = fn a b c d e f g h

/// Invokes the function.
let inline invoke ([<InlineIfLambda>] fn) = fn ()

/// Gets the function that ignores the second argument and returns the first.
let inline ignored a _ = a

/// Gets the function that ignores the passed value with false.
let inline fals _ = false

/// Gets the function that ignores the passed value with true.
let inline tru _ = true

/// Drops the first element from the tuple.
let inline dropLeft (_, a, b) = a, b

/// Drops the second element from the tuple.
let inline dropMiddle (a, _, b) = a, b

/// Drops the third element from the tuple.
let inline dropRight (a, b, _) = a, b

/// Wraps the function around a dictionary such that subsequent
/// calls to the returned memoized function will retrieve a cached
/// result if the argument given has been evaluated before.
/// The function argument is assumed to be deterministic.
let inline memoize ([<InlineIfLambda>] fn) =
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
let inline time ([<InlineIfLambda>] fn) =
    let watch = Stopwatch.StartNew ()
    let x = fn ()
    x, watch.Elapsed

/// Shuffles the sequence based on the function.
let inline shuffle ([<InlineIfLambda>] fn) xs =
    let inline fn _ = fn Int32.MinValue Int32.MaxValue
    xs |> Seq.sortBy fn

/// Picks the random element from the sequence based on the function.
#if NETFRAMEWORK && !NET40_OR_GREATER
let inline pickRandom ([<InlineIfLambda>] fn) xs = xs |> shuffle fn |> Seq.nth 0
#else
let inline pickRandom ([<InlineIfLambda>] fn) xs = xs |> shuffle fn |> Seq.item 0
#endif

/// Picks the random index from the sequence based on the function.
let inline pickIndex ([<InlineIfLambda>] fn) (xs : _[]) = xs.GetLowerBound 0 |> fn <| xs.Length

/// Picks the random index from the sequence based on the function.
let inline pickIndex2 ([<InlineIfLambda>] fn) (xs : _[,]) =
    xs.GetLowerBound 0 |> fn <| Array2D.length1 xs, xs.GetLowerBound 1 |> fn <| Array2D.length2 xs

/// Picks the random index from the sequence based on the function.
let inline pickIndex3 ([<InlineIfLambda>] fn) (xs : _[,,]) =
    xs.GetLowerBound 0 |> fn <| Array3D.length1 xs, xs.GetLowerBound 1 |> fn <| Array3D.length2 xs,
    xs.GetLowerBound 2 |> fn <| Array3D.length3 xs

/// Picks the random index from the sequence based on the function.
let inline pickIndex4 ([<InlineIfLambda>] fn) (xs : _[,,,]) =
    xs.GetLowerBound 0 |> fn <| Array4D.length1 xs, xs.GetLowerBound 1 |> fn <| Array4D.length2 xs,
    xs.GetLowerBound 2 |> fn <| Array4D.length3 xs, xs.GetLowerBound 3 |> fn <| Array4D.length4 xs

/// Makes the boolean generator out of the number generator.
let inline toBoolRng ([<InlineIfLambda>] fn) _ = fn 0 2 = 0

/// Attempts to pick a random element from a sequence based on a function.
/// Returns None if the sequence is empty.
#if NETFRAMEWORK && !NET40_OR_GREATER
let inline tryPickRandom ([<InlineIfLambda>] fn) xs =
    if xs |> Seq.isEmpty then None else xs |> shuffle fn |> Seq.nth 0 |> Some
#else
let inline tryPickRandom ([<InlineIfLambda>] fn) xs =
    if xs |> Seq.isEmpty then None else xs |> shuffle fn |> Seq.item 0 |> Some
#endif

/// Determines whether any element in the sequence is equal to the argument.
let inline contains a xs = xs |> Seq.exists ((=) a)

/// Determines whether the coordinates are in range for a 2D array.
let inline inBounds y x xs =
    Array2D.base1 xs <= y && Array2D.base2 xs <= x && y < Array2D.length1 xs && x < Array2D.length2 xs

/// Determines whether the coordinates are in range for a 3D array.
let inline inBounds3 z y x xs =
    z < Array3D.length1 xs && y < Array3D.length2 xs && x < Array3D.length3 xs &&
    xs.GetLowerBound 0 <= z && xs.GetLowerBound 1 <= y && xs.GetLowerBound 2 <= x

/// Determines whether the coordinates are in range for a 4D array.
let inline inBounds4 w z y x xs =
    w < Array4D.length1 xs && z < Array4D.length2 xs &&
    y < Array4D.length3 xs && x < Array4D.length4 xs &&
    xs.GetLowerBound 0 <= w && xs.GetLowerBound 1 <= z &&
    xs.GetLowerBound 2 <= y && xs.GetLowerBound 3 <= x

/// Gets the jagged array from the 2D array.
let inline toJagged (xs : _[,]) =
    [| for y in xs.GetLowerBound 0 .. xs.GetLength 0 - 1 do
           yield [| for x in xs.GetLowerBound 1 .. xs.GetLength 1 - 1 -> xs[y, x] |] |]

/// Gets the jagged array from the 3D array.
let inline toJagged3 (xs : _[,,]) =
    [| for z in xs.GetLowerBound 0 .. xs.GetLength 0 - 1 do
           yield
               [| for y in xs.GetLowerBound 1 .. xs.GetLength 1 - 1 do
                      yield [| for x in xs.GetLowerBound 2 .. xs.GetLength 2 - 1 ->
                                   xs[z, y, x] |] |] |]

/// Gets the jagged array from the 4D array.
let inline toJagged4 (xs : _[,,,]) =
    [| for w in xs.GetLowerBound 0 .. xs.GetLength 0 - 1 do
           yield
               [| for z in xs.GetLowerBound 1 .. xs.GetLength 1 - 1 do
                      yield
                          [| for y in xs.GetLowerBound 2 .. xs.GetLength 2 - 1 do
                                 yield
                                     [| for x in xs.GetLowerBound 3 .. xs.GetLength 3 - 1 ->
                                            xs[w, z, y, x] |] |] |] |]

/// Gets the sequence from the 2D array.
let inline toSeq (xs : _[,]) =
    seq {
        for y in xs.GetLowerBound 0 .. xs.GetLength 0 - 1 do
            for x in xs.GetLowerBound 1 .. xs.GetLength 1 - 1 do
                yield xs[y, x]
    }

/// Gets the sequence from the 3D array.
let inline toSeq3 (xs : _[,,]) =
    seq {
        for z in xs.GetLowerBound 0 .. xs.GetLength 0 - 1 do
            for y in xs.GetLowerBound 1 .. xs.GetLength 1 - 1 do
                for x in xs.GetLowerBound 2 .. xs.GetLength 2 - 1 do
                    yield xs[z, y, x]
    }

/// Gets the sequence from the 4D array.
let inline toSeq4 (xs : _[,,,]) =
    seq {
        for w in xs.GetLowerBound 0 .. xs.GetLength 0 - 1 do
            for z in xs.GetLowerBound 1 .. xs.GetLength 1 - 1 do
                for y in xs.GetLowerBound 2 .. xs.GetLength 2 - 1 do
                    for x in xs.GetLowerBound 3 .. xs.GetLength 3 - 1 do
                        yield xs[w, z, y, x]
    }

/// Returns the corresponding index if in range for a 2D array, else None.
let inline tryGet y x xs = if xs |> inBounds y x then Some (xs[y, x]) else None

/// Returns the corresponding index if in range for a 3D array, else None.
let inline tryGet3 z y x xs = if xs |> inBounds3 z y x then Some (xs[z, y, x]) else None

/// Returns the corresponding index if in range for a 4D array, else None.
let inline tryGet4 w z y x xs = if xs |> inBounds4 w z y x then Some (xs[w, z, y, x]) else None

/// Removes the item from the list. Only the first occurence of the item will be removed.
let inline remove x xs =
    let rec go n list acc =
        match list with
        | h :: tl when h = n -> acc @ tl
        | h :: tl -> h :: acc |> go n tl
        | [] -> acc

    go x xs []

type Boolean with
    /// Maps the boolean to Some(a) if true, or None otherwise.
    member inline this.some a = if this then Some a else None

    /// Maps the boolean to Some(fn()) if true, or None otherwise.
    member inline this.some ([<InlineIfLambda>] fn) = if this then Some (fn ()) else None

type Option<'a> with
    /// Gets the value, or the fallback.
    member inline this.getOr a =
        match this with
        | Some a -> a
        | None -> a

    /// Gets the value, or invokes the callback.
    member inline this.getOr ([<InlineIfLambda>] fn) =
        match this with
        | Some a -> a
        | None -> fn ()
