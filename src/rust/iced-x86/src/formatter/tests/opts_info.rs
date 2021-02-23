// SPDX-License-Identifier: MIT
// Copyright (C) 2018-present iced project and contributors

use super::super::super::*;
use super::enums::OptionsProps;
use super::opt_value::OptionValue;
use alloc::string::String;
use alloc::vec::Vec;

pub(super) struct OptionsInstructionInfo {
	pub(super) bitness: u32,
	pub(super) hex_bytes: String,
	pub(super) ip: u64,
	pub(super) decoder_options: u32,
	pub(super) code: Code,
	pub(super) vec: Vec<(OptionsProps, OptionValue)>,
}

impl OptionsInstructionInfo {
	#[cfg(any(feature = "gas", feature = "intel", feature = "masm", feature = "nasm"))]
	pub(super) fn initialize_options(&self, options: &mut FormatterOptions) {
		for info in &self.vec {
			info.1.initialize_options(options, info.0);
		}
	}

	#[cfg(feature = "fast_fmt")]
	pub(super) fn initialize_options_fast(&self, options: &mut FastFormatterOptions) {
		for info in &self.vec {
			info.1.initialize_options_fast(options, info.0);
		}
	}

	pub(super) fn initialize_decoder(&self, decoder: &mut Decoder) {
		for info in &self.vec {
			info.1.initialize_decoder(decoder, info.0);
		}
	}
}
