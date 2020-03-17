/*
Copyright (C) 2018-2019 de4dot@gmail.com

Permission is hereby granted, free of charge, to any person obtaining
a copy of this software and associated documentation files (the
"Software"), to deal in the Software without restriction, including
without limitation the rights to use, copy, modify, merge, publish,
distribute, sublicense, and/or sell copies of the Software, and to
permit persons to whom the Software is furnished to do so, subject to
the following conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

mod code_table;
#[cfg(feature = "instr_info")]
mod condition_code_table;
#[cfg(feature = "instr_info")]
mod cpuid_feature_table;
#[cfg(any(feature = "gas", feature = "intel", feature = "masm", feature = "nasm"))]
mod decoder_options_table;
#[cfg(feature = "instr_info")]
mod encoding_kind_table;
#[cfg(feature = "instr_info")]
mod flow_control_table;
#[cfg(any(feature = "gas", feature = "intel", feature = "masm", feature = "nasm"))]
mod memory_size_options_table;
mod memory_size_table;
mod mnemonic_table;
#[cfg(any(feature = "gas", feature = "intel", feature = "masm", feature = "nasm"))]
mod number_base_table;
#[cfg(feature = "encoder")]
mod op_code_operand_kind_table;
#[cfg(any(feature = "gas", feature = "intel", feature = "masm", feature = "nasm"))]
mod options_props_table;
mod register_table;
#[cfg(feature = "encoder")]
mod tuple_type_table;

use self::code_table::*;
#[cfg(feature = "instr_info")]
use self::condition_code_table::*;
#[cfg(feature = "instr_info")]
use self::cpuid_feature_table::*;
#[cfg(any(feature = "gas", feature = "intel", feature = "masm", feature = "nasm"))]
use self::decoder_options_table::*;
#[cfg(feature = "instr_info")]
use self::encoding_kind_table::*;
#[cfg(feature = "instr_info")]
use self::flow_control_table::*;
#[cfg(any(feature = "gas", feature = "intel", feature = "masm", feature = "nasm"))]
use self::memory_size_options_table::*;
use self::memory_size_table::*;
use self::mnemonic_table::*;
#[cfg(any(feature = "gas", feature = "intel", feature = "masm", feature = "nasm"))]
use self::number_base_table::*;
#[cfg(feature = "encoder")]
use self::op_code_operand_kind_table::*;
#[cfg(any(feature = "gas", feature = "intel", feature = "masm", feature = "nasm"))]
use self::options_props_table::*;
use self::register_table::*;
#[cfg(feature = "encoder")]
use self::tuple_type_table::*;
#[cfg(any(feature = "gas", feature = "intel", feature = "masm", feature = "nasm"))]
use super::super::formatter::tests::enums::OptionsProps;
use super::super::*;
#[cfg(not(feature = "std"))]
use alloc::string::String;
#[cfg(not(feature = "std"))]
use alloc::vec::Vec;
#[cfg(any(feature = "gas", feature = "intel", feature = "masm", feature = "nasm"))]
use core::{i16, i8};
use core::{i32, u16, u32, u8};
#[cfg(feature = "instr_info")]
#[cfg(not(feature = "std"))]
use hashbrown::HashMap;
#[cfg(feature = "instr_info")]
#[cfg(feature = "std")]
use std::collections::HashMap;

pub(crate) fn to_vec_u8(hex_data: &str) -> Result<Vec<u8>, String> {
	let mut bytes = Vec::with_capacity(hex_data.len() / 2);
	let mut iter = hex_data.chars().filter(|c| !c.is_whitespace());
	loop {
		let hi = try_parse_hex_char(match iter.next() {
			Some(c) => c,
			None => break,
		});
		let lo = try_parse_hex_char(match iter.next() {
			Some(c) => c,
			None => return Err(format!("Missing hex digit in string: '{}'", hex_data)),
		});
		if hi < 0 || lo < 0 {
			return Err(format!("Invalid hex string: '{}'", hex_data));
		}
		bytes.push(((hi << 4) | lo) as u8);
	}

	fn try_parse_hex_char(c: char) -> i32 {
		if '0' <= c && c <= '9' {
			return c as i32 - '0' as i32;
		}
		if 'A' <= c && c <= 'F' {
			return c as i32 - 'A' as i32 + 10;
		}
		if 'a' <= c && c <= 'f' {
			return c as i32 - 'a' as i32 + 10;
		}
		-1
	}

	Ok(bytes)
}

pub(crate) fn to_u64(value: &str) -> Result<u64, String> {
	let value = value.trim();
	let result = if value.starts_with("0x") { u64::from_str_radix(&value[2..], 16) } else { value.trim().parse() };
	match result {
		Ok(value) => Ok(value),
		Err(_) => Err(format!("Invalid number: {}", value)),
	}
}

#[cfg(any(feature = "encoder", feature = "instr_info", feature = "gas", feature = "intel", feature = "masm", feature = "nasm"))]
pub(crate) fn to_i64(value: &str) -> Result<i64, String> {
	let mut unsigned_value = value.trim();
	let mult = if unsigned_value.starts_with('-') {
		unsigned_value = &unsigned_value[1..];
		-1
	} else {
		1
	};
	let result = if unsigned_value.starts_with("0x") { u64::from_str_radix(&unsigned_value[2..], 16) } else { unsigned_value.trim().parse() };
	match result {
		Ok(value) => Ok((value as i64).wrapping_mul(mult)),
		Err(_) => Err(format!("Invalid number: {}", value)),
	}
}

pub(crate) fn to_u32(value: &str) -> Result<u32, String> {
	let value = value.trim();
	if let Ok(v64) = to_u64(value) {
		if v64 <= u32::MAX as u64 {
			return Ok(v64 as u32);
		}
	}
	Err(format!("Invalid number: {}", value))
}

#[cfg(any(feature = "encoder", feature = "instr_info", feature = "gas", feature = "intel", feature = "masm", feature = "nasm"))]
pub(crate) fn to_i32(value: &str) -> Result<i32, String> {
	let value = value.trim();
	if let Ok(v64) = to_i64(value) {
		if i32::MIN as i64 <= v64 && v64 <= i32::MAX as i64 {
			return Ok(v64 as i32);
		}
	}
	Err(format!("Invalid number: {}", value))
}

pub(crate) fn to_u16(value: &str) -> Result<u16, String> {
	let value = value.trim();
	if let Ok(v64) = to_u64(value) {
		if v64 <= u16::MAX as u64 {
			return Ok(v64 as u16);
		}
	}
	Err(format!("Invalid number: {}", value))
}

#[cfg(any(feature = "gas", feature = "intel", feature = "masm", feature = "nasm"))]
pub(crate) fn to_i16(value: &str) -> Result<i16, String> {
	let value = value.trim();
	if let Ok(v64) = to_i64(value) {
		if i16::MIN as i64 <= v64 && v64 <= i16::MAX as i64 {
			return Ok(v64 as i16);
		}
	}
	Err(format!("Invalid number: {}", value))
}

pub(crate) fn to_u8(value: &str) -> Result<u8, String> {
	let value = value.trim();
	if let Ok(v64) = to_u64(value) {
		if v64 <= u8::MAX as u64 {
			return Ok(v64 as u8);
		}
	}
	Err(format!("Invalid number: {}", value))
}

#[cfg(any(feature = "gas", feature = "intel", feature = "masm", feature = "nasm"))]
pub(crate) fn to_i8(value: &str) -> Result<i8, String> {
	let value = value.trim();
	if let Ok(v64) = to_i64(value) {
		if i8::MIN as i64 <= v64 && v64 <= i8::MAX as i64 {
			return Ok(v64 as i8);
		}
	}
	Err(format!("Invalid number: {}", value))
}

pub(crate) fn to_code(value: &str) -> Result<Code, String> {
	let value = value.trim();
	match TO_CODE_HASH.get(value) {
		Some(code) => Ok(*code),
		None => Err(format!("Invalid Code value: {}", value)),
	}
}

#[cfg(any(feature = "gas", feature = "intel", feature = "masm", feature = "nasm"))]
pub(crate) fn code_names() -> Vec<&'static str> {
	(&*TO_CODE_HASH).iter().map(|kv| *kv.0).collect()
}

pub(crate) fn to_mnemonic(value: &str) -> Result<Mnemonic, String> {
	let value = value.trim();
	match TO_MNEMONIC_HASH.get(value) {
		Some(mnemonic) => Ok(*mnemonic),
		None => Err(format!("Invalid Mnemonic value: {}", value)),
	}
}

pub(crate) fn to_register(value: &str) -> Result<Register, String> {
	let value = value.trim();
	if value.is_empty() {
		return Ok(Register::None);
	}
	match TO_REGISTER_HASH.get(value) {
		Some(register) => Ok(*register),
		None => Err(format!("Invalid Register value: {}", value)),
	}
}

#[cfg(feature = "instr_info")]
pub(crate) fn clone_register_hashmap() -> HashMap<String, Register> {
	TO_REGISTER_HASH.iter().map(|kv| ((*kv.0).to_string(), *kv.1)).collect()
}

pub(crate) fn to_memory_size(value: &str) -> Result<MemorySize, String> {
	let value = value.trim();
	match TO_MEMORY_SIZE_HASH.get(value) {
		Some(memory_size) => Ok(*memory_size),
		None => Err(format!("Invalid MemorySize value: {}", value)),
	}
}

#[cfg(any(feature = "gas", feature = "intel", feature = "masm", feature = "nasm"))]
pub(crate) fn to_decoder_options(value: &str) -> Result<u32, String> {
	let value = value.trim();
	match TO_DECODER_OPTIONS_HASH.get(value) {
		Some(decoder_options) => Ok(*decoder_options),
		None => Err(format!("Invalid DecoderOptions value: {}", value)),
	}
}

#[cfg(feature = "instr_info")]
pub(crate) fn to_encoding_kind(value: &str) -> Result<EncodingKind, String> {
	let value = value.trim();
	match TO_ENCODING_KIND_HASH.get(value) {
		Some(encoding_kind) => Ok(*encoding_kind),
		None => Err(format!("Invalid EncodingKind value: {}", value)),
	}
}

#[cfg(feature = "encoder")]
pub(crate) fn to_tuple_type(value: &str) -> Result<TupleType, String> {
	let value = value.trim();
	match TO_TUPLE_TYPE_HASH.get(value) {
		Some(tuple_type) => Ok(*tuple_type),
		None => Err(format!("Invalid TupleType value: {}", value)),
	}
}

#[cfg(feature = "instr_info")]
pub(crate) fn to_cpuid_features(value: &str) -> Result<CpuidFeature, String> {
	let value = value.trim();
	match TO_CPUID_FEATURE_HASH.get(value) {
		Some(cpuid_features) => Ok(*cpuid_features),
		None => Err(format!("Invalid CpuidFeature value: {}", value)),
	}
}

#[cfg(feature = "instr_info")]
pub(crate) fn to_flow_control(value: &str) -> Result<FlowControl, String> {
	let value = value.trim();
	match TO_FLOW_CONTROL_HASH.get(value) {
		Some(flow_control) => Ok(*flow_control),
		None => Err(format!("Invalid FlowControl value: {}", value)),
	}
}

#[cfg(feature = "encoder")]
pub(crate) fn to_op_code_operand_kind(value: &str) -> Result<OpCodeOperandKind, String> {
	let value = value.trim();
	match TO_OP_CODE_OPERAND_KIND_HASH.get(value) {
		Some(op_code_operand_kind) => Ok(*op_code_operand_kind),
		None => Err(format!("Invalid OpCodeOperandKind value: {}", value)),
	}
}

#[cfg(feature = "instr_info")]
pub(crate) fn to_condition_code(value: &str) -> Result<ConditionCode, String> {
	let value = value.trim();
	match TO_CONDITION_CODE_HASH.get(value) {
		Some(condition_code) => Ok(*condition_code),
		None => Err(format!("Invalid ConditionCode value: {}", value)),
	}
}

#[cfg(any(feature = "gas", feature = "intel", feature = "masm", feature = "nasm"))]
pub(crate) fn to_options_props(value: &str) -> Result<OptionsProps, String> {
	let value = value.trim();
	match TO_OPTIONS_PROPS_HASH.get(value) {
		Some(options_props) => Ok(*options_props),
		None => Err(format!("Invalid OptionsProps value: {}", value)),
	}
}

#[cfg(any(feature = "gas", feature = "intel", feature = "masm", feature = "nasm"))]
pub(crate) fn to_memory_size_options(value: &str) -> Result<MemorySizeOptions, String> {
	let value = value.trim();
	match TO_MEMORY_SIZE_OPTIONS_HASH.get(value) {
		Some(memory_size_options) => Ok(*memory_size_options),
		None => Err(format!("Invalid MemorySizeOptions value: {}", value)),
	}
}

#[cfg(any(feature = "gas", feature = "intel", feature = "masm", feature = "nasm"))]
pub(crate) fn to_number_base(value: &str) -> Result<NumberBase, String> {
	let value = value.trim();
	match TO_NUMBER_BASE_HASH.get(value) {
		Some(number_base) => Ok(*number_base),
		None => Err(format!("Invalid NumberBase value: {}", value)),
	}
}

#[cfg(any(feature = "gas", feature = "intel", feature = "masm", feature = "nasm"))]
pub(crate) fn number_base_len() -> usize {
	TO_NUMBER_BASE_HASH.len()
}

#[cfg(any(feature = "gas", feature = "intel", feature = "masm", feature = "nasm"))]
pub(crate) fn to_boolean(value: &str) -> Result<bool, String> {
	let value = value.trim();
	match value {
		"false" => Ok(false),
		"true" => Ok(true),
		_ => Err(format!("Invalid boolean value: {}", value)),
	}
}
