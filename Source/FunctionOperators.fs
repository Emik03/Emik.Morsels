// SPDX-License-Identifier: MPL-2.0
/// Defines operators for functions.
module internal Emik.Morsels.FunctionOperators

/// Combines two predicate functions together in eager AND form.
let inline (<&&>) f g = (fun x -> f x && g x)

/// Combines two predicate functions together in eager OR form.
let inline (<||>) f g = (fun x -> f x || g x)
