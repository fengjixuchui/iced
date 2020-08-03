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

// GENERATOR-BEGIN: MiscConstants
// ⚠️This was generated by GENERATOR!🦹‍♂️
pub(crate) struct MiscInstrInfoTestConstants;
#[allow(dead_code)]
impl MiscInstrInfoTestConstants {
	pub(crate) const VMM_PREFIX: &'static str = "vmm";
	pub(crate) const XSP: &'static str = "xsp";
	pub(crate) const XBP: &'static str = "xbp";
	pub(crate) const INSTR_INFO_ELEMS_PER_LINE: usize = 5;
	pub(crate) const MEMORY_SIZE_ELEMS_PER_LINE: usize = 6;
	pub(crate) const REGISTER_ELEMS_PER_LINE: usize = 7;
}
// GENERATOR-END: MiscConstants

// GENERATOR-BEGIN: MemorySizeFlags
// ⚠️This was generated by GENERATOR!🦹‍♂️
pub(crate) struct MemorySizeFlags;
#[allow(dead_code)]
impl MemorySizeFlags {
	pub(crate) const NONE: u32 = 0x0000_0000;
	pub(crate) const SIGNED: u32 = 0x0000_0001;
	pub(crate) const BROADCAST: u32 = 0x0000_0002;
	pub(crate) const PACKED: u32 = 0x0000_0004;
}
// GENERATOR-END: MemorySizeFlags

// GENERATOR-BEGIN: RegisterFlags
// ⚠️This was generated by GENERATOR!🦹‍♂️
pub(crate) struct RegisterFlags;
#[allow(dead_code)]
impl RegisterFlags {
	pub(crate) const NONE: u32 = 0x0000_0000;
	pub(crate) const SEGMENT_REGISTER: u32 = 0x0000_0001;
	pub(crate) const GPR: u32 = 0x0000_0002;
	pub(crate) const GPR8: u32 = 0x0000_0004;
	pub(crate) const GPR16: u32 = 0x0000_0008;
	pub(crate) const GPR32: u32 = 0x0000_0010;
	pub(crate) const GPR64: u32 = 0x0000_0020;
	pub(crate) const XMM: u32 = 0x0000_0040;
	pub(crate) const YMM: u32 = 0x0000_0080;
	pub(crate) const ZMM: u32 = 0x0000_0100;
	pub(crate) const VECTOR_REGISTER: u32 = 0x0000_0200;
	pub(crate) const IP: u32 = 0x0000_0400;
	pub(crate) const K: u32 = 0x0000_0800;
	pub(crate) const BND: u32 = 0x0000_1000;
	pub(crate) const CR: u32 = 0x0000_2000;
	pub(crate) const DR: u32 = 0x0000_4000;
	pub(crate) const TR: u32 = 0x0000_8000;
	pub(crate) const ST: u32 = 0x0001_0000;
	pub(crate) const MM: u32 = 0x0002_0000;
	pub(crate) const TMM: u32 = 0x0004_0000;
}
// GENERATOR-END: RegisterFlags
