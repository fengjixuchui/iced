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

mod enums;
mod fmt_consts;
mod fmt_opt_provider;
mod fmt_opts;
mod fmt_utils;
#[cfg(feature = "gas")]
mod gas;
#[cfg(feature = "intel")]
mod intel;
#[cfg(feature = "masm")]
mod masm;
#[cfg(feature = "nasm")]
mod nasm;
mod num_fmt;
mod num_fmt_opts;
mod pseudo_ops;
mod regs_tbl;
mod string_output;
mod strings_data;
mod strings_tbl;
mod symres;
#[cfg(test)]
pub(crate) mod tests;

pub use self::enums::*;
pub use self::fmt_opt_provider::*;
pub use self::fmt_opts::*;
#[cfg(feature = "gas")]
pub use self::gas::*;
#[cfg(feature = "intel")]
pub use self::intel::*;
#[cfg(feature = "masm")]
pub use self::masm::*;
#[cfg(feature = "nasm")]
pub use self::nasm::*;
use self::num_fmt::NumberFormatter;
pub use self::num_fmt_opts::*;
pub use self::string_output::*;
pub use self::symres::*;
use super::*;
#[cfg(not(feature = "std"))]
use alloc::string::String;
#[cfg(not(feature = "std"))]
use alloc::vec::Vec;
use core::{i16, i32, i8, u16, u32, u8};

#[derive(Debug, Default, Clone)]
struct FormatterString {
	lower: String,
	upper: String,
}

impl FormatterString {
	#[cfg_attr(has_must_use, must_use)]
	fn new(lower: String) -> Self {
		debug_assert_eq!(lower, lower.to_lowercase());
		Self { upper: lower.to_uppercase(), lower }
	}

	#[cfg_attr(has_must_use, must_use)]
	fn with_strings(strings: Vec<String>) -> Vec<Self> {
		strings.into_iter().map(FormatterString::new).collect()
	}

	#[cfg_attr(has_must_use, must_use)]
	fn new_str(lower: &str) -> Self {
		debug_assert_eq!(lower, lower.to_lowercase());
		Self { lower: String::from(lower), upper: lower.to_uppercase() }
	}

	#[cfg_attr(has_must_use, must_use)]
	#[inline]
	fn len(&self) -> usize {
		self.lower.len()
	}

	#[cfg(any(feature = "gas", feature = "intel", feature = "nasm"))]
	#[cfg_attr(has_must_use, must_use)]
	#[inline]
	fn is_default(&self) -> bool {
		self.lower.is_empty()
	}

	#[cfg_attr(has_must_use, must_use)]
	#[inline]
	fn get(&self, upper: bool) -> &str {
		if upper {
			&self.upper
		} else {
			&self.lower
		}
	}
}

/// Used by a [`Formatter`] to write all text. `String` also implements this trait.
///
/// The only method that must be implemented is [`write()`], all other methods call it if they're not overridden.
///
/// [`Formatter`]: trait.Formatter.html
/// [`write()`]: #tymethod.write
pub trait FormatterOutput {
	/// Writes text and text kind
	///
	/// # Arguments
	///
	/// - `text`: Text
	/// - `kind`: Text kind
	fn write(&mut self, text: &str, kind: FormatterTextKind);

	/// Writes a prefix
	///
	/// # Arguments
	///
	/// - `instruction`: Instruction
	/// - `text`: Prefix text
	/// - `prefix`: Prefix
	#[inline]
	#[allow(unused_variables)]
	fn write_prefix(&mut self, instruction: &Instruction, text: &str, prefix: PrefixKind) {
		self.write(text, FormatterTextKind::Prefix);
	}

	/// Writes a mnemonic (see [`Instruction::mnemonic()`])
	///
	/// [`Instruction::mnemonic()`]: struct.Instruction.html#method.mnemonic
	///
	/// - `instruction`: Instruction
	/// - `text`: Mnemonic text
	#[inline]
	#[allow(unused_variables)]
	fn write_mnemonic(&mut self, instruction: &Instruction, text: &str) {
		self.write(text, FormatterTextKind::Mnemonic);
	}

	/// Writes a number
	///
	/// # Arguments
	///
	/// - `instruction`: Instruction
	/// - `operand`: Operand number, 0-based. This is a formatter operand and isn't necessarily the same as an instruction operand.
	/// - `instruction_operand`: Instruction operand number, 0-based, or `None` if it's an operand created by the formatter.
	/// - `text`: Number text
	/// - `value`: Value
	/// - `number_kind`: Number kind
	/// - `kind`: Text kind
	#[inline]
	#[cfg_attr(feature = "cargo-clippy", allow(clippy::too_many_arguments))]
	#[allow(unused_variables)]
	fn write_number(
		&mut self, instruction: &Instruction, operand: u32, instruction_operand: Option<u32>, text: &str, value: u64, number_kind: NumberKind,
		kind: FormatterTextKind,
	) {
		self.write(text, kind);
	}

	/// Writes a decorator
	///
	/// # Arguments
	///
	/// - `instruction`: Instruction
	/// - `operand`: Operand number, 0-based. This is a formatter operand and isn't necessarily the same as an instruction operand.
	/// - `instruction_operand`: Instruction operand number, 0-based, or `None` if it's an operand created by the formatter.
	/// - `text`: Decorator text
	/// - `decorator`: Decorator
	#[inline]
	#[allow(unused_variables)]
	fn write_decorator(&mut self, instruction: &Instruction, operand: u32, instruction_operand: Option<u32>, text: &str, decorator: DecoratorKind) {
		self.write(text, FormatterTextKind::Decorator);
	}

	/// Writes a register
	///
	/// # Arguments
	///
	/// - `instruction`: Instruction
	/// - `operand`: Operand number, 0-based. This is a formatter operand and isn't necessarily the same as an instruction operand.
	/// - `instruction_operand`: Instruction operand number, 0-based, or `None` if it's an operand created by the formatter.
	/// - `text`: Register text
	/// - `register`: Register
	#[inline]
	#[allow(unused_variables)]
	fn write_register(&mut self, instruction: &Instruction, operand: u32, instruction_operand: Option<u32>, text: &str, register: Register) {
		self.write(text, FormatterTextKind::Register);
	}

	/// Writes a symbol
	///
	/// # Arguments
	///
	/// - `instruction`: Instruction
	/// - `operand`: Operand number, 0-based. This is a formatter operand and isn't necessarily the same as an instruction operand.
	/// - `instruction_operand`: Instruction operand number, 0-based, or `None` if it's an operand created by the formatter.
	/// - `address`: Address
	/// - `symbol`: Symbol
	#[inline]
	#[allow(unused_variables)]
	fn write_symbol(&mut self, instruction: &Instruction, operand: u32, instruction_operand: Option<u32>, address: u64, symbol: &SymbolResult) {
		match symbol.text {
			SymResTextInfo::Text(ref part) => {
				let s = match &part.text {
					&SymResString::Str(s) => s,
					&SymResString::String(ref s) => s.as_str(),
				};
				self.write(s, part.color);
			}

			SymResTextInfo::TextVec(v) => {
				for part in v.iter() {
					let s = match &part.text {
						&SymResString::Str(s) => s,
						&SymResString::String(ref s) => s.as_str(),
					};
					self.write(s, part.color);
				}
			}
		}
	}
}

struct FormatterOutputMethods;
impl FormatterOutputMethods {
	#[cfg_attr(feature = "cargo-clippy", allow(clippy::too_many_arguments))]
	fn write1(
		output: &mut FormatterOutput, instruction: &Instruction, operand: u32, instruction_operand: Option<u32>, options: &FormatterOptions,
		number_formatter: &mut NumberFormatter, number_options: &NumberFormattingOptions, address: u64, symbol: &SymbolResult,
		show_symbol_address: bool,
	) {
		FormatterOutputMethods::write2(
			output,
			instruction,
			operand,
			instruction_operand,
			options,
			number_formatter,
			number_options,
			address,
			symbol,
			show_symbol_address,
			true,
			false,
		);
	}

	#[cfg_attr(feature = "cargo-clippy", allow(clippy::too_many_arguments))]
	fn write2(
		output: &mut FormatterOutput, instruction: &Instruction, operand: u32, instruction_operand: Option<u32>, options: &FormatterOptions,
		number_formatter: &mut NumberFormatter, number_options: &NumberFormattingOptions, address: u64, symbol: &SymbolResult,
		show_symbol_address: bool, write_minus_if_signed: bool, spaces_between_op: bool,
	) {
		let mut displ = address.wrapping_sub(symbol.address) as i64;
		if (symbol.flags & SymbolFlags::SIGNED) != 0 {
			if write_minus_if_signed {
				output.write("-", FormatterTextKind::Operator);
			}
			displ = -displ;
		}
		output.write_symbol(instruction, operand, instruction_operand, address, symbol);
		let mut number_kind: NumberKind;
		if displ != 0 {
			if spaces_between_op {
				output.write(" ", FormatterTextKind::Text);
			}
			let orig_displ = displ as u64;
			if displ < 0 {
				output.write("-", FormatterTextKind::Operator);
				displ = -displ;
				if displ <= i8::MAX as i64 + 1 {
					number_kind = NumberKind::Int8;
				} else if displ <= i16::MAX as i64 + 1 {
					number_kind = NumberKind::Int16;
				} else if displ <= i32::MAX as i64 + 1 {
					number_kind = NumberKind::Int32;
				} else {
					number_kind = NumberKind::Int64;
				}
			} else {
				output.write("+", FormatterTextKind::Operator);
				if displ <= i8::MAX as i64 {
					number_kind = NumberKind::Int8;
				} else if displ <= i16::MAX as i64 {
					number_kind = NumberKind::Int16;
				} else if displ <= i32::MAX as i64 {
					number_kind = NumberKind::Int32;
				} else {
					number_kind = NumberKind::Int64;
				}
			}
			if spaces_between_op {
				output.write(" ", FormatterTextKind::Text);
			}
			let s = number_formatter.format_u64_zeroes(options, number_options, displ as u64, false);
			output.write_number(instruction, operand, instruction_operand, s, orig_displ, number_kind, FormatterTextKind::Number);
		}
		if show_symbol_address {
			output.write(" ", FormatterTextKind::Text);
			output.write("(", FormatterTextKind::Punctuation);
			let s = if address <= u16::MAX as u64 {
				number_kind = NumberKind::UInt16;
				number_formatter.format_u16_zeroes(options, number_options, address as u16, true)
			} else if address <= u32::MAX as u64 {
				number_kind = NumberKind::UInt32;
				number_formatter.format_u32_zeroes(options, number_options, address as u32, true)
			} else {
				number_kind = NumberKind::UInt64;
				number_formatter.format_u64_zeroes(options, number_options, address, true)
			};
			output.write_number(instruction, operand, instruction_operand, s, address, number_kind, FormatterTextKind::Number);
			output.write(")", FormatterTextKind::Punctuation);
		}
	}
}

/// Formats instructions
///
/// This trait is sealed and cannot be implemented by your own types.
pub trait Formatter: private::Sealed {
	/// Formats the whole instruction: prefixes, mnemonic, operands
	///
	/// # Arguments
	///
	/// - `instruction`: Instruction
	/// - `output`: Output, eg. a `String`
	fn format(&mut self, instruction: &Instruction, output: &mut FormatterOutput);

	/// Gets the formatter options (immutable)
	#[cfg_attr(has_must_use, must_use)]
	fn options(&self) -> &FormatterOptions;

	/// Gets the formatter options (mutable)
	#[cfg_attr(has_must_use, must_use)]
	fn options_mut(&mut self) -> &mut FormatterOptions;

	/// Formats the mnemonic and any prefixes
	///
	/// # Arguments
	///
	/// - `instruction`: Instruction
	/// - `output`: Output, eg. a `String`
	#[inline]
	fn format_mnemonic(&mut self, instruction: &Instruction, output: &mut FormatterOutput) {
		self.format_mnemonic_options(instruction, output, FormatMnemonicOptions::NONE);
	}

	/// Formats the mnemonic and any prefixes
	///
	/// # Arguments
	///
	/// - `instruction`: Instruction
	/// - `output`: Output, eg. a `String`
	/// - `options`: Options, see [`FormatMnemonicOptions`]
	///
	/// [`FormatMnemonicOptions`]: struct.FormatMnemonicOptions.html
	fn format_mnemonic_options(&mut self, instruction: &Instruction, output: &mut FormatterOutput, options: u32);

	/// Gets the number of operands that will be formatted. A formatter can add and remove operands
	///
	/// # Arguments
	///
	/// - `instruction`: Instruction
	#[cfg_attr(has_must_use, must_use)]
	fn operand_count(&mut self, instruction: &Instruction) -> u32;

	/// Returns the operand access but only if it's an operand added by the formatter. If it's an
	/// operand that is part of [`Instruction`], you should call eg. [`InstructionInfoFactory::info()`].
	///
	/// # Panics
	///
	/// Panics if `operand` is invalid
	///
	/// # Arguments
	///
	/// - `instruction`: Instruction
	/// - `operand`: Operand number, 0-based. This is a formatter operand and isn't necessarily the same as an instruction operand. See [`operand_count()`]
	///
	/// [`Instruction`]: struct.Instruction.html
	/// [`InstructionInfoFactory::info()`]: struct.InstructionInfoFactory.html#method.info
	/// [`operand_count()`]: #tymethod.operand_count
	#[cfg(feature = "instr_info")]
	#[cfg_attr(has_must_use, must_use)]
	fn op_access(&mut self, instruction: &Instruction, operand: u32) -> Option<OpAccess>;

	/// Converts a formatter operand index to an instruction operand index. Returns `None` if it's an operand added by the formatter
	///
	/// # Panics
	///
	/// Panics if `operand` is invalid
	///
	/// # Arguments
	///
	/// - `instruction`: Instruction
	/// - `operand`: Operand number, 0-based. This is a formatter operand and isn't necessarily the same as an instruction operand. See [`operand_count()`]
	///
	/// [`operand_count()`]: #tymethod.operand_count
	#[cfg_attr(has_must_use, must_use)]
	fn get_instruction_operand(&mut self, instruction: &Instruction, operand: u32) -> Option<u32>;

	/// Converts an instruction operand index to a formatter operand index. Returns `None` if the instruction operand isn't used by the formatter
	///
	/// # Panics
	///
	/// Panics if `instruction_operand` is invalid
	///
	/// # Arguments
	///
	/// - `instruction`: Instruction
	/// - `instruction_operand`: Instruction operand
	#[cfg_attr(has_must_use, must_use)]
	fn get_formatter_operand(&mut self, instruction: &Instruction, instruction_operand: u32) -> Option<u32>;

	/// Formats an operand. This is a formatter operand and not necessarily a real instruction operand.
	/// A formatter can add and remove operands.
	///
	/// # Panics
	///
	/// Panics if `operand` is invalid
	///
	/// # Arguments
	///
	/// - `instruction`: Instruction
	/// - `output`: Output, eg. a `String`
	/// - `operand`: Operand number, 0-based. This is a formatter operand and isn't necessarily the same as an instruction operand. See [`operand_count()`]
	///
	/// [`operand_count()`]: #tymethod.operand_count
	fn format_operand(&mut self, instruction: &Instruction, output: &mut FormatterOutput, operand: u32);

	/// Formats an operand separator
	///
	/// # Arguments
	///
	/// - `instruction`: Instruction
	/// - `output`: Output, eg. a `String`
	fn format_operand_separator(&mut self, instruction: &Instruction, output: &mut FormatterOutput);

	/// Formats all operands
	///
	/// # Arguments
	///
	/// - `instruction`: Instruction
	/// - `output`: Output, eg. a `String`
	fn format_all_operands(&mut self, instruction: &Instruction, output: &mut FormatterOutput);

	/// Formats a register
	///
	/// # Arguments
	///
	/// - `register`: Register
	#[cfg_attr(has_must_use, must_use)]
	fn format_register(&mut self, register: Register) -> &str;

	/// Formats a `i8`
	///
	/// # Arguments
	///
	/// - `value`: Value
	#[cfg_attr(has_must_use, must_use)]
	fn format_i8(&mut self, value: i8) -> &str;

	/// Formats a `i16`
	///
	/// # Arguments
	///
	/// - `value`: Value
	#[cfg_attr(has_must_use, must_use)]
	fn format_i16(&mut self, value: i16) -> &str;

	/// Formats a `i32`
	///
	/// # Arguments
	///
	/// - `value`: Value
	#[cfg_attr(has_must_use, must_use)]
	fn format_i32(&mut self, value: i32) -> &str;

	/// Formats a `i64`
	///
	/// # Arguments
	///
	/// - `value`: Value
	#[cfg_attr(has_must_use, must_use)]
	fn format_i64(&mut self, value: i64) -> &str;

	/// Formats a `u8`
	///
	/// # Arguments
	///
	/// - `value`: Value
	#[cfg_attr(has_must_use, must_use)]
	fn format_u8(&mut self, value: u8) -> &str;

	/// Formats a `u16`
	///
	/// # Arguments
	///
	/// - `value`: Value
	#[cfg_attr(has_must_use, must_use)]
	fn format_u16(&mut self, value: u16) -> &str;

	/// Formats a `u32`
	///
	/// # Arguments
	///
	/// - `value`: Value
	#[cfg_attr(has_must_use, must_use)]
	fn format_u32(&mut self, value: u32) -> &str;

	/// Formats a `u64`
	///
	/// # Arguments
	///
	/// - `value`: Value
	#[cfg_attr(has_must_use, must_use)]
	fn format_u64(&mut self, value: u64) -> &str;

	/// Formats a `i8`
	///
	/// # Arguments
	///
	/// - `value`: Value
	/// - `number_options`: Options
	#[cfg_attr(has_must_use, must_use)]
	fn format_i8_options(&mut self, value: i8, number_options: &NumberFormattingOptions) -> &str;

	/// Formats a `i16`
	///
	/// # Arguments
	///
	/// - `value`: Value
	/// - `number_options`: Options
	#[cfg_attr(has_must_use, must_use)]
	fn format_i16_options(&mut self, value: i16, number_options: &NumberFormattingOptions) -> &str;

	/// Formats a `i32`
	///
	/// # Arguments
	///
	/// - `value`: Value
	/// - `number_options`: Options
	#[cfg_attr(has_must_use, must_use)]
	fn format_i32_options(&mut self, value: i32, number_options: &NumberFormattingOptions) -> &str;

	/// Formats a `i64`
	///
	/// # Arguments
	///
	/// - `value`: Value
	/// - `number_options`: Options
	#[cfg_attr(has_must_use, must_use)]
	fn format_i64_options(&mut self, value: i64, number_options: &NumberFormattingOptions) -> &str;

	/// Formats a `u8`
	///
	/// # Arguments
	///
	/// - `value`: Value
	/// - `number_options`: Options
	#[cfg_attr(has_must_use, must_use)]
	fn format_u8_options(&mut self, value: u8, number_options: &NumberFormattingOptions) -> &str;

	/// Formats a `u16`
	///
	/// # Arguments
	///
	/// - `value`: Value
	/// - `number_options`: Options
	#[cfg_attr(has_must_use, must_use)]
	fn format_u16_options(&mut self, value: u16, number_options: &NumberFormattingOptions) -> &str;

	/// Formats a `u32`
	///
	/// # Arguments
	///
	/// - `value`: Value
	/// - `number_options`: Options
	#[cfg_attr(has_must_use, must_use)]
	fn format_u32_options(&mut self, value: u32, number_options: &NumberFormattingOptions) -> &str;

	/// Formats a `u64`
	///
	/// # Arguments
	///
	/// - `value`: Value
	/// - `number_options`: Options
	#[cfg_attr(has_must_use, must_use)]
	fn format_u64_options(&mut self, value: u64, number_options: &NumberFormattingOptions) -> &str;
}

mod private {
	pub trait Sealed {}
	#[cfg(feature = "gas")]
	impl Sealed for super::gas::GasFormatter {}
	#[cfg(feature = "intel")]
	impl Sealed for super::intel::IntelFormatter {}
	#[cfg(feature = "masm")]
	impl Sealed for super::masm::MasmFormatter {}
	#[cfg(feature = "nasm")]
	impl Sealed for super::nasm::NasmFormatter {}
}

fn to_owned<'a>(sym_res: Option<SymbolResult>, vec: &'a mut Vec<SymResTextPart<'a>>) -> Option<SymbolResult<'a>> {
	match sym_res {
		None => None,
		Some(sym_res) => Some(sym_res.to_owned(vec)),
	}
}

fn get_mnemonic_cc<'a, 'b>(options: &'a FormatterOptions, cc_index: u32, mnemonics: &'b [FormatterString]) -> &'b FormatterString {
	let index = match cc_index {
		// o
		0 => {
			debug_assert_eq!(1, mnemonics.len());
			0
		}
		// no
		1 => {
			debug_assert_eq!(1, mnemonics.len());
			0
		}
		// b, c, nae
		2 => {
			debug_assert_eq!(3, mnemonics.len());
			options.cc_b() as usize
		}
		// ae, nb, nc
		3 => {
			debug_assert_eq!(3, mnemonics.len());
			options.cc_ae() as usize
		}
		// e, z
		4 => {
			debug_assert_eq!(2, mnemonics.len());
			options.cc_e() as usize
		}
		// ne, nz
		5 => {
			debug_assert_eq!(2, mnemonics.len());
			options.cc_ne() as usize
		}
		// be, na
		6 => {
			debug_assert_eq!(2, mnemonics.len());
			options.cc_be() as usize
		}
		// a, nbe
		7 => {
			debug_assert_eq!(2, mnemonics.len());
			options.cc_a() as usize
		}
		// s
		8 => {
			debug_assert_eq!(1, mnemonics.len());
			0
		}
		// ns
		9 => {
			debug_assert_eq!(1, mnemonics.len());
			0
		}
		// p, pe
		10 => {
			debug_assert_eq!(2, mnemonics.len());
			options.cc_p() as usize
		}
		// np, po
		11 => {
			debug_assert_eq!(2, mnemonics.len());
			options.cc_np() as usize
		}
		// l, nge
		12 => {
			debug_assert_eq!(2, mnemonics.len());
			options.cc_l() as usize
		}
		// ge, nl
		13 => {
			debug_assert_eq!(2, mnemonics.len());
			options.cc_ge() as usize
		}
		// le, ng
		14 => {
			debug_assert_eq!(2, mnemonics.len());
			options.cc_le() as usize
		}
		// g, nle
		15 => {
			debug_assert_eq!(2, mnemonics.len());
			options.cc_g() as usize
		}
		_ => unreachable!(),
	};
	debug_assert!(index < mnemonics.len());
	&mnemonics[index]
}
