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

use super::super::super::data_reader::DataReader;
use super::super::super::iced_constants::IcedConstants;
use super::super::pseudo_ops::get_pseudo_ops;
use super::super::strings_tbl::get_strings_table;
use super::enums::*;
use super::fmt_data::FORMATTER_TBL_DATA;
use super::info::*;
#[cfg(not(feature = "std"))]
use alloc::boxed::Box;
#[cfg(not(feature = "std"))]
use alloc::string::String;
#[cfg(not(feature = "std"))]
use alloc::vec::Vec;
use core::mem;

lazy_static! {
	pub(super) static ref ALL_INFOS: Vec<Box<InstrInfo + Sync + Send>> = { read() };
}

fn add_suffix(s: &str, c: char) -> String {
	if c == '\0' {
		String::from(s)
	} else {
		let mut res = String::with_capacity(s.len() + 1);
		res.push_str(s);
		res.push(c);
		res
	}
}

fn add_prefix(s: &str, c: char) -> String {
	let mut res = String::with_capacity(s.len() + 1);
	res.push(c);
	res.push_str(s);
	res
}

fn read() -> Vec<Box<InstrInfo + Sync + Send>> {
	let mut infos: Vec<Box<InstrInfo + Sync + Send>> = Vec::with_capacity(IcedConstants::NUMBER_OF_CODE_VALUES);
	let mut reader = DataReader::new(FORMATTER_TBL_DATA);
	let strings = get_strings_table();
	let mut prev_index = -1isize;
	for i in 0..IcedConstants::NUMBER_OF_CODE_VALUES {
		let f = reader.read_u8();
		let mut ctor_kind: CtorKind = unsafe { mem::transmute((f & 0x7F) as u8) };
		let current_index;
		if ctor_kind == CtorKind::Previous {
			current_index = reader.index() as isize;
			reader.set_index(prev_index as usize);
			ctor_kind = unsafe { mem::transmute((reader.read_u8() & 0x7F) as u8) };
		} else {
			current_index = -1;
			prev_index = reader.index() as isize - 1;
		}
		let mut s = if (f & 0x80) != 0 {
			add_prefix(&strings[reader.read_compressed_u32() as usize], 'v')
		} else {
			strings[reader.read_compressed_u32() as usize].clone()
		};

		let mut c;
		let v;
		let v2;
		let info: Box<InstrInfo + Sync + Send> = match ctor_kind {
			CtorKind::Previous => unreachable!(),
			CtorKind::Normal_1 => Box::new(SimpleInstrInfo::with_mnemonic(s)),

			CtorKind::Normal_2a => {
				c = reader.read_u8() as u8 as char;
				let s2 = add_suffix(&s, c);
				Box::new(SimpleInstrInfo::with_mnemonic_suffix(s, s2))
			}

			CtorKind::Normal_2b => {
				v = reader.read_compressed_u32();
				Box::new(SimpleInstrInfo::with_mnemonic_flags(s, v))
			}

			CtorKind::Normal_2c => {
				c = reader.read_u8() as u8 as char;
				s = add_suffix(&s, c);
				Box::new(SimpleInstrInfo::with_mnemonic(s))
			}

			CtorKind::Normal_3 => {
				c = reader.read_u8() as u8 as char;
				let s2 = add_suffix(&s, c);
				v = reader.read_compressed_u32();
				Box::new(SimpleInstrInfo::new(s, s2, v))
			}

			CtorKind::AamAad => Box::new(SimpleInstrInfo_AamAad::new(s)),

			CtorKind::asz => {
				v = reader.read_compressed_u32();
				Box::new(SimpleInstrInfo_as::new(v, s))
			}

			CtorKind::bnd2_2 => {
				c = reader.read_u8() as u8 as char;
				let s2 = add_suffix(&s, c);
				Box::new(SimpleInstrInfo_bnd2::with_mnemonic(s, s2))
			}

			CtorKind::bnd2_3 => {
				c = reader.read_u8() as u8 as char;
				let s2 = add_suffix(&s, c);
				v = reader.read_compressed_u32();
				Box::new(SimpleInstrInfo_bnd2::new(s, s2, v))
			}

			CtorKind::DeclareData => Box::new(SimpleInstrInfo_DeclareData::new(unsafe { mem::transmute(i as u16) }, s)),

			CtorKind::er_2 => {
				v = reader.read_compressed_u32();
				Box::new(SimpleInstrInfo_er::with_mnemonic(v, s))
			}

			CtorKind::er_4 => {
				c = reader.read_u8() as u8 as char;
				let s2 = add_suffix(&s, c);
				v = reader.read_compressed_u32();
				v2 = reader.read_compressed_u32();
				Box::new(SimpleInstrInfo_er::new(v, s, s2, v2))
			}

			CtorKind::far => {
				c = reader.read_u8() as u8 as char;
				let s2 = add_suffix(&s, c);
				v = reader.read_compressed_u32();
				Box::new(SimpleInstrInfo_far::new(v, s, s2))
			}

			CtorKind::imul => {
				c = reader.read_u8() as u8 as char;
				let s2 = add_suffix(&s, c);
				Box::new(SimpleInstrInfo_imul::new(s, s2))
			}

			CtorKind::maskmovq => Box::new(SimpleInstrInfo_maskmovq::new(s)),

			CtorKind::movabs => {
				c = reader.read_u8() as u8 as char;
				let s2 = add_suffix(&s, c);
				v = reader.read_compressed_u32();
				let s3 = strings[reader.read_compressed_u32() as usize].clone();
				c = reader.read_u8() as u8 as char;
				let s4 = add_suffix(&s3, c);
				Box::new(SimpleInstrInfo_movabs::new(v, s, s2, s3, s4))
			}

			CtorKind::nop => {
				v = reader.read_compressed_u32();
				v2 = reader.read_u8() as u32;
				Box::new(SimpleInstrInfo_nop::new(v, s, unsafe { mem::transmute(v2 as u8) }))
			}

			CtorKind::OpSize => {
				v = reader.read_u8() as u32;
				let s2 = add_suffix(&s, 'w');
				let s3 = add_suffix(&s, 'l');
				let s4 = add_suffix(&s, 'q');
				Box::new(SimpleInstrInfo_OpSize::new(unsafe { mem::transmute(v as u8) }, s, s2, s3, s4))
			}

			CtorKind::OpSize2_bnd => {
				let s2 = strings[reader.read_compressed_u32() as usize].clone();
				let s3 = strings[reader.read_compressed_u32() as usize].clone();
				let s4 = strings[reader.read_compressed_u32() as usize].clone();
				Box::new(SimpleInstrInfo_OpSize2_bnd::new(s, s2, s3, s4))
			}

			CtorKind::OpSize3 => {
				c = reader.read_u8() as u8 as char;
				let s2 = add_suffix(&s, c);
				v = reader.read_compressed_u32();
				Box::new(SimpleInstrInfo_OpSize3::new(v, s, s2))
			}

			CtorKind::os_A => {
				v = reader.read_compressed_u32();
				Box::new(SimpleInstrInfo_os::with_mnemonic(v, s))
			}

			CtorKind::os_B => {
				v = reader.read_compressed_u32();
				v2 = reader.read_compressed_u32();
				Box::new(SimpleInstrInfo_os::new(v, s, v2))
			}

			CtorKind::os_bnd => {
				v = reader.read_compressed_u32();
				Box::new(SimpleInstrInfo_os_bnd::new(v, s))
			}

			CtorKind::os_jcc => {
				v = reader.read_compressed_u32();
				Box::new(SimpleInstrInfo_os_jcc::new(v, s))
			}

			CtorKind::os_loop => {
				c = reader.read_u8() as u8 as char;
				let s2 = add_suffix(&s, c);
				v = reader.read_compressed_u32();
				v2 = reader.read_compressed_u32();
				Box::new(SimpleInstrInfo_os_loop::new(v, v2, s, s2))
			}

			CtorKind::os_mem => {
				c = reader.read_u8() as u8 as char;
				let s2 = add_suffix(&s, c);
				v = reader.read_compressed_u32();
				Box::new(SimpleInstrInfo_os_mem::new(v, s, s2))
			}

			CtorKind::os_mem_reg16 => {
				v = reader.read_compressed_u32();
				Box::new(SimpleInstrInfo_os_mem_reg16::new(v, s))
			}

			CtorKind::os_mem2 => {
				c = reader.read_u8() as u8 as char;
				let s2 = add_suffix(&s, c);
				v = reader.read_compressed_u32();
				Box::new(SimpleInstrInfo_os_mem2::new(v, s, s2))
			}

			CtorKind::os2_3 => {
				c = reader.read_u8() as u8 as char;
				let s2 = add_suffix(&s, c);
				v = reader.read_compressed_u32();
				Box::new(SimpleInstrInfo_os2::with_mnemonic(v, s, s2))
			}

			CtorKind::os2_4 => {
				c = reader.read_u8() as u8 as char;
				let s2 = add_suffix(&s, c);
				v = reader.read_compressed_u32();
				v2 = reader.read_compressed_u32();
				Box::new(SimpleInstrInfo_os2::new(v, s, s2, v2))
			}

			CtorKind::os2_bnd => {
				c = reader.read_u8() as u8 as char;
				let s2 = add_suffix(&s, c);
				v = reader.read_compressed_u32();
				Box::new(SimpleInstrInfo_os2_bnd::new(v, s, s2))
			}

			CtorKind::pblendvb => Box::new(SimpleInstrInfo_pblendvb::new(s)),

			CtorKind::pclmulqdq => {
				v = reader.read_u8() as u32;
				Box::new(SimpleInstrInfo_pclmulqdq::new(s, get_pseudo_ops(unsafe { mem::transmute(v as u8) })))
			}

			CtorKind::pops => {
				v = reader.read_u8() as u32;
				Box::new(SimpleInstrInfo_pops::new(s, get_pseudo_ops(unsafe { mem::transmute(v as u8) })))
			}

			CtorKind::Reg16 => Box::new(SimpleInstrInfo_Reg16::new(s)),

			CtorKind::sae => {
				v = reader.read_compressed_u32();
				Box::new(SimpleInstrInfo_sae::new(v, s))
			}

			CtorKind::sae_pops => {
				v = reader.read_compressed_u32();
				v2 = reader.read_u8() as u32;
				Box::new(SimpleInstrInfo_sae_pops::new(v, s, get_pseudo_ops(unsafe { mem::transmute(v2 as u8) })))
			}

			CtorKind::ST_STi => Box::new(SimpleInstrInfo_ST_STi::new(s)),
			CtorKind::STi_ST => Box::new(SimpleInstrInfo_STi_ST::new(s)),
			CtorKind::STi_ST2 => Box::new(SimpleInstrInfo_STi_ST2::new(s)),
			CtorKind::STIG_1a => Box::new(SimpleInstrInfo_STIG1::with_mnemonic(s)),

			CtorKind::STIG_1b => {
				v = reader.read_u8() as u32;
				if v > 1 {
					panic!();
				}
				Box::new(SimpleInstrInfo_STIG1::new(s, v != 0))
			}

			CtorKind::xbegin => {
				v = reader.read_compressed_u32();
				Box::new(SimpleInstrInfo_xbegin::new(v, s))
			}
		};

		infos.push(info);
		if current_index >= 0 {
			reader.set_index(current_index as usize);
		}
	}
	if reader.can_read() {
		panic!();
	}
	infos
}
