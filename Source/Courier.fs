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
