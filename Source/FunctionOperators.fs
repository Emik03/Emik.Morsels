/// Defines operators for functions.
module Emik.Morsels.FunctionOperators

/// Combines two predicate functions together in eager AND form.
let (<&&>) f g x = f x && g x

/// Combines two predicate functions together in eager OR form.
let (<||>) f g x = f x || g x
