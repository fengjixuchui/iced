// SPDX-License-Identifier: MIT
// Copyright (C) 2018-present iced project and contributors

use wasm_bindgen::prelude::*;

// GENERATOR-BEGIN: Enum
// ⚠️This was generated by GENERATOR!🦹‍♂️
/// `REP`/`REPE`/`REPNE` prefix
#[wasm_bindgen]
#[derive(Copy, Clone)]
pub enum RepPrefixKind {
	/// No `REP`/`REPE`/`REPNE` prefix
	None = 0,
	/// `REP`/`REPE` prefix
	Repe = 1,
	/// `REPNE` prefix
	Repne = 2,
}
// GENERATOR-END: Enum

#[allow(dead_code)]
pub(crate) fn rep_prefix_kind_to_iced(value: RepPrefixKind) -> iced_x86_rust::RepPrefixKind {
	// Safe, the enums are exactly identical
	unsafe { std::mem::transmute(value as u8) }
}
