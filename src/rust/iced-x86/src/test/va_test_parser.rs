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

use super::super::test_utils::from_str_conv::*;
use super::va_test_case::*;
#[cfg(not(feature = "std"))]
use alloc::string::String;
#[cfg(not(feature = "std"))]
use alloc::vec::Vec;
use core::iter::IntoIterator;
use core::u32;
use std::fs::File;
use std::io::prelude::*;
use std::io::{BufReader, Lines};
use std::path::Path;

pub(super) struct VirtualAddressTestParser {
	filename: String,
	lines: Lines<BufReader<File>>,
}

impl VirtualAddressTestParser {
	pub(super) fn new(filename: &Path) -> Self {
		let display_filename = filename.display().to_string();
		let file = File::open(filename).unwrap_or_else(|_| panic!("Couldn't open file {}", display_filename));
		let lines = BufReader::new(file).lines();
		Self { filename: display_filename, lines }
	}
}

impl IntoIterator for VirtualAddressTestParser {
	type Item = VirtualAddressTestCase;
	type IntoIter = IntoIter;

	fn into_iter(self) -> Self::IntoIter {
		IntoIter { filename: self.filename, lines: self.lines, line_number: 0 }
	}
}

pub(super) struct IntoIter {
	filename: String,
	lines: Lines<BufReader<File>>,
	line_number: u32,
}

impl Iterator for IntoIter {
	type Item = VirtualAddressTestCase;

	fn next(&mut self) -> Option<Self::Item> {
		loop {
			match self.lines.next() {
				None => return None,
				Some(info) => {
					let result = match info {
						Ok(line) => {
							self.line_number += 1;
							if line.is_empty() || line.starts_with('#') {
								continue;
							}
							IntoIter::read_next_test_case(line)
						}
						Err(err) => Err(err.to_string()),
					};
					match result {
						Ok(tc) => {
							if let Some(tc) = tc {
								return Some(tc);
							} else {
								continue;
							}
						}
						Err(err) => panic!("Error parsing virtual address test case file '{}', line {}: {}", self.filename, self.line_number, err),
					}
				}
			}
		}
	}
}

impl IntoIter {
	fn read_next_test_case(line: String) -> Result<Option<VirtualAddressTestCase>, String> {
		let elems: Vec<_> = line.split(',').collect();
		if elems.len() != 7 {
			return Err(format!("Invalid number of commas: {}", elems.len() - 1));
		}

		let bitness = to_u32(elems[0])?;
		if is_ignored_code(elems[1].trim()) {
			return Ok(None);
		}
		let hex_bytes = String::from(elems[2].trim());
		let _ = to_vec_u8(&hex_bytes)?;
		let operand = to_u32(elems[3])?;
		let element_index = to_u32(elems[4])? as usize;
		let expected_value = to_u64(elems[5])?;

		let mut register_values: Vec<VARegisterValue> = Vec::new();
		for tmp in elems[6].split_whitespace() {
			if tmp.is_empty() {
				continue;
			}
			let kv: Vec<_> = tmp.split('=').collect();
			if kv.len() != 2 {
				return Err(format!("Expected key=value: {}", tmp));
			}
			let key = kv[0];
			let value_str = kv[1];

			let (register, expected_element_index, expected_element_size) = if key.contains(';') {
				let parts: Vec<_> = key.split(';').collect();
				if parts.len() != 3 {
					return Err(format!("Invalid number of semicolons: {}", parts.len() - 1));
				}
				(to_register(parts[0])?, to_u32(parts[1])? as usize, to_u32(parts[2])? as usize)
			} else {
				(to_register(key)?, 0, 0)
			};
			let value = to_u64(value_str)?;
			register_values.push(VARegisterValue { register, element_index: expected_element_index, element_size: expected_element_size, value });
		}

		Ok(Some(VirtualAddressTestCase { bitness, hex_bytes, operand, element_index, expected_value, register_values }))
	}
}
