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

use super::FormatterString;
#[cfg(not(feature = "std"))]
#[cfg(any(feature = "intel", feature = "nasm"))]
use alloc::vec::Vec;

#[allow(dead_code)]
pub(super) struct FormatterConstants {
	pub(super) empty: FormatterString,
	pub(super) b1to2: FormatterString,
	pub(super) b1to4: FormatterString,
	pub(super) b1to8: FormatterString,
	pub(super) b1to16: FormatterString,
	pub(super) ptr: FormatterString,
	pub(super) bcst: FormatterString,
	pub(super) byte: FormatterString,
	pub(super) word: FormatterString,
	pub(super) dword: FormatterString,
	pub(super) qword: FormatterString,
	pub(super) mmword: FormatterString,
	pub(super) xmmword: FormatterString,
	pub(super) ymmword: FormatterString,
	pub(super) zmmword: FormatterString,
	pub(super) oword: FormatterString,
	pub(super) yword: FormatterString,
	pub(super) zword: FormatterString,
	pub(super) fword: FormatterString,
	pub(super) tword: FormatterString,
	pub(super) tbyte: FormatterString,
	pub(super) fpuenv14: FormatterString,
	pub(super) fpuenv28: FormatterString,
	pub(super) fpustate108: FormatterString,
	pub(super) fpustate94: FormatterString,
	pub(super) far: FormatterString,
	pub(super) bnd: FormatterString,
	pub(super) dot_byte: FormatterString,
	pub(super) hint_not_taken: FormatterString,
	pub(super) hint_taken: FormatterString,
	pub(super) hnt: FormatterString,
	pub(super) ht: FormatterString,
	pub(super) lock: FormatterString,
	pub(super) near: FormatterString,
	pub(super) notrack: FormatterString,
	pub(super) offset: FormatterString,
	pub(super) pn: FormatterString,
	pub(super) pt: FormatterString,
	pub(super) rd_sae: FormatterString,
	pub(super) rel: FormatterString,
	pub(super) rep: FormatterString,
	pub(super) repe: [FormatterString; 2],
	pub(super) repne: [FormatterString; 2],
	pub(super) rex_w: FormatterString,
	pub(super) rne_sae: FormatterString,
	pub(super) rn_sae: FormatterString,
	pub(super) ru_sae: FormatterString,
	pub(super) rz_sae: FormatterString,
	pub(super) sae: FormatterString,
	pub(super) short: FormatterString,
	pub(super) to: FormatterString,
	pub(super) xacquire: FormatterString,
	pub(super) xrelease: FormatterString,
	pub(super) z: FormatterString,
	pub(super) a16: FormatterString,
	pub(super) a32: FormatterString,
	pub(super) a64: FormatterString,
	pub(super) addr16: FormatterString,
	pub(super) addr32: FormatterString,
	pub(super) addr64: FormatterString,
	pub(super) data16: FormatterString,
	pub(super) data32: FormatterString,
	pub(super) data64: FormatterString,
	pub(super) o16: FormatterString,
	pub(super) o32: FormatterString,
	pub(super) o64: FormatterString,
}

lazy_static! {
	pub(super) static ref FORMATTER_CONSTANTS: FormatterConstants = {
		FormatterConstants {
			empty: FormatterString::new_str(""),
			b1to2: FormatterString::new_str("1to2"),
			b1to4: FormatterString::new_str("1to4"),
			b1to8: FormatterString::new_str("1to8"),
			b1to16: FormatterString::new_str("1to16"),
			ptr: FormatterString::new_str("ptr"),
			bcst: FormatterString::new_str("bcst"),
			byte: FormatterString::new_str("byte"),
			word: FormatterString::new_str("word"),
			dword: FormatterString::new_str("dword"),
			qword: FormatterString::new_str("qword"),
			mmword: FormatterString::new_str("mmword"),
			xmmword: FormatterString::new_str("xmmword"),
			ymmword: FormatterString::new_str("ymmword"),
			zmmword: FormatterString::new_str("zmmword"),
			oword: FormatterString::new_str("oword"),
			yword: FormatterString::new_str("yword"),
			zword: FormatterString::new_str("zword"),
			fword: FormatterString::new_str("fword"),
			tword: FormatterString::new_str("tword"),
			tbyte: FormatterString::new_str("tbyte"),
			fpuenv14: FormatterString::new_str("fpuenv14"),
			fpuenv28: FormatterString::new_str("fpuenv28"),
			fpustate108: FormatterString::new_str("fpustate108"),
			fpustate94: FormatterString::new_str("fpustate94"),
			far: FormatterString::new_str("far"),
			bnd: FormatterString::new_str("bnd"),
			dot_byte: FormatterString::new_str(".byte"),
			hint_not_taken: FormatterString::new_str("hint-not-taken"),
			hint_taken: FormatterString::new_str("hint-taken"),
			hnt: FormatterString::new_str("hnt"),
			ht: FormatterString::new_str("ht"),
			lock: FormatterString::new_str("lock"),
			near: FormatterString::new_str("near"),
			notrack: FormatterString::new_str("notrack"),
			offset: FormatterString::new_str("offset"),
			pn: FormatterString::new_str("pn"),
			pt: FormatterString::new_str("pt"),
			rd_sae: FormatterString::new_str("rd-sae"),
			rel: FormatterString::new_str("rel"),
			rep: FormatterString::new_str("rep"),
			repe: [FormatterString::new_str("repe"), FormatterString::new_str("repz")],
			repne: [FormatterString::new_str("repne"), FormatterString::new_str("repnz")],
			rex_w: FormatterString::new_str("rex.w"),
			rne_sae: FormatterString::new_str("rne-sae"),
			rn_sae: FormatterString::new_str("rn-sae"),
			ru_sae: FormatterString::new_str("ru-sae"),
			rz_sae: FormatterString::new_str("rz-sae"),
			sae: FormatterString::new_str("sae"),
			short: FormatterString::new_str("short"),
			to: FormatterString::new_str("to"),
			xacquire: FormatterString::new_str("xacquire"),
			xrelease: FormatterString::new_str("xrelease"),
			z: FormatterString::new_str("z"),
			a16: FormatterString::new_str("a16"),
			a32: FormatterString::new_str("a32"),
			a64: FormatterString::new_str("a64"),
			addr16: FormatterString::new_str("addr16"),
			addr32: FormatterString::new_str("addr32"),
			addr64: FormatterString::new_str("addr64"),
			data16: FormatterString::new_str("data16"),
			data32: FormatterString::new_str("data32"),
			data64: FormatterString::new_str("data64"),
			o16: FormatterString::new_str("o16"),
			o32: FormatterString::new_str("o32"),
			o64: FormatterString::new_str("o64"),
		}
	};
}

#[allow(dead_code)]
pub(super) struct FormatterArrayConstants {
	pub(super) nothing: [&'static FormatterString; 0],
	pub(super) byte_ptr: [&'static FormatterString; 2],
	pub(super) dword_ptr: [&'static FormatterString; 2],
	pub(super) fpuenv14_ptr: [&'static FormatterString; 2],
	pub(super) fpuenv28_ptr: [&'static FormatterString; 2],
	pub(super) fpustate108_ptr: [&'static FormatterString; 2],
	pub(super) fpustate94_ptr: [&'static FormatterString; 2],
	pub(super) fword_ptr: [&'static FormatterString; 2],
	pub(super) qword_ptr: [&'static FormatterString; 2],
	pub(super) tbyte_ptr: [&'static FormatterString; 2],
	pub(super) word_ptr: [&'static FormatterString; 2],
	pub(super) mmword_ptr: [&'static FormatterString; 2],
	pub(super) xmmword_ptr: [&'static FormatterString; 2],
	pub(super) ymmword_ptr: [&'static FormatterString; 2],
	pub(super) zmmword_ptr: [&'static FormatterString; 2],
	pub(super) oword_ptr: [&'static FormatterString; 2],
	pub(super) dword_bcst: [&'static FormatterString; 2],
	pub(super) qword_bcst: [&'static FormatterString; 2],
	#[cfg(feature = "gas")]
	pub(super) gas_op_size_strings: [&'static FormatterString; super::gas::enums::InstrOpInfoFlags::SIZE_OVERRIDE_MASK as usize + 1],
	#[cfg(not(feature = "gas"))]
	pub(super) gas_op_size_strings: (),
	#[cfg(feature = "gas")]
	pub(super) gas_addr_size_strings: [&'static FormatterString; super::gas::enums::InstrOpInfoFlags::SIZE_OVERRIDE_MASK as usize + 1],
	#[cfg(not(feature = "gas"))]
	pub(super) gas_addr_size_strings: (),
	#[cfg(feature = "intel")]
	pub(super) intel_op_size_strings: [&'static FormatterString; super::intel::enums::InstrOpInfoFlags::SIZE_OVERRIDE_MASK as usize + 1],
	#[cfg(not(feature = "intel"))]
	pub(super) intel_op_size_strings: (),
	#[cfg(feature = "intel")]
	pub(super) intel_addr_size_strings: [&'static FormatterString; super::intel::enums::InstrOpInfoFlags::SIZE_OVERRIDE_MASK as usize + 1],
	#[cfg(not(feature = "intel"))]
	pub(super) intel_addr_size_strings: (),
	#[cfg(feature = "intel")]
	pub(super) intel_rc_strings: [&'static FormatterString; 4],
	#[cfg(not(feature = "intel"))]
	pub(super) intel_rc_strings: (),
	#[cfg(feature = "intel")]
	pub(super) intel_branch_infos: [Vec<&'static FormatterString>; super::intel::enums::InstrOpInfoFlags::BRANCH_SIZE_INFO_MASK as usize + 1],
	#[cfg(not(feature = "intel"))]
	pub(super) intel_branch_infos: (),
	#[cfg(feature = "masm")]
	pub(super) masm_rc_strings: [&'static FormatterString; 4],
	#[cfg(not(feature = "masm"))]
	pub(super) masm_rc_strings: (),
	#[cfg(feature = "nasm")]
	pub(super) nasm_op_size_strings: [&'static FormatterString; super::nasm::enums::InstrOpInfoFlags::SIZE_OVERRIDE_MASK as usize + 1],
	#[cfg(not(feature = "nasm"))]
	pub(super) nasm_op_size_strings: (),
	#[cfg(feature = "nasm")]
	pub(super) nasm_addr_size_strings: [&'static FormatterString; super::nasm::enums::InstrOpInfoFlags::SIZE_OVERRIDE_MASK as usize + 1],
	#[cfg(not(feature = "nasm"))]
	pub(super) nasm_addr_size_strings: (),
	#[cfg(feature = "nasm")]
	pub(super) nasm_branch_infos: [Vec<&'static FormatterString>; super::nasm::enums::InstrOpInfoFlags::BRANCH_SIZE_INFO_MASK as usize + 1],
	#[cfg(not(feature = "nasm"))]
	pub(super) nasm_branch_infos: (),
	#[cfg(feature = "nasm")]
	pub(super) nasm_mem_size_infos: [&'static FormatterString; super::nasm::enums::InstrOpInfoFlags::MEMORY_SIZE_INFO_MASK as usize + 1],
	#[cfg(not(feature = "nasm"))]
	pub(super) nasm_mem_size_infos: (),
	#[cfg(feature = "nasm")]
	pub(super) nasm_far_mem_size_infos: [&'static FormatterString; super::nasm::enums::InstrOpInfoFlags::FAR_MEMORY_SIZE_INFO_MASK as usize + 1],
	#[cfg(not(feature = "nasm"))]
	pub(super) nasm_far_mem_size_infos: (),
}

lazy_static! {
	pub(super) static ref ARRAY_CONSTS: FormatterArrayConstants = {
		#![cfg_attr(feature = "cargo-clippy", allow(clippy::let_unit_value))]
		let c = &*FORMATTER_CONSTANTS;
		let nothing: [&'static FormatterString; 0] = [];
		let byte_ptr: [&'static FormatterString; 2] = [&c.byte, &c.ptr];
		let word_ptr: [&'static FormatterString; 2] = [&c.word, &c.ptr];
		let dword_ptr: [&'static FormatterString; 2] = [&c.dword, &c.ptr];
		let qword_ptr: [&'static FormatterString; 2] = [&c.qword, &c.ptr];
		let mmword_ptr: [&'static FormatterString; 2] = [&c.mmword, &c.ptr];
		let xmmword_ptr: [&'static FormatterString; 2] = [&c.xmmword, &c.ptr];
		let ymmword_ptr: [&'static FormatterString; 2] = [&c.ymmword, &c.ptr];
		let zmmword_ptr: [&'static FormatterString; 2] = [&c.zmmword, &c.ptr];
		let fword_ptr: [&'static FormatterString; 2] = [&c.fword, &c.ptr];
		let tbyte_ptr: [&'static FormatterString; 2] = [&c.tbyte, &c.ptr];
		let fpuenv14_ptr: [&'static FormatterString; 2] = [&c.fpuenv14, &c.ptr];
		let fpuenv28_ptr: [&'static FormatterString; 2] = [&c.fpuenv28, &c.ptr];
		let fpustate108_ptr: [&'static FormatterString; 2] = [&c.fpustate108, &c.ptr];
		let fpustate94_ptr: [&'static FormatterString; 2] = [&c.fpustate94, &c.ptr];
		let oword_ptr: [&'static FormatterString; 2] = [&c.oword, &c.ptr];
		let dword_bcst: [&'static FormatterString; 2] = [&c.dword, &c.bcst];
		let qword_bcst: [&'static FormatterString; 2] = [&c.qword, &c.bcst];
		#[cfg(feature = "gas")]
		#[cfg_attr(feature = "cargo-fmt", rustfmt::skip)]
		let gas_op_size_strings: [&'static FormatterString; super::gas::enums::InstrOpInfoFlags::SIZE_OVERRIDE_MASK as usize + 1] = [
			&c.empty,
			&c.data16,
			&c.data32,
			&c.rex_w,
		];
		#[cfg(not(feature = "gas"))]
		let gas_op_size_strings = ();
		#[cfg(feature = "gas")]
		#[cfg_attr(feature = "cargo-fmt", rustfmt::skip)]
		let gas_addr_size_strings: [&'static FormatterString; super::gas::enums::InstrOpInfoFlags::SIZE_OVERRIDE_MASK as usize + 1] = [
			&c.empty,
			&c.addr16,
			&c.addr32,
			&c.addr64,
		];
		#[cfg(not(feature = "gas"))]
		let gas_addr_size_strings = ();
		#[cfg(feature = "intel")]
		#[cfg_attr(feature = "cargo-fmt", rustfmt::skip)]
		let intel_op_size_strings: [&'static FormatterString; super::intel::enums::InstrOpInfoFlags::SIZE_OVERRIDE_MASK as usize + 1] = [
			&c.empty,
			&c.data16,
			&c.data32,
			&c.data64,
		];
		#[cfg(not(feature = "intel"))]
		let intel_op_size_strings = ();
		#[cfg(feature = "intel")]
		#[cfg_attr(feature = "cargo-fmt", rustfmt::skip)]
		let intel_addr_size_strings: [&'static FormatterString; super::intel::enums::InstrOpInfoFlags::SIZE_OVERRIDE_MASK as usize + 1] = [
			&c.empty,
			&c.addr16,
			&c.addr32,
			&c.addr64,
		];
		#[cfg(not(feature = "intel"))]
		let intel_addr_size_strings = ();
		#[cfg(feature = "intel")]
		#[cfg_attr(feature = "cargo-fmt", rustfmt::skip)]
		let intel_rc_strings: [&'static FormatterString; 4] = [
			&c.rne_sae,
			&c.rd_sae,
			&c.ru_sae,
			&c.rz_sae,
		];
		#[cfg(not(feature = "intel"))]
		let intel_rc_strings = ();
		#[cfg(feature = "intel")]
		#[cfg_attr(feature = "cargo-fmt", rustfmt::skip)]
		let intel_branch_infos: [Vec<&'static FormatterString>; super::intel::enums::InstrOpInfoFlags::BRANCH_SIZE_INFO_MASK as usize + 1] = [
			vec![],
			vec![&c.short],
		];
		#[cfg(not(feature = "intel"))]
		let intel_branch_infos = ();
		#[cfg(feature = "masm")]
		#[cfg_attr(feature = "cargo-fmt", rustfmt::skip)]
		let masm_rc_strings: [&'static FormatterString; 4] = [
			&c.rn_sae,
			&c.rd_sae,
			&c.ru_sae,
			&c.rz_sae,
		];
		#[cfg(not(feature = "masm"))]
		let masm_rc_strings = ();
		#[cfg(feature = "nasm")]
		#[cfg_attr(feature = "cargo-fmt", rustfmt::skip)]
		let nasm_op_size_strings: [&'static FormatterString; super::nasm::enums::InstrOpInfoFlags::SIZE_OVERRIDE_MASK as usize + 1] = [
			&c.empty,
			&c.o16,
			&c.o32,
			&c.o64,
		];
		#[cfg(not(feature = "nasm"))]
		let nasm_op_size_strings = ();
		#[cfg(feature = "nasm")]
		#[cfg_attr(feature = "cargo-fmt", rustfmt::skip)]
		let nasm_addr_size_strings: [&'static FormatterString; super::nasm::enums::InstrOpInfoFlags::SIZE_OVERRIDE_MASK as usize + 1] = [
			&c.empty,
			&c.a16,
			&c.a32,
			&c.a64,
		];
		#[cfg(not(feature = "nasm"))]
		let nasm_addr_size_strings = ();
		#[cfg(feature = "nasm")]
		#[cfg_attr(feature = "cargo-fmt", rustfmt::skip)]
		let nasm_branch_infos: [Vec<&'static FormatterString>; super::nasm::enums::InstrOpInfoFlags::BRANCH_SIZE_INFO_MASK as usize + 1] = [
			vec![],
			vec![&c.near],
			vec![&c.near, &c.word],
			vec![&c.near, &c.dword],
			vec![&c.word],
			vec![&c.dword],
			vec![&c.short],
			vec![],
		];
		#[cfg(not(feature = "nasm"))]
		let nasm_branch_infos = ();
		#[cfg(feature = "nasm")]
		#[cfg_attr(feature = "cargo-fmt", rustfmt::skip)]
		let nasm_mem_size_infos: [&'static FormatterString; super::nasm::enums::InstrOpInfoFlags::MEMORY_SIZE_INFO_MASK as usize + 1] = [
			&c.empty,
			&c.word,
			&c.dword,
			&c.qword,
		];
		#[cfg(not(feature = "nasm"))]
		let nasm_mem_size_infos = ();
		#[cfg(feature = "nasm")]
		#[cfg_attr(feature = "cargo-fmt", rustfmt::skip)]
		let nasm_far_mem_size_infos: [&'static FormatterString; super::nasm::enums::InstrOpInfoFlags::FAR_MEMORY_SIZE_INFO_MASK as usize + 1] = [
			&c.empty,
			&c.word,
			&c.dword,
			&c.empty,
		];
		#[cfg(not(feature = "nasm"))]
		let nasm_far_mem_size_infos = ();

		FormatterArrayConstants {
			nothing,
			byte_ptr,
			dword_ptr,
			fpuenv14_ptr,
			fpuenv28_ptr,
			fpustate108_ptr,
			fpustate94_ptr,
			fword_ptr,
			qword_ptr,
			tbyte_ptr,
			word_ptr,
			mmword_ptr,
			xmmword_ptr,
			ymmword_ptr,
			zmmword_ptr,
			oword_ptr,
			dword_bcst,
			qword_bcst,
			gas_op_size_strings,
			gas_addr_size_strings,
			intel_op_size_strings,
			intel_addr_size_strings,
			intel_rc_strings,
			intel_branch_infos,
			masm_rc_strings,
			nasm_op_size_strings,
			nasm_addr_size_strings,
			nasm_branch_infos,
			nasm_mem_size_infos,
			nasm_far_mem_size_infos,
		}
	};
}

pub(super) static SCALE_NUMBERS: [&str; 4] = ["1", "2", "4", "8"];
