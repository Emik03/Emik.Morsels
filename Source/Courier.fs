/// Provides functions to convert tupled functions into curried.
module Emik.Morsels.Courier

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
let inline tuple a b = (a, b)

/// Maps the 3 arguments into a tuple.
let inline tuple3 a b c = (a, b, c)

/// Maps the 4 arguments into a tuple.
let inline tuple4 a b c d = (a, b, c, d)

/// Maps the 5 arguments into a tuple.
let inline tuple5 a b c d e = (a, b, c, d, e)

/// Maps the 6 arguments into a tuple.
let inline tuple6 a b c d e f = (a, b, c, d, e, f)

/// Maps the 7 arguments into a tuple.
let inline tuple7 a b c d e f g = (a, b, c, d, e, f, g)

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

/// Invokes the function.
let inline invoke fn = fn ()

/// Returns the argument.
let inline id a = a

/// Takes the first element of the tuple.
let inline left (a, _) = a

/// Takes the second element of the tuple.
let inline right (_, b) = b

/// Drops the first element from the tuple.
let inline dropLeft (_, b, c) = (b, c)

/// Drops the second element from the tuple.
let inline dropMiddle (a, _, c) = (a, c)

/// Drops the third element from the tuple.
let inline dropRight (a, b, _) = (a, b)
