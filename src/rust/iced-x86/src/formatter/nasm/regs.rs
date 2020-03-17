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

use super::super::super::Register;
use super::super::regs_tbl::{MAX_STRING_LENGTH, REGS_TBL};
use super::super::FormatterString;
#[cfg(not(feature = "std"))]
use alloc::string::String;
#[cfg(not(feature = "std"))]
use alloc::vec::Vec;
use core::fmt::Write;

pub(super) struct Registers;
impl Registers {
	pub(super) const EXTRA_REGISTERS: u32 = 0;
}

lazy_static! {
	pub(super) static ref ALL_REGISTERS: Vec<FormatterString> = {
		let mut v: Vec<_> = (&*REGS_TBL).to_vec();
		let mut s = String::with_capacity(MAX_STRING_LENGTH);
		for i in 0..8usize {
			write!(s, "st{}", i).unwrap();
			v[Register::ST0 as usize + i] = FormatterString::new(s.clone());
			s.clear();
		}
		v
	};
}
