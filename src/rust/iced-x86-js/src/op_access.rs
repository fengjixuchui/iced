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

use wasm_bindgen::prelude::*;

// GENERATOR-BEGIN: Enum
// ⚠️This was generated by GENERATOR!🦹‍♂️
/// Operand, register and memory access
#[wasm_bindgen]
#[derive(Copy, Clone)]
pub enum OpAccess {
	/// Nothing is read and nothing is written
	None = 0,
	/// The value is read
	Read = 1,
	/// The value is sometimes read and sometimes not
	CondRead = 2,
	/// The value is completely overwritten
	Write = 3,
	/// Conditional write, sometimes it's written and sometimes it's not modified
	CondWrite = 4,
	/// The value is read and written
	ReadWrite = 5,
	/// The value is read and sometimes written
	ReadCondWrite = 6,
	/// The memory operand doesn't refer to memory (eg. `LEA` instruction) or it's an instruction that doesn't read the data to a register or doesn't write to the memory location, it just prefetches/invalidates it, eg. `INVLPG`, `PREFETCHNTA`, `VGATHERPF0DPS`, etc. Some of those instructions still check if the code can access the memory location.
	NoMemAccess = 7,
}
// GENERATOR-END: Enum

#[allow(dead_code)]
pub(crate) fn iced_to_op_access(value: iced_x86_rust::OpAccess) -> OpAccess {
	// Safe, the enums are exactly identical
	unsafe { std::mem::transmute(value as u8) }
}
