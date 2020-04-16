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

use super::block_encoder_options::BlockEncoderOptions;
use super::instruction::Instruction;
use iced_x86_rust::InstructionBlock;
use wasm_bindgen::prelude::*;

/// Encodes instructions. It can be used to move instructions from one location to another location.
#[wasm_bindgen]
pub struct BlockEncoder {
	instructions: Vec<iced_x86_rust::Instruction>,
	bitness: u32,
	options: u32,
}

#[wasm_bindgen]
impl BlockEncoder {
	/// Constructor
	///
	/// # Throws
	///
	/// Throws if `bitness` is not one of 16, 32, 64.
	///
	/// # Arguments
	///
	/// * `bitness`: 16, 32, or 64
	/// * `options`: Encoder options ([`BlockEncoderOptions`])
	///
	/// [`BlockEncoderOptions`]: enum.BlockEncoderOptions.html
	#[wasm_bindgen(constructor)]
	pub fn new(bitness: u32, options: u32 /*flags: BlockEncoderOptions*/) -> Self {
		// It's not part of the method sig so make sure it's still compiled by referencing it here
		const_assert_eq!(0, BlockEncoderOptions::None as u32);
		if bitness != 16 && bitness != 32 && bitness != 64 {
			panic!();
		}
		BlockEncoder { instructions: Vec::new(), bitness, options }
	}

	/// Adds an instruction that will be encoded when [`encode()`] is called.
	/// The input `instruction` can be a decoded instruction or an instruction
	/// created by the user, eg. `Instruction.with*()` constructor methods.
	///
	/// [`encode()`]: #method.encode
	///
	/// # Arguments
	///
	/// * `instruction`: Next instruction to encode
	pub fn add(&mut self, instruction: &Instruction) {
		self.instructions.push(instruction.0);
	}

	/// Encodes all instructions added by [`add()`] and returns the encoded bytes.
	///
	/// Enable the `bigint` feature to use APIs with 64-bit numbers (requires `BigInt`).
	///
	/// [`add()`]: #method.add
	///
	/// # Arguments
	///
	/// * `ripHi`: High 32 bits of the base IP of all encoded instructions
	/// * `ripLo`: Low 32 bits of the base IP of all encoded instructions
	#[cfg(not(feature = "bigint"))]
	pub fn encode(&mut self, #[allow(non_snake_case)] ripHi: u32, #[allow(non_snake_case)] ripLo: u32) -> Result<Vec<u8>, JsValue> {
		let rip = ((ripHi as u64) << 32) | (ripLo as u64);
		self.encode_core(rip)
	}

	/// Encodes all instructions added by [`add()`] and returns the encoded bytes
	///
	/// [`add()`]: #method.add
	///
	/// # Arguments
	///
	/// * `rip`: Base IP of all encoded instructions
	#[cfg(feature = "bigint")]
	pub fn encode(&mut self, rip: u64) -> Result<Vec<u8>, JsValue> {
		self.encode_core(rip)
	}

	fn encode_core(&mut self, rip: u64) -> Result<Vec<u8>, JsValue> {
		let block = InstructionBlock::new(&self.instructions, rip);
		match iced_x86_rust::BlockEncoder::encode(self.bitness, block, self.options) {
			Ok(result) => Ok(result.code_buffer),
			Err(error) => Err(js_sys::Error::new(&error).into()),
		}
	}
}
