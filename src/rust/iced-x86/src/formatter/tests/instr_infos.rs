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

use super::super::super::code::Code;
use super::super::test_utils::from_str_conv::{to_code, to_decoder_options};
use super::super::test_utils::get_formatter_unit_tests_dir;
#[cfg(not(feature = "std"))]
use alloc::string::String;
#[cfg(not(feature = "std"))]
use alloc::vec::Vec;
use std::fs::File;
use std::io::prelude::*;
use std::io::BufReader;

pub(super) struct InstructionInfo {
	pub(super) bitness: u32,
	pub(super) hex_bytes: String,
	pub(super) code: Code,
	pub(super) options: u32,
}

lazy_static! {
	static ref INFOS_16: Vec<InstructionInfo> = { read_infos(16, false) };
}
lazy_static! {
	static ref INFOS_32: Vec<InstructionInfo> = { read_infos(32, false) };
}
lazy_static! {
	static ref INFOS_64: Vec<InstructionInfo> = { read_infos(64, false) };
}

lazy_static! {
	static ref INFOS_MISC_16: Vec<InstructionInfo> = { read_infos(16, true) };
}
lazy_static! {
	static ref INFOS_MISC_32: Vec<InstructionInfo> = { read_infos(32, true) };
}
lazy_static! {
	static ref INFOS_MISC_64: Vec<InstructionInfo> = { read_infos(64, true) };
}

pub(super) fn get_infos(bitness: u32, is_misc: bool) -> &'static Vec<InstructionInfo> {
	if is_misc {
		match bitness {
			16 => &*INFOS_MISC_16,
			32 => &*INFOS_MISC_32,
			64 => &*INFOS_MISC_64,
			_ => unreachable!(),
		}
	} else {
		match bitness {
			16 => &*INFOS_16,
			32 => &*INFOS_32,
			64 => &*INFOS_64,
			_ => unreachable!(),
		}
	}
}

fn read_infos(bitness: u32, is_misc: bool) -> Vec<InstructionInfo> {
	let mut filename = get_formatter_unit_tests_dir();
	if is_misc {
		filename.push(format!("InstructionInfos{}_Misc.txt", bitness));
	} else {
		filename.push(format!("InstructionInfos{}.txt", bitness));
	}

	let display_filename = filename.display().to_string();
	let file = File::open(filename).unwrap_or_else(|_| panic!("Couldn't open file {}", display_filename));
	let mut infos: Vec<InstructionInfo> = Vec::new();
	let mut line_number = 0;
	for info in BufReader::new(file).lines() {
		let result = match info {
			Ok(line) => {
				line_number += 1;
				if line.is_empty() || line.starts_with('#') {
					continue;
				}
				read_next_info(bitness, line)
			}
			Err(err) => Err(err.to_string()),
		};
		match result {
			Ok(tc) => infos.push(tc),
			Err(err) => panic!("Error parsing formatter test case file '{}', line {}: {}", display_filename, line_number, err),
		}
	}
	infos
}

fn read_next_info(bitness: u32, line: String) -> Result<InstructionInfo, String> {
	let parts: Vec<_> = line.split(',').collect();
	let options = match parts.len() {
		2 => 0,
		3 => to_decoder_options(parts[2])?,
		_ => return Err(String::from("Invalid number of commas")),
	};
	let hex_bytes = parts[0].trim();
	let code = to_code(parts[1].trim())?;
	Ok(InstructionInfo { bitness, hex_bytes: String::from(hex_bytes), code, options })
}

pub(super) fn get_formatted_lines(bitness: u32, dir: &str, file_part: &str) -> Vec<String> {
	let mut filename = get_formatter_unit_tests_dir();
	filename.push(dir);
	filename.push(format!("Test{}_{}.txt", bitness, file_part));
	super::get_lines_ignore_comments(filename.as_path())
}
