// SPDX-License-Identifier: MIT
// Copyright (C) 2018-present iced project and contributors

use super::super::super::NumberBase;
use std::collections::HashMap;

lazy_static! {
	pub(super) static ref TO_NUMBER_BASE_HASH: HashMap<&'static str, NumberBase> = {
		// GENERATOR-BEGIN: NumberBaseHash
		// ⚠️This was generated by GENERATOR!🦹‍♂️
		let mut h = HashMap::with_capacity(4);
		let _ = h.insert("Hexadecimal", NumberBase::Hexadecimal);
		let _ = h.insert("Decimal", NumberBase::Decimal);
		let _ = h.insert("Octal", NumberBase::Octal);
		let _ = h.insert("Binary", NumberBase::Binary);
		// GENERATOR-END: NumberBaseHash
		h
	};
}
