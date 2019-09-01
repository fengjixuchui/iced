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

#if !NO_ENCODER
namespace Iced.Intel.EncoderInternal {
	static class OpCodeOperandKinds {
		public static readonly byte[] LegacyOpKinds = new byte[(int)LegacyOpKind.Last] {
			(byte)OpCodeOperandKind.None,
			(byte)OpCodeOperandKind.farbr2_2,
			(byte)OpCodeOperandKind.farbr4_2,
			(byte)OpCodeOperandKind.mem,
			(byte)OpCodeOperandKind.mem,
			(byte)OpCodeOperandKind.mem,
			(byte)OpCodeOperandKind.mem,
			(byte)OpCodeOperandKind.mem,
			(byte)OpCodeOperandKind.mem,
			(byte)OpCodeOperandKind.mem,
			(byte)OpCodeOperandKind.mem,
			(byte)OpCodeOperandKind.mem,
			(byte)OpCodeOperandKind.mem,
			(byte)OpCodeOperandKind.mem,
			(byte)OpCodeOperandKind.mem,
			(byte)OpCodeOperandKind.mem,
			(byte)OpCodeOperandKind.mem,
			(byte)OpCodeOperandKind.mem,
			(byte)OpCodeOperandKind.mem,
			(byte)OpCodeOperandKind.mem,
			(byte)OpCodeOperandKind.mem,
			(byte)OpCodeOperandKind.mem_mpx,
			(byte)OpCodeOperandKind.mem,
			(byte)OpCodeOperandKind.mem_mpx,
			(byte)OpCodeOperandKind.mem,
			(byte)OpCodeOperandKind.mem,
			(byte)OpCodeOperandKind.r8_mem,
			(byte)OpCodeOperandKind.r16_mem,
			(byte)OpCodeOperandKind.r32_mem,
			(byte)OpCodeOperandKind.r32_mem_mpx,
			(byte)OpCodeOperandKind.r32_mem,
			(byte)OpCodeOperandKind.r64_mem,
			(byte)OpCodeOperandKind.r64_mem,
			(byte)OpCodeOperandKind.r64_mem_mpx,
			(byte)OpCodeOperandKind.mem,
			(byte)OpCodeOperandKind.mem,
			(byte)OpCodeOperandKind.mem,
			(byte)OpCodeOperandKind.r32_mem,
			(byte)OpCodeOperandKind.r64_mem,
			(byte)OpCodeOperandKind.r32_mem,
			(byte)OpCodeOperandKind.r64_mem,
			(byte)OpCodeOperandKind.r8_reg,
			(byte)OpCodeOperandKind.r16_reg,
			(byte)OpCodeOperandKind.r32_reg,
			(byte)OpCodeOperandKind.r64_reg,
			(byte)OpCodeOperandKind.r16_rm,
			(byte)OpCodeOperandKind.r32_rm,
			(byte)OpCodeOperandKind.r64_rm,
			(byte)OpCodeOperandKind.seg_reg,
			(byte)OpCodeOperandKind.cr_reg,
			(byte)OpCodeOperandKind.cr_reg,
			(byte)OpCodeOperandKind.dr_reg,
			(byte)OpCodeOperandKind.dr_reg,
			(byte)OpCodeOperandKind.tr_reg,
			(byte)OpCodeOperandKind.imm8,
			(byte)OpCodeOperandKind.imm8sex16,
			(byte)OpCodeOperandKind.imm8sex32,
			(byte)OpCodeOperandKind.imm8sex64,
			(byte)OpCodeOperandKind.imm16,
			(byte)OpCodeOperandKind.imm32,
			(byte)OpCodeOperandKind.imm32sex64,
			(byte)OpCodeOperandKind.imm64,
			(byte)OpCodeOperandKind.imm8,
			(byte)OpCodeOperandKind.imm8,
			(byte)OpCodeOperandKind.seg_rSI,
			(byte)OpCodeOperandKind.seg_rSI,
			(byte)OpCodeOperandKind.seg_rSI,
			(byte)OpCodeOperandKind.seg_rSI,
			(byte)OpCodeOperandKind.es_rDI,
			(byte)OpCodeOperandKind.es_rDI,
			(byte)OpCodeOperandKind.es_rDI,
			(byte)OpCodeOperandKind.es_rDI,
			(byte)OpCodeOperandKind.br16_1,
			(byte)OpCodeOperandKind.br32_1,
			(byte)OpCodeOperandKind.br64_1,
			(byte)OpCodeOperandKind.br16_2,
			(byte)OpCodeOperandKind.br32_4,
			(byte)OpCodeOperandKind.br32_4,
			(byte)OpCodeOperandKind.br64_4,
			(byte)OpCodeOperandKind.xbegin_2,
			(byte)OpCodeOperandKind.xbegin_4,
			(byte)OpCodeOperandKind.brdisp_2,
			(byte)OpCodeOperandKind.brdisp_4,
			(byte)OpCodeOperandKind.mem_offs,
			(byte)OpCodeOperandKind.mem_offs,
			(byte)OpCodeOperandKind.mem_offs,
			(byte)OpCodeOperandKind.mem_offs,
			(byte)OpCodeOperandKind.imm8_const_1,
			(byte)OpCodeOperandKind.bnd_reg,
			(byte)OpCodeOperandKind.bnd_mem_mpx,
			(byte)OpCodeOperandKind.bnd_mem_mpx,
			(byte)OpCodeOperandKind.mem_mpx,
			(byte)OpCodeOperandKind.mm_rm,
			(byte)OpCodeOperandKind.mm_reg,
			(byte)OpCodeOperandKind.mm_mem,
			(byte)OpCodeOperandKind.xmm_rm,
			(byte)OpCodeOperandKind.xmm_reg,
			(byte)OpCodeOperandKind.xmm_mem,
			(byte)OpCodeOperandKind.seg_rDI,
			(byte)OpCodeOperandKind.seg_rBX_al,
			(byte)OpCodeOperandKind.es,
			(byte)OpCodeOperandKind.cs,
			(byte)OpCodeOperandKind.ss,
			(byte)OpCodeOperandKind.ds,
			(byte)OpCodeOperandKind.fs,
			(byte)OpCodeOperandKind.gs,
			(byte)OpCodeOperandKind.al,
			(byte)OpCodeOperandKind.cl,
			(byte)OpCodeOperandKind.ax,
			(byte)OpCodeOperandKind.dx,
			(byte)OpCodeOperandKind.eax,
			(byte)OpCodeOperandKind.rax,
			(byte)OpCodeOperandKind.st0,
			(byte)OpCodeOperandKind.sti_opcode,
			(byte)OpCodeOperandKind.r8_opcode,
			(byte)OpCodeOperandKind.r16_opcode,
			(byte)OpCodeOperandKind.r32_opcode,
			(byte)OpCodeOperandKind.r64_opcode,
		};

		public static readonly byte[] VexOpKinds = new byte[(int)VexOpKind.Last] {
			(byte)OpCodeOperandKind.None,
			(byte)OpCodeOperandKind.r32_mem,
			(byte)OpCodeOperandKind.r64_mem,
			(byte)OpCodeOperandKind.r32_reg,
			(byte)OpCodeOperandKind.r64_reg,
			(byte)OpCodeOperandKind.r32_mem,
			(byte)OpCodeOperandKind.r64_mem,
			(byte)OpCodeOperandKind.r32_mem,
			(byte)OpCodeOperandKind.r64_mem,
			(byte)OpCodeOperandKind.r32_rm,
			(byte)OpCodeOperandKind.r64_rm,
			(byte)OpCodeOperandKind.r32_vvvv,
			(byte)OpCodeOperandKind.r64_vvvv,
			(byte)OpCodeOperandKind.k_vvvv,
			(byte)OpCodeOperandKind.xmm_vvvv,
			(byte)OpCodeOperandKind.ymm_vvvv,
			(byte)OpCodeOperandKind.imm8,
			(byte)OpCodeOperandKind.imm2_m2z,
			(byte)OpCodeOperandKind.xmm_is4,
			(byte)OpCodeOperandKind.ymm_is4,
			(byte)OpCodeOperandKind.xmm_is5,
			(byte)OpCodeOperandKind.ymm_is5,
			(byte)OpCodeOperandKind.mem,
			(byte)OpCodeOperandKind.mem,
			(byte)OpCodeOperandKind.mem,
			(byte)OpCodeOperandKind.seg_rDI,
			(byte)OpCodeOperandKind.k_rm,
			(byte)OpCodeOperandKind.xmm_rm,
			(byte)OpCodeOperandKind.ymm_rm,
			(byte)OpCodeOperandKind.k_reg,
			(byte)OpCodeOperandKind.mem_vsib32x,
			(byte)OpCodeOperandKind.mem_vsib32y,
			(byte)OpCodeOperandKind.mem_vsib64x,
			(byte)OpCodeOperandKind.mem_vsib64y,
			(byte)OpCodeOperandKind.xmm_reg,
			(byte)OpCodeOperandKind.ymm_reg,
			(byte)OpCodeOperandKind.k_mem,
			(byte)OpCodeOperandKind.xmm_mem,
			(byte)OpCodeOperandKind.ymm_mem,
		};

		public static readonly byte[] XopOpKinds = new byte[(int)XopOpKind.Last] {
			(byte)OpCodeOperandKind.None,
			(byte)OpCodeOperandKind.r32_mem,
			(byte)OpCodeOperandKind.r64_mem,
			(byte)OpCodeOperandKind.r32_reg,
			(byte)OpCodeOperandKind.r64_reg,
			(byte)OpCodeOperandKind.r32_rm,
			(byte)OpCodeOperandKind.r64_rm,
			(byte)OpCodeOperandKind.r32_vvvv,
			(byte)OpCodeOperandKind.r64_vvvv,
			(byte)OpCodeOperandKind.xmm_vvvv,
			(byte)OpCodeOperandKind.ymm_vvvv,
			(byte)OpCodeOperandKind.imm8,
			(byte)OpCodeOperandKind.imm32,
			(byte)OpCodeOperandKind.xmm_is4,
			(byte)OpCodeOperandKind.ymm_is4,
			(byte)OpCodeOperandKind.xmm_reg,
			(byte)OpCodeOperandKind.ymm_reg,
			(byte)OpCodeOperandKind.xmm_mem,
			(byte)OpCodeOperandKind.ymm_mem,
		};

		public static readonly byte[] EvexOpKinds = new byte[(int)EvexOpKind.Last] {
			(byte)OpCodeOperandKind.None,
			(byte)OpCodeOperandKind.r32_mem,
			(byte)OpCodeOperandKind.r64_mem,
			(byte)OpCodeOperandKind.r32_reg,
			(byte)OpCodeOperandKind.r64_reg,
			(byte)OpCodeOperandKind.r32_mem,
			(byte)OpCodeOperandKind.r64_mem,
			(byte)OpCodeOperandKind.r32_mem,
			(byte)OpCodeOperandKind.r64_mem,
			(byte)OpCodeOperandKind.xmm_vvvv,
			(byte)OpCodeOperandKind.ymm_vvvv,
			(byte)OpCodeOperandKind.zmm_vvvv,
			(byte)OpCodeOperandKind.xmmp3_vvvv,
			(byte)OpCodeOperandKind.zmmp3_vvvv,
			(byte)OpCodeOperandKind.imm8,
			(byte)OpCodeOperandKind.mem,
			(byte)OpCodeOperandKind.r32_rm,
			(byte)OpCodeOperandKind.r64_rm,
			(byte)OpCodeOperandKind.xmm_rm,
			(byte)OpCodeOperandKind.ymm_rm,
			(byte)OpCodeOperandKind.zmm_rm,
			(byte)OpCodeOperandKind.k_rm,
			(byte)OpCodeOperandKind.mem_vsib32x,
			(byte)OpCodeOperandKind.mem_vsib32y,
			(byte)OpCodeOperandKind.mem_vsib32z,
			(byte)OpCodeOperandKind.mem_vsib64x,
			(byte)OpCodeOperandKind.mem_vsib64y,
			(byte)OpCodeOperandKind.mem_vsib64z,
			(byte)OpCodeOperandKind.k_reg,
			(byte)OpCodeOperandKind.xmm_reg,
			(byte)OpCodeOperandKind.ymm_reg,
			(byte)OpCodeOperandKind.zmm_reg,
			(byte)OpCodeOperandKind.xmm_mem,
			(byte)OpCodeOperandKind.ymm_mem,
			(byte)OpCodeOperandKind.zmm_mem,
		};
	}
}
#endif
