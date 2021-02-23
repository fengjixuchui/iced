// SPDX-License-Identifier: MIT
// Copyright (C) 2018-present iced project and contributors

use wasm_bindgen::prelude::*;

// GENERATOR-BEGIN: Enum
// ⚠️This was generated by GENERATOR!🦹‍♂️
/// Rounding control
#[wasm_bindgen]
#[derive(Copy, Clone)]
pub enum RoundingControl {
	/// No rounding mode
	None = 0,
	/// Round to nearest (even)
	RoundToNearest = 1,
	/// Round down (toward -inf)
	RoundDown = 2,
	/// Round up (toward +inf)
	RoundUp = 3,
	/// Round toward zero (truncate)
	RoundTowardZero = 4,
}
// GENERATOR-END: Enum

#[allow(dead_code)]
pub(crate) fn rounding_control_to_iced(value: RoundingControl) -> iced_x86_rust::RoundingControl {
	// Safe, the enums are exactly identical
	unsafe { std::mem::transmute(value as u8) }
}

#[allow(dead_code)]
pub(crate) fn iced_to_rounding_control(value: iced_x86_rust::RoundingControl) -> RoundingControl {
	// Safe, the enums are exactly identical
	unsafe { std::mem::transmute(value as u8) }
}
