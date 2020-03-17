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

use super::super::super::test_utils::from_str_conv::to_code;
use super::super::super::test_utils::*;
use super::super::super::*;
use super::decoder_mem_test_case::*;
use super::decoder_test_case::*;
use super::test_cases::*;
#[cfg(not(feature = "std"))]
use alloc::string::String;
#[cfg(not(feature = "std"))]
use alloc::vec::Vec;
#[cfg(not(feature = "std"))]
use hashbrown::HashSet;
#[cfg(feature = "std")]
use std::collections::HashSet;
use std::fs::File;
use std::io::prelude::*;
use std::io::BufReader;

pub(crate) struct DecoderTestInfo {
	bitness: u32,
	code: Code,
	hex_bytes: String,
	#[allow(dead_code)]
	encoded_hex_bytes: String,
	decoder_options: u32,
}

impl DecoderTestInfo {
	pub(crate) fn bitness(&self) -> u32 {
		self.bitness
	}
	pub(crate) fn code(&self) -> Code {
		self.code
	}
	pub(crate) fn hex_bytes(&self) -> &str {
		&self.hex_bytes
	}
	#[cfg(feature = "encoder")]
	pub(crate) fn encoded_hex_bytes(&self) -> &str {
		&self.encoded_hex_bytes
	}
	pub(crate) fn decoder_options(&self) -> u32 {
		self.decoder_options
	}
}

lazy_static! {
	static ref NOT_DECODED: HashSet<Code> = { read_code_values("Code.NotDecoded.txt") };
}
lazy_static! {
	static ref NOT_DECODED32_ONLY: HashSet<Code> = { read_code_values("Code.NotDecoded32Only.txt") };
}
lazy_static! {
	static ref NOT_DECODED64_ONLY: HashSet<Code> = { read_code_values("Code.NotDecoded64Only.txt") };
}
lazy_static! {
	static ref CODE32_ONLY: HashSet<Code> = { read_code_values("Code.32Only.txt") };
}
lazy_static! {
	static ref CODE64_ONLY: HashSet<Code> = { read_code_values("Code.64Only.txt") };
}

fn read_code_values(name: &str) -> HashSet<Code> {
	let mut filename = get_decoder_unit_tests_dir();
	filename.push(name);
	let display_filename = filename.display();
	let file = File::open(filename.as_path()).unwrap_or_else(|_| panic!("Couldn't open file {}", display_filename));
	let mut h = HashSet::new();
	for (info, line_number) in BufReader::new(file).lines().zip(1..) {
		let err = match info {
			Ok(line) => {
				if line.is_empty() || line.starts_with('#') {
					None
				} else {
					match to_code(&line) {
						Ok(code) => {
							let _ = h.insert(code);
							None
						}
						Err(err) => Some(err),
					}
				}
			}
			Err(err) => Some(err.to_string()),
		};
		if let Some(err) = err {
			panic!("Error parsing Code file '{}', line {}: {}", display_filename, line_number, err);
		}
	}
	h
}

pub(crate) fn not_decoded() -> &'static HashSet<Code> {
	&*NOT_DECODED
}

pub(crate) fn not_decoded32_only() -> &'static HashSet<Code> {
	&*NOT_DECODED32_ONLY
}

pub(crate) fn not_decoded64_only() -> &'static HashSet<Code> {
	&*NOT_DECODED64_ONLY
}

pub(crate) fn code32_only() -> &'static HashSet<Code> {
	&*CODE32_ONLY
}

pub(crate) fn code64_only() -> &'static HashSet<Code> {
	&*CODE64_ONLY
}

#[cfg(feature = "encoder")]
pub(crate) fn encoder_tests(include_other_tests: bool, include_invalid: bool) -> Vec<DecoderTestInfo> {
	get_tests(include_other_tests, include_invalid, Some(true))
}

pub(crate) fn decoder_tests(include_other_tests: bool, include_invalid: bool) -> Vec<DecoderTestInfo> {
	get_tests(include_other_tests, include_invalid, None)
}

fn get_tests(include_other_tests: bool, include_invalid: bool, can_encode: Option<bool>) -> Vec<DecoderTestInfo> {
	let mut v: Vec<DecoderTestInfo> = Vec::new();
	let bitness_array = [16, 32, 64];
	for bitness in &bitness_array {
		add_tests(&mut v, get_test_cases(*bitness), include_invalid, can_encode);
	}
	if include_other_tests {
		for bitness in &bitness_array {
			add_tests(&mut v, get_misc_test_cases(*bitness), include_invalid, can_encode);
		}
		for bitness in &bitness_array {
			add_tests_mem(&mut v, get_mem_test_cases(*bitness), include_invalid, can_encode);
		}
	}
	v
}

fn add_tests(v: &mut Vec<DecoderTestInfo>, tests: &[DecoderTestCase], include_invalid: bool, can_encode: Option<bool>) {
	for tc in tests {
		if !include_invalid && tc.code == Code::INVALID {
			continue;
		}
		if let Some(can_encode) = can_encode {
			if tc.can_encode != can_encode {
				continue;
			}
		}
		v.push(DecoderTestInfo {
			bitness: tc.bitness,
			code: tc.code,
			hex_bytes: tc.hex_bytes.clone(),
			encoded_hex_bytes: tc.encoded_hex_bytes.clone(),
			decoder_options: tc.decoder_options,
		});
	}
}

fn add_tests_mem(v: &mut Vec<DecoderTestInfo>, tests: &[DecoderMemoryTestCase], include_invalid: bool, can_encode: Option<bool>) {
	for tc in tests {
		if !include_invalid && tc.code == Code::INVALID {
			continue;
		}
		if let Some(can_encode) = can_encode {
			if tc.can_encode != can_encode {
				continue;
			}
		}
		v.push(DecoderTestInfo {
			bitness: tc.bitness,
			code: tc.code,
			hex_bytes: tc.hex_bytes.clone(),
			encoded_hex_bytes: tc.encoded_hex_bytes.clone(),
			decoder_options: tc.decoder_options,
		});
	}
}
