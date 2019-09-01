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
using System;
using System.Text;

namespace Iced.Intel.EncoderInternal {
	static class OpCodeFormatter {
		public static string ToString(OpCodeInfo opCode, StringBuilder sb, bool l0l1) {
			if (!opCode.IsInstruction) {
				switch (opCode.Code) {
				case Code.INVALID:		return "<invalid>";
				case Code.DeclareByte:	return "<db>";
				case Code.DeclareWord:	return "<dw>";
				case Code.DeclareDword:	return "<dd>";
				case Code.DeclareQword:	return "<dq>";
				default:				throw new InvalidOperationException();
				}
			}

			switch (opCode.Encoding) {
			case EncodingKind.Legacy:
				return ToString_Legacy(opCode, sb);

			case EncodingKind.VEX:
				return ToString_VEX_XOP_EVEX(opCode, sb, l0l1, "VEX");

			case EncodingKind.EVEX:
				return ToString_VEX_XOP_EVEX(opCode, sb, l0l1, "EVEX");

			case EncodingKind.XOP:
				return ToString_VEX_XOP_EVEX(opCode, sb, l0l1, "XOP");

			case EncodingKind.D3NOW:
				return ToString_3DNow(opCode, sb);

			default:
				throw new InvalidOperationException();
			}
		}

		static void AppendHexByte(StringBuilder sb, byte value) => sb.Append(value.ToString("X2"));

		static void AppendOpCode(StringBuilder sb, uint value, bool sep) {
			if (value <= byte.MaxValue)
				AppendHexByte(sb, (byte)value);
			else if (value <= ushort.MaxValue) {
				AppendHexByte(sb, (byte)(value >> 8));
				if (sep)
					sb.Append(' ');
				AppendHexByte(sb, (byte)value);
			}
			else
				throw new InvalidOperationException();
		}

		static void AppendTable(OpCodeInfo opCode, StringBuilder sb, bool sep) {
			switch (opCode.Table) {
			case OpCodeTableKind.Normal:
				break;

			case OpCodeTableKind.T0F:
				AppendOpCode(sb, 0x0F, sep);
				break;

			case OpCodeTableKind.T0F38:
				AppendOpCode(sb, 0x0F38, sep);
				break;

			case OpCodeTableKind.T0F3A:
				AppendOpCode(sb, 0x0F3A, sep);
				break;

			case OpCodeTableKind.XOP8:
				sb.Append("X8");
				break;

			case OpCodeTableKind.XOP9:
				sb.Append("X9");
				break;

			case OpCodeTableKind.XOPA:
				sb.Append("XA");
				break;

			default:
				throw new InvalidOperationException();
			}
		}

		static bool HasModRM(OpCodeInfo opCode) {
			int opCount = opCode.OpCount;
			if (opCount == 0)
				return false;

			switch (opCode.Encoding) {
			case EncodingKind.Legacy:
				break;
			case EncodingKind.VEX:
			case EncodingKind.EVEX:
			case EncodingKind.XOP:
			case EncodingKind.D3NOW:
				return true;
			default:
				throw new InvalidOperationException();
			}

			for (int i = 0; i < opCount; i++) {
				switch (opCode.GetOpKind(i)) {
				case OpCodeOperandKind.mem:
				case OpCodeOperandKind.mem_mpx:
				case OpCodeOperandKind.mem_vsib32x:
				case OpCodeOperandKind.mem_vsib64x:
				case OpCodeOperandKind.mem_vsib32y:
				case OpCodeOperandKind.mem_vsib64y:
				case OpCodeOperandKind.mem_vsib32z:
				case OpCodeOperandKind.mem_vsib64z:
				case OpCodeOperandKind.r8_mem:
				case OpCodeOperandKind.r16_mem:
				case OpCodeOperandKind.r32_mem:
				case OpCodeOperandKind.r32_mem_mpx:
				case OpCodeOperandKind.r64_mem:
				case OpCodeOperandKind.r64_mem_mpx:
				case OpCodeOperandKind.mm_mem:
				case OpCodeOperandKind.xmm_mem:
				case OpCodeOperandKind.ymm_mem:
				case OpCodeOperandKind.zmm_mem:
				case OpCodeOperandKind.bnd_mem_mpx:
				case OpCodeOperandKind.k_mem:
				case OpCodeOperandKind.r8_reg:
				case OpCodeOperandKind.r16_reg:
				case OpCodeOperandKind.r16_rm:
				case OpCodeOperandKind.r32_reg:
				case OpCodeOperandKind.r32_rm:
				case OpCodeOperandKind.r64_reg:
				case OpCodeOperandKind.r64_rm:
				case OpCodeOperandKind.seg_reg:
				case OpCodeOperandKind.k_reg:
				case OpCodeOperandKind.k_rm:
				case OpCodeOperandKind.mm_reg:
				case OpCodeOperandKind.mm_rm:
				case OpCodeOperandKind.xmm_reg:
				case OpCodeOperandKind.xmm_rm:
				case OpCodeOperandKind.ymm_reg:
				case OpCodeOperandKind.ymm_rm:
				case OpCodeOperandKind.zmm_reg:
				case OpCodeOperandKind.zmm_rm:
				case OpCodeOperandKind.cr_reg:
				case OpCodeOperandKind.dr_reg:
				case OpCodeOperandKind.tr_reg:
				case OpCodeOperandKind.bnd_reg:
					return true;
				}
			}
			return false;
		}

		static bool HasVsib(OpCodeInfo opCode) {
			int opCount = opCode.OpCount;
			for (int i = 0; i < opCount; i++) {
				switch (opCode.GetOpKind(i)) {
				case OpCodeOperandKind.mem_vsib32x:
				case OpCodeOperandKind.mem_vsib64x:
				case OpCodeOperandKind.mem_vsib32y:
				case OpCodeOperandKind.mem_vsib64y:
				case OpCodeOperandKind.mem_vsib32z:
				case OpCodeOperandKind.mem_vsib64z:
					return true;
				}
			}
			return false;
		}

		static OpCodeOperandKind GetOpCodeBitsOperand(OpCodeInfo opCode) {
			int opCount = opCode.OpCount;
			for (int i = 0; i < opCount; i++) {
				var opKind = opCode.GetOpKind(i);
				switch (opKind) {
				case OpCodeOperandKind.r8_opcode:
				case OpCodeOperandKind.r16_opcode:
				case OpCodeOperandKind.r32_opcode:
				case OpCodeOperandKind.r64_opcode:
					return opKind;
				}
			}
			return OpCodeOperandKind.None;
		}

		static void AppendRest(OpCodeInfo opCode, StringBuilder sb) {
			bool isVsib = HasVsib(opCode);
			if (opCode.IsGroup) {
				sb.Append(" /");
				sb.Append(opCode.GroupIndex);
			}
			else if (!isVsib && HasModRM(opCode))
				sb.Append(" /r");
			if (isVsib)
				sb.Append(" /vsib");

			int opCount = opCode.OpCount;
			for (int i = 0; i < opCount; i++) {
				switch (opCode.GetOpKind(i)) {
				case OpCodeOperandKind.br16_1:
				case OpCodeOperandKind.br32_1:
				case OpCodeOperandKind.br64_1:
					sb.Append(" cb");
					break;

				case OpCodeOperandKind.br16_2:
				case OpCodeOperandKind.xbegin_2:
				case OpCodeOperandKind.brdisp_2:
					sb.Append(" cw");
					break;

				case OpCodeOperandKind.farbr2_2:
				case OpCodeOperandKind.br32_4:
				case OpCodeOperandKind.br64_4:
				case OpCodeOperandKind.xbegin_4:
				case OpCodeOperandKind.brdisp_4:
					sb.Append(" cd");
					break;

				case OpCodeOperandKind.farbr4_2:
					sb.Append(" cp");
					break;

				case OpCodeOperandKind.imm8:
				case OpCodeOperandKind.imm8sex16:
				case OpCodeOperandKind.imm8sex32:
				case OpCodeOperandKind.imm8sex64:
					sb.Append(" ib");
					break;

				case OpCodeOperandKind.imm16:
					sb.Append(" iw");
					break;

				case OpCodeOperandKind.imm32:
				case OpCodeOperandKind.imm32sex64:
					sb.Append(" id");
					break;

				case OpCodeOperandKind.imm64:
					sb.Append(" io");
					break;

				case OpCodeOperandKind.xmm_is4:
				case OpCodeOperandKind.ymm_is4:
					sb.Append(" /is4");
					break;

				case OpCodeOperandKind.xmm_is5:
				case OpCodeOperandKind.ymm_is5:
					sb.Append(" /is5");
					// don't show the next imm8
					i = opCount;
					break;

				case OpCodeOperandKind.mem_offs:
					sb.Append(" mo");
					break;
				}
			}
		}

		static string ToString_Legacy(OpCodeInfo opCode, StringBuilder sb) {
			sb.Length = 0;

			if (opCode.Fwait) {
				AppendHexByte(sb, 0x9B);
				sb.Append(' ');
			}

			switch (opCode.AddressSize) {
			case 0:
				break;

			case 16:
				sb.Append("a16 ");
				break;

			case 32:
				sb.Append("a32 ");
				break;

			case 64:
				sb.Append("a64 ");
				break;

			default:
				throw new InvalidOperationException();
			}

			switch (opCode.OperandSize) {
			case 0:
				break;

			case 16:
				sb.Append("o16 ");
				break;

			case 32:
				sb.Append("o32 ");
				break;

			case 64:
				// REX.W must be immediately before the opcode and is handled below
				break;

			default:
				throw new InvalidOperationException();
			}

			switch (opCode.MandatoryPrefix) {
			case MandatoryPrefix.None:
				break;
			case MandatoryPrefix.PNP:
				sb.Append("NP ");
				break;
			case MandatoryPrefix.P66:
				AppendHexByte(sb, 0x66);
				sb.Append(' ');
				break;
			case MandatoryPrefix.PF3:
				AppendHexByte(sb, 0xF3);
				sb.Append(' ');
				break;
			case MandatoryPrefix.PF2:
				AppendHexByte(sb, 0xF2);
				sb.Append(' ');
				break;
			default:
				throw new InvalidOperationException();
			}

			if (opCode.OperandSize == 64) {
				// There's no '+' because Intel isn't consistent, some opcodes use it others don't
				sb.Append("REX.W ");
			}

			AppendTable(opCode, sb, true);
			if (opCode.Table != OpCodeTableKind.Normal)
				sb.Append(' ');
			AppendOpCode(sb, opCode.OpCode, true);
			switch (GetOpCodeBitsOperand(opCode)) {
			case OpCodeOperandKind.r8_opcode:
				sb.Append("+rb");
				break;
			case OpCodeOperandKind.r16_opcode:
				sb.Append("+rw");
				break;
			case OpCodeOperandKind.r32_opcode:
				sb.Append("+rd");
				break;
			case OpCodeOperandKind.r64_opcode:
				sb.Append("+ro");
				break;
			case OpCodeOperandKind.None:
				break;
			default:
				throw new InvalidOperationException();
			}
			int opCount = opCode.OpCount;
			for (int i = 0; i < opCount; i++) {
				if (opCode.GetOpKind(i) == OpCodeOperandKind.sti_opcode) {
					sb.Append("+i");
					break;
				}
			}

			AppendRest(opCode, sb);

			return sb.ToString();
		}

		static string ToString_3DNow(OpCodeInfo opCode, StringBuilder sb) {
			sb.Length = 0;

			AppendOpCode(sb, 0x0F0F, true);
			sb.Append(" /r");
			sb.Append(' ');
			AppendOpCode(sb, opCode.OpCode, true);

			return sb.ToString();
		}

		static string ToString_VEX_XOP_EVEX(OpCodeInfo opCode, StringBuilder sb, bool l0l1, string encodingName) {
			sb.Length = 0;

			sb.Append(encodingName);
			sb.Append('.');
			if (opCode.IsLIG)
				sb.Append("LIG");
			else if (l0l1) {
				sb.Append('L');
				sb.Append(opCode.L);
			}
			else
				sb.Append(128U << (int)opCode.L);
			switch (opCode.MandatoryPrefix) {
			case MandatoryPrefix.None:
			case MandatoryPrefix.PNP:
				break;
			case MandatoryPrefix.P66:
				sb.Append('.');
				AppendHexByte(sb, 0x66);
				break;
			case MandatoryPrefix.PF3:
				sb.Append('.');
				AppendHexByte(sb, 0xF3);
				break;
			case MandatoryPrefix.PF2:
				sb.Append('.');
				AppendHexByte(sb, 0xF2);
				break;
			default:
				throw new InvalidOperationException();
			}
			sb.Append('.');
			AppendTable(opCode, sb, false);
			if (opCode.IsWIG)
				sb.Append(".WIG");
			else {
				sb.Append(".W");
				sb.Append(opCode.W);
			}
			sb.Append(' ');
			AppendOpCode(sb, opCode.OpCode, true);
			AppendRest(opCode, sb);

			return sb.ToString();
		}
	}
}
#endif
