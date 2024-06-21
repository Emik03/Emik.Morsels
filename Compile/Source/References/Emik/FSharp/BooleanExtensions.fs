// SPDX-License-Identifier: MPL-2.0
/// Extension methods for boolean types.
module internal Emik.Morsels.BooleanExtensions

type System.Boolean with
    /// Maps the boolean to Some(f) if true, or None otherwise.
    member this.some f = if this then Some f else None

    /// Maps the boolean to Some(f()) if true, or None otherwise.
    member this.some f = if this then Some (f ()) else None
