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
using System.Diagnostics;

namespace Iced.Intel.EncoderInternal {
	enum LegacyOpKind : byte {
		None,
		Aww,
		Adw,
		M,
		Mfbcd,
		Mf32,
		Mf64,
		Mf80,
		Mfi16,
		Mfi32,
		Mfi64,
		M14,
		M28,
		M98,
		M108,
		Mp,
		Ms,
		Mo,
		Mb,
		Mw,
		Md,
		Md_MPX,
		Mq,
		Mq_MPX,
		Mw2,
		Md2,
		Eb,
		Ew,
		Ed,
		Ed_MPX,
		Ew_d,
		Ew_q,
		Eq,
		Eq_MPX,
		Eww,
		Edw,
		Eqw,
		RdMb,
		RqMb,
		RdMw,
		RqMw,
		Gb,
		Gw,
		Gd,
		Gq,
		Rw,
		Rd,
		Rq,
		Sw,
		Cd,
		Cq,
		Dd,
		Dq,
		Td,
		Ib,
		Ib16,
		Ib32,
		Ib64,
		Iw,
		Id,
		Id64,
		Iq,
		Ib21,
		Ib11,
		Xb,
		Xw,
		Xd,
		Xq,
		Yb,
		Yw,
		Yd,
		Yq,
		wJb,
		dJb,
		qJb,
		Jw,
		wJd,
		dJd,
		qJd,
		Jxw,
		Jxd,
		Jdisp16,
		Jdisp32,
		Ob,
		Ow,
		Od,
		Oq,
		Imm1,
		B,
		BMq,
		BMo,
		MIB,
		N,
		P,
		Q,
		RX,
		VX,
		WX,
		rDI,
		MRBX,
		ES,
		CS,
		SS,
		DS,
		FS,
		GS,
		AL,
		CL,
		AX,
		DX,
		EAX,
		RAX,
		ST,
		STi,
		r8_rb,
		r16_rw,
		r32_rd,
		r64_ro,

		Last,
	}

	static class LegacyOps {
		public static readonly Op[] Ops = new Op[(int)LegacyOpKind.Last] {
			null!,// Never accessed
			new OpA(2),
			new OpA(4),
			new OpModRM_rm_mem_only(),
			new OpModRM_rm_mem_only(),
			new OpModRM_rm_mem_only(),
			new OpModRM_rm_mem_only(),
			new OpModRM_rm_mem_only(),
			new OpModRM_rm_mem_only(),
			new OpModRM_rm_mem_only(),
			new OpModRM_rm_mem_only(),
			new OpModRM_rm_mem_only(),
			new OpModRM_rm_mem_only(),
			new OpModRM_rm_mem_only(),
			new OpModRM_rm_mem_only(),
			new OpModRM_rm_mem_only(),
			new OpModRM_rm_mem_only(),
			new OpModRM_rm_mem_only(),
			new OpModRM_rm_mem_only(),
			new OpModRM_rm_mem_only(),
			new OpModRM_rm_mem_only(),
			new OpModRM_rm_mem_only(),
			new OpModRM_rm_mem_only(),
			new OpModRM_rm_mem_only(),
			new OpModRM_rm_mem_only(),
			new OpModRM_rm_mem_only(),
			new OpModRM_rm(Register.AL, Register.R15L),
			new OpModRM_rm(Register.AX, Register.R15W),
			new OpModRM_rm(Register.EAX, Register.R15D),
			new OpModRM_rm(Register.EAX, Register.R15D),
			new OpModRM_rm(Register.EAX, Register.R15D),
			new OpModRM_rm(Register.RAX, Register.R15),
			new OpModRM_rm(Register.RAX, Register.R15),
			new OpModRM_rm(Register.RAX, Register.R15),
			new OpModRM_rm_mem_only(),
			new OpModRM_rm_mem_only(),
			new OpModRM_rm_mem_only(),
			new OpModRM_rm(Register.EAX, Register.R15D),
			new OpModRM_rm(Register.RAX, Register.R15),
			new OpModRM_rm(Register.EAX, Register.R15D),
			new OpModRM_rm(Register.RAX, Register.R15),
			new OpModRM_reg(Register.AL, Register.R15L),
			new OpModRM_reg(Register.AX, Register.R15W),
			new OpModRM_reg(Register.EAX, Register.R15D),
			new OpModRM_reg(Register.RAX, Register.R15),
			new OpModRM_rm_reg_only(Register.AX, Register.R15W),
			new OpModRM_rm_reg_only(Register.EAX, Register.R15D),
			new OpModRM_rm_reg_only(Register.RAX, Register.R15),
			new OpModRM_reg(Register.ES, Register.GS),
			new OpModRM_regF0(Register.CR0, Register.CR15),
			new OpModRM_reg(Register.CR0, Register.CR15),
			new OpModRM_reg(Register.DR0, Register.DR15),
			new OpModRM_reg(Register.DR0, Register.DR15),
			new OpModRM_reg(Register.TR0, Register.TR7),
			new OpIb(OpKind.Immediate8),
			new OpIb(OpKind.Immediate8to16),
			new OpIb(OpKind.Immediate8to32),
			new OpIb(OpKind.Immediate8to64),
			new OpIw(),
			new OpId(OpKind.Immediate32),
			new OpId(OpKind.Immediate32to64),
			new OpIq(),
			new OpIb21(),
			new OpIb11(),
			new OpX(),
			new OpX(),
			new OpX(),
			new OpX(),
			new OpY(),
			new OpY(),
			new OpY(),
			new OpY(),
			new OpJ(OpKind.NearBranch16, 1),
			new OpJ(OpKind.NearBranch32, 1),
			new OpJ(OpKind.NearBranch64, 1),
			new OpJ(OpKind.NearBranch16, 2),
			new OpJ(OpKind.NearBranch32, 4),
			new OpJ(OpKind.NearBranch32, 4),
			new OpJ(OpKind.NearBranch64, 4),
			new OpJx(2),
			new OpJx(4),
			new OpJdisp(2),
			new OpJdisp(4),
			new OpO(),
			new OpO(),
			new OpO(),
			new OpO(),
			new OpImm(1),
			new OpModRM_reg(Register.BND0, Register.BND3),
			new OpModRM_rm(Register.BND0, Register.BND3),
			new OpModRM_rm(Register.BND0, Register.BND3),
			new OpModRM_rm_mem_only(),
			new OpModRM_rm_reg_only(Register.MM0, Register.MM7),
			new OpModRM_reg(Register.MM0, Register.MM7),
			new OpModRM_rm(Register.MM0, Register.MM7),
			new OpModRM_rm_reg_only(Register.XMM0, Register.XMM15),
			new OpModRM_reg(Register.XMM0, Register.XMM15),
			new OpModRM_rm(Register.XMM0, Register.XMM15),
			new OprDI(),
			new OpMRBX(),
			new OpReg(Register.ES),
			new OpReg(Register.CS),
			new OpReg(Register.SS),
			new OpReg(Register.DS),
			new OpReg(Register.FS),
			new OpReg(Register.GS),
			new OpReg(Register.AL),
			new OpReg(Register.CL),
			new OpReg(Register.AX),
			new OpReg(Register.DX),
			new OpReg(Register.EAX),
			new OpReg(Register.RAX),
			new OpReg(Register.ST0),
			new OpRegSTi(),
			new OpRegEmbed8(Register.AL, Register.R15L),
			new OpRegEmbed8(Register.AX, Register.R15W),
			new OpRegEmbed8(Register.EAX, Register.R15D),
			new OpRegEmbed8(Register.RAX, Register.R15),
		};
	}

	enum VexOpKind : byte {
		None,
		Ed,
		Eq,
		Gd,
		Gq,
		RdMb,
		RqMb,
		RdMw,
		RqMw,
		Rd,
		Rq,
		Hd,
		Hq,
		HK,
		HX,
		HY,
		Ib,
		I2,
		Is4X,
		Is4Y,
		Is5X,
		Is5Y,
		M,
		Md,
		MK,
		rDI,
		RK,
		RX,
		RY,
		VK,
		VM32X,
		VM32Y,
		VM64X,
		VM64Y,
		VX,
		VY,
		WK,
		WX,
		WY,

		Last,
	}

	static class VexOps {
		public static readonly Op[] Ops = new Op[(int)VexOpKind.Last] {
			null!,// Never accessed
			new OpModRM_rm(Register.EAX, Register.R15D),
			new OpModRM_rm(Register.RAX, Register.R15),
			new OpModRM_reg(Register.EAX, Register.R15D),
			new OpModRM_reg(Register.RAX, Register.R15),
			new OpModRM_rm(Register.EAX, Register.R15D),
			new OpModRM_rm(Register.RAX, Register.R15),
			new OpModRM_rm(Register.EAX, Register.R15D),
			new OpModRM_rm(Register.RAX, Register.R15),
			new OpModRM_rm_reg_only(Register.EAX, Register.R15D),
			new OpModRM_rm_reg_only(Register.RAX, Register.R15),
			new OpHx(Register.EAX, Register.R15D),
			new OpHx(Register.RAX, Register.R15),
			new OpHx(Register.K0, Register.K7),
			new OpHx(Register.XMM0, Register.XMM15),
			new OpHx(Register.YMM0, Register.YMM15),
			new OpIb(OpKind.Immediate8),
			new OpI2(),
			new OpIs4x(Register.XMM0, Register.XMM15),
			new OpIs4x(Register.YMM0, Register.YMM15),
			new OpIs4x(Register.XMM0, Register.XMM15),
			new OpIs4x(Register.YMM0, Register.YMM15),
			new OpModRM_rm_mem_only(),
			new OpModRM_rm_mem_only(),
			new OpModRM_rm_mem_only(),
			new OprDI(),
			new OpModRM_rm_reg_only(Register.K0, Register.K7),
			new OpModRM_rm_reg_only(Register.XMM0, Register.XMM15),
			new OpModRM_rm_reg_only(Register.YMM0, Register.YMM15),
			new OpModRM_reg(Register.K0, Register.K7),
			new OpVMx(Register.XMM0, Register.XMM15),
			new OpVMx(Register.YMM0, Register.YMM15),
			new OpVMx(Register.XMM0, Register.XMM15),
			new OpVMx(Register.YMM0, Register.YMM15),
			new OpModRM_reg(Register.XMM0, Register.XMM15),
			new OpModRM_reg(Register.YMM0, Register.YMM15),
			new OpModRM_rm(Register.K0, Register.K7),
			new OpModRM_rm(Register.XMM0, Register.XMM15),
			new OpModRM_rm(Register.YMM0, Register.YMM15),
		};
	}

	enum XopOpKind : byte {
		None,
		Ed,
		Eq,
		Gd,
		Gq,
		Rd,
		Rq,
		Hd,
		Hq,
		HX,
		HY,
		Ib,
		Id,
		Is4X,
		Is4Y,
		VX,
		VY,
		WX,
		WY,

		Last,
	}

	static class XopOps {
		public static readonly Op[] Ops = new Op[(int)XopOpKind.Last] {
			null!,// Never accessed
			new OpModRM_rm(Register.EAX, Register.R15D),
			new OpModRM_rm(Register.RAX, Register.R15),
			new OpModRM_reg(Register.EAX, Register.R15D),
			new OpModRM_reg(Register.RAX, Register.R15),
			new OpModRM_rm_reg_only(Register.EAX, Register.R15D),
			new OpModRM_rm_reg_only(Register.RAX, Register.R15),
			new OpHx(Register.EAX, Register.R15D),
			new OpHx(Register.RAX, Register.R15),
			new OpHx(Register.XMM0, Register.XMM15),
			new OpHx(Register.YMM0, Register.YMM15),
			new OpIb(OpKind.Immediate8),
			new OpId(OpKind.Immediate32),
			new OpIs4x(Register.XMM0, Register.XMM15),
			new OpIs4x(Register.YMM0, Register.YMM15),
			new OpModRM_reg(Register.XMM0, Register.XMM15),
			new OpModRM_reg(Register.YMM0, Register.YMM15),
			new OpModRM_rm(Register.XMM0, Register.XMM15),
			new OpModRM_rm(Register.YMM0, Register.YMM15),
		};
	}

	enum EvexOpKind : byte {
		None,
		Ed,
		Eq,
		Gd,
		Gq,
		RdMb,
		RqMb,
		RdMw,
		RqMw,
		HX,
		HY,
		HZ,
		HXP3,
		HZP3,
		Ib,
		M,
		Rd,
		Rq,
		RX,
		RY,
		RZ,
		RK,
		VM32X,
		VM32Y,
		VM32Z,
		VM64X,
		VM64Y,
		VM64Z,
		VK,
		VKP1,
		VX,
		VY,
		VZ,
		WX,
		WY,
		WZ,

		Last,
	}

	static class EvexOps {
		public static readonly Op[] Ops = new Op[(int)EvexOpKind.Last] {
			null!,// Never accessed
			new OpModRM_rm(Register.EAX, Register.R15D),
			new OpModRM_rm(Register.RAX, Register.R15),
			new OpModRM_reg(Register.EAX, Register.R15D),
			new OpModRM_reg(Register.RAX, Register.R15),
			new OpModRM_rm(Register.EAX, Register.R15D),
			new OpModRM_rm(Register.RAX, Register.R15),
			new OpModRM_rm(Register.EAX, Register.R15D),
			new OpModRM_rm(Register.RAX, Register.R15),
			new OpHx(Register.XMM0, Register.XMM31),
			new OpHx(Register.YMM0, Register.YMM31),
			new OpHx(Register.ZMM0, Register.ZMM31),
			new OpHx(Register.XMM0, Register.XMM31),
			new OpHx(Register.ZMM0, Register.ZMM31),
			new OpIb(OpKind.Immediate8),
			new OpModRM_rm_mem_only(),
			new OpModRM_rm_reg_only(Register.EAX, Register.R15D),
			new OpModRM_rm_reg_only(Register.RAX, Register.R15),
			new OpModRM_rm_reg_only(Register.XMM0, Register.XMM31),
			new OpModRM_rm_reg_only(Register.YMM0, Register.YMM31),
			new OpModRM_rm_reg_only(Register.ZMM0, Register.ZMM31),
			new OpModRM_rm_reg_only(Register.K0, Register.K7),
			new OpVMx(Register.XMM0, Register.XMM31),
			new OpVMx(Register.YMM0, Register.YMM31),
			new OpVMx(Register.ZMM0, Register.ZMM31),
			new OpVMx(Register.XMM0, Register.XMM31),
			new OpVMx(Register.YMM0, Register.YMM31),
			new OpVMx(Register.ZMM0, Register.ZMM31),
			new OpModRM_reg(Register.K0, Register.K7),
			new OpModRM_reg(Register.K0, Register.K7),
			new OpModRM_reg(Register.XMM0, Register.XMM31),
			new OpModRM_reg(Register.YMM0, Register.YMM31),
			new OpModRM_reg(Register.ZMM0, Register.ZMM31),
			new OpModRM_rm(Register.XMM0, Register.XMM31),
			new OpModRM_rm(Register.YMM0, Register.YMM31),
			new OpModRM_rm(Register.ZMM0, Register.ZMM31),
		};
	}

	abstract class Op {
		public abstract void Encode(Encoder encoder, in Instruction instr, int operand);

		/// <summary>
		/// If this is an immediate operand, it returns the <see cref="OpKind"/> value, else it returns -1
		/// </summary>
		/// <returns></returns>
		public virtual OpKind GetImmediateOpKind() => (OpKind)(-1);

		/// <summary>
		/// If this is a near branch operand, it returns the <see cref="OpKind"/> value, else it returns -1
		/// </summary>
		/// <returns></returns>
		public virtual OpKind GetNearBranchOpKind() => (OpKind)(-1);

		/// <summary>
		/// If this is a far branch operand, it returns the <see cref="OpKind"/> value, else it returns -1
		/// </summary>
		/// <returns></returns>
		public virtual OpKind GetFarBranchOpKind() => (OpKind)(-1);
	}

	sealed class OpModRM_rm_mem_only : Op {
		public override void Encode(Encoder encoder, in Instruction instr, int operand) =>
			encoder.AddRegOrMem(instr, operand, Register.None, Register.None, allowMemOp: true, allowRegOp: false);
	}

	sealed class OpModRM_rm : Op {
		readonly Register regLo;
		readonly Register regHi;

		public OpModRM_rm(Register regLo, Register regHi) {
			this.regLo = regLo;
			this.regHi = regHi;
		}

		public override void Encode(Encoder encoder, in Instruction instr, int operand) =>
			encoder.AddRegOrMem(instr, operand, regLo, regHi, allowMemOp: true, allowRegOp: true);
	}

	sealed class OpRegEmbed8: Op {
		readonly Register regLo;
		readonly Register regHi;

		public OpRegEmbed8(Register regLo, Register regHi) {
			this.regLo = regLo;
			this.regHi = regHi;
		}

		public override void Encode(Encoder encoder, in Instruction instr, int operand) =>
			encoder.AddReg(instr, operand, regLo, regHi);
	}

	sealed class OpModRM_rm_reg_only : Op {
		readonly Register regLo;
		readonly Register regHi;

		public OpModRM_rm_reg_only(Register regLo, Register regHi) {
			this.regLo = regLo;
			this.regHi = regHi;
		}

		public override void Encode(Encoder encoder, in Instruction instr, int operand) =>
			encoder.AddRegOrMem(instr, operand, regLo, regHi, allowMemOp: false, allowRegOp: true);
	}

	sealed class OpModRM_reg : Op {
		readonly Register regLo;
		readonly Register regHi;

		public OpModRM_reg(Register regLo, Register regHi) {
			this.regLo = regLo;
			this.regHi = regHi;
		}

		public override void Encode(Encoder encoder, in Instruction instr, int operand) =>
			encoder.AddModRMRegister(instr, operand, regLo, regHi);
	}

	sealed class OpModRM_regF0 : Op {
		readonly Register regLo;
		readonly Register regHi;

		public OpModRM_regF0(Register regLo, Register regHi) {
			this.regLo = regLo;
			this.regHi = regHi;
		}

		public override void Encode(Encoder encoder, in Instruction instr, int operand) {
			if (encoder.Bitness != 64 && instr.GetOpKind(operand) == OpKind.Register && instr.GetOpRegister(operand) == regLo + 8) {
				encoder.EncoderFlags |= EncoderFlags.PF0;
				encoder.AddModRMRegister(instr, operand, regLo + 8, regLo + 8);
			}
			else
				encoder.AddModRMRegister(instr, operand, regLo, regHi);
		}
	}

	sealed class OpReg : Op {
		readonly Register register;

		public OpReg(Register register) => this.register = register;

		public override void Encode(Encoder encoder, in Instruction instr, int operand) {
			encoder.Verify(operand, OpKind.Register, instr.GetOpKind(operand));
			encoder.Verify(operand, register, instr.GetOpRegister(operand));
		}
	}

	sealed class OpRegSTi : Op {
		public override void Encode(Encoder encoder, in Instruction instr, int operand) {
			if (!encoder.Verify(operand, OpKind.Register, instr.GetOpKind(operand)))
				return;
			var reg = instr.GetOpRegister(operand);
			if (!encoder.Verify(operand, reg, Register.ST0, Register.ST7))
				return;
			Debug.Assert((encoder.OpCode & 7) == 0);
			encoder.OpCode |= (uint)(reg - Register.ST0);
		}
	}

	sealed class OprDI : Op {
		static int GetRegSize(OpKind opKind) {
			if (opKind == OpKind.MemorySegRDI)
				return 8;
			if (opKind == OpKind.MemorySegEDI)
				return 4;
			if (opKind == OpKind.MemorySegDI)
				return 2;
			return 0;
		}

		public override void Encode(Encoder encoder, in Instruction instr, int operand) {
			var regSize = GetRegSize(instr.GetOpKind(operand));
			if (regSize == 0) {
				encoder.ErrorMessage = $"Operand {operand}: expected OpKind = {nameof(OpKind.MemorySegDI)}, {nameof(OpKind.MemorySegEDI)} or {nameof(OpKind.MemorySegRDI)}";
				return;
			}
			encoder.SetAddrSize(regSize);
		}
	}

	sealed class OpIb : Op {
		readonly OpKind opKind;

		public OpIb(OpKind opKind) => this.opKind = opKind;

		public override void Encode(Encoder encoder, in Instruction instr, int operand) {
			var opImmKind = instr.GetOpKind(operand);
			if (!encoder.Verify(operand, opKind, opImmKind))
				return;
			encoder.ImmSize = ImmSize.Size1;
			encoder.Immediate = instr.Immediate8;
		}

		public override OpKind GetImmediateOpKind() => opKind;
	}

	sealed class OpIw : Op {
		public override void Encode(Encoder encoder, in Instruction instr, int operand) {
			if (!encoder.Verify(operand, OpKind.Immediate16, instr.GetOpKind(operand)))
				return;
			encoder.ImmSize = ImmSize.Size2;
			encoder.Immediate = instr.Immediate16;
		}

		public override OpKind GetImmediateOpKind() => OpKind.Immediate16;
	}

	sealed class OpId : Op {
		readonly OpKind opKind;

		public OpId(OpKind opKind) => this.opKind = opKind;

		public override void Encode(Encoder encoder, in Instruction instr, int operand) {
			var opImmKind = instr.GetOpKind(operand);
			if (!encoder.Verify(operand, opKind, opImmKind))
				return;
			encoder.ImmSize = ImmSize.Size4;
			encoder.Immediate = instr.Immediate32;
		}

		public override OpKind GetImmediateOpKind() => opKind;
	}

	sealed class OpIq : Op {
		public override void Encode(Encoder encoder, in Instruction instr, int operand) {
			if (!encoder.Verify(operand, OpKind.Immediate64, instr.GetOpKind(operand)))
				return;
			encoder.ImmSize = ImmSize.Size8;
			ulong imm = instr.Immediate64;
			encoder.Immediate = (uint)imm;
			encoder.ImmediateHi = (uint)(imm >> 32);
		}

		public override OpKind GetImmediateOpKind() => OpKind.Immediate64;
	}

	sealed class OpIb21 : Op {
		public override void Encode(Encoder encoder, in Instruction instr, int operand) {
			if (!encoder.Verify(operand, OpKind.Immediate8_2nd, instr.GetOpKind(operand)))
				return;
			Debug.Assert(encoder.ImmSize == ImmSize.Size2);
			encoder.ImmSize = ImmSize.Size2_1;
			encoder.ImmediateHi = instr.Immediate8_2nd;
		}

		public override OpKind GetImmediateOpKind() => OpKind.Immediate8_2nd;
	}

	sealed class OpIb11 : Op {
		public override void Encode(Encoder encoder, in Instruction instr, int operand) {
			if (!encoder.Verify(operand, OpKind.Immediate8_2nd, instr.GetOpKind(operand)))
				return;
			Debug.Assert(encoder.ImmSize == ImmSize.Size1);
			encoder.ImmSize = ImmSize.Size1_1;
			encoder.ImmediateHi = instr.Immediate8_2nd;
		}

		public override OpKind GetImmediateOpKind() => OpKind.Immediate8_2nd;
	}

	sealed class OpI2 : Op {
		public override void Encode(Encoder encoder, in Instruction instr, int operand) {
			var opImmKind = instr.GetOpKind(operand);
			if (!encoder.Verify(operand, OpKind.Immediate8, opImmKind))
				return;
			Debug.Assert(encoder.ImmSize == ImmSize.SizeIbReg);
			Debug.Assert((encoder.Immediate & 3) == 0);
			if (instr.Immediate8 > 3) {
				encoder.ErrorMessage = $"Operand {operand}: Immediate value must be 0-3, but value is 0x{instr.Immediate8:X2}";
				return;
			}
			encoder.ImmSize = ImmSize.Size1;
			encoder.Immediate |= instr.Immediate8;
		}

		public override OpKind GetImmediateOpKind() => OpKind.Immediate8;
	}

	sealed class OpX : Op {
		internal static int GetXRegSize(OpKind opKind) {
			if (opKind == OpKind.MemorySegRSI)
				return 8;
			if (opKind == OpKind.MemorySegESI)
				return 4;
			if (opKind == OpKind.MemorySegSI)
				return 2;
			return 0;
		}

		internal static int GetYRegSize(OpKind opKind) {
			if (opKind == OpKind.MemoryESRDI)
				return 8;
			if (opKind == OpKind.MemoryESEDI)
				return 4;
			if (opKind == OpKind.MemoryESDI)
				return 2;
			return 0;
		}

		public override void Encode(Encoder encoder, in Instruction instr, int operand) {
			var regXSize = GetXRegSize(instr.GetOpKind(operand));
			if (regXSize == 0) {
				encoder.ErrorMessage = $"Operand {operand}: expected OpKind = {nameof(OpKind.MemorySegSI)}, {nameof(OpKind.MemorySegESI)} or {nameof(OpKind.MemorySegRSI)}";
				return;
			}
			switch (instr.Code) {
			case Code.Movsb_m8_m8:
			case Code.Movsw_m16_m16:
			case Code.Movsd_m32_m32:
			case Code.Movsq_m64_m64:
				var regYSize = GetYRegSize(instr.Op0Kind);
				if (regXSize != regYSize) {
					encoder.ErrorMessage = $"Same sized register must be used: reg #1 size = {regYSize * 8}, reg #2 size = {regXSize * 8}";
					return;
				}
				break;
			}
			encoder.SetAddrSize(regXSize);
		}
	}

	sealed class OpY : Op {
		public override void Encode(Encoder encoder, in Instruction instr, int operand) {
			var regYSize = OpX.GetYRegSize(instr.GetOpKind(operand));
			if (regYSize == 0) {
				encoder.ErrorMessage = $"Operand {operand}: expected OpKind = {nameof(OpKind.MemoryESDI)}, {nameof(OpKind.MemoryESEDI)} or {nameof(OpKind.MemoryESRDI)}";
				return;
			}
			switch (instr.Code) {
			case Code.Cmpsb_m8_m8:
			case Code.Cmpsw_m16_m16:
			case Code.Cmpsd_m32_m32:
			case Code.Cmpsq_m64_m64:
				var regXSize = OpX.GetXRegSize(instr.Op0Kind);
				if (regXSize != regYSize) {
					encoder.ErrorMessage = $"Same sized register must be used: reg #1 size = {regXSize * 8}, reg #2 size = {regYSize * 8}";
					return;
				}
				break;
			}
			encoder.SetAddrSize(regYSize);
		}
	}

	sealed class OpMRBX : Op {
		public override void Encode(Encoder encoder, in Instruction instr, int operand) {
			if (!encoder.Verify(operand, OpKind.Memory, instr.GetOpKind(operand)))
				return;
			var baseReg = instr.MemoryBase;
			if (instr.MemoryDisplSize != 0 || instr.MemoryIndex != Register.AL || (baseReg != Register.BX && baseReg != Register.EBX && baseReg != Register.RBX)) {
				encoder.ErrorMessage = $"Operand {operand}: Operand must be [bx+al], [ebx+al], or [rbx+al]";
				return;
			}
			int regSize;
			if (baseReg == Register.RBX)
				regSize = 8;
			else if (baseReg == Register.EBX)
				regSize = 4;
			else {
				Debug.Assert(baseReg == Register.BX);
				regSize = 2;
			}
			encoder.SetAddrSize(regSize);
		}
	}

	sealed class OpJ : Op {
		readonly OpKind opKind;
		readonly int immSize;

		public OpJ(OpKind opKind, int immSize) {
			this.opKind = opKind;
			this.immSize = immSize;
		}

		public override void Encode(Encoder encoder, in Instruction instr, int operand) =>
			encoder.AddBranch(opKind, immSize, instr, operand);

		public override OpKind GetNearBranchOpKind() => opKind;
	}

	sealed class OpJx : Op {
		readonly int immSize;

		public OpJx(int immSize) => this.immSize = immSize;

		public override void Encode(Encoder encoder, in Instruction instr, int operand) =>
			encoder.AddBranchX(immSize, instr, operand);

		public override OpKind GetNearBranchOpKind() {
			// xbegin is special and doesn't mask the target IP. We need to know the code size to return the correct value
			return base.GetNearBranchOpKind();
		}
	}

	sealed class OpJdisp : Op {
		readonly int displSize;

		public OpJdisp(int displSize) => this.displSize = displSize;

		public override void Encode(Encoder encoder, in Instruction instr, int operand) =>
			encoder.AddBranchDisp(displSize, instr, operand);

		public override OpKind GetNearBranchOpKind() => displSize == 2 ? OpKind.NearBranch16 : OpKind.NearBranch32;
	}

	sealed class OpA : Op {
		readonly int size;

		public OpA(int size) {
			Debug.Assert(size == 2 || size == 4);
			this.size = size;
		}

		public override void Encode(Encoder encoder, in Instruction instr, int operand) =>
			encoder.AddFarBranch(instr, operand, size);

		public override OpKind GetFarBranchOpKind() {
			Debug.Assert(size == 2 || size == 4);
			return size == 2 ? OpKind.FarBranch16 : OpKind.FarBranch32;
		}
	}

	sealed class OpO : Op {
		public override void Encode(Encoder encoder, in Instruction instr, int operand) =>
			encoder.AddAbsMem(instr, operand);
	}

	sealed class OpImm : Op {
		public byte value;

		public OpImm(byte value) => this.value = value;

		public override void Encode(Encoder encoder, in Instruction instr, int operand) {
			if (!encoder.Verify(operand, OpKind.Immediate8, instr.GetOpKind(operand)))
				return;
			if (instr.Immediate8 != value) {
				encoder.ErrorMessage = $"Operand {operand}: Expected 0x{value:X2}, actual: 0x{instr.Immediate8:X2}";
				return;
			}
		}

		public override OpKind GetImmediateOpKind() => OpKind.Immediate8;
	}

	sealed class OpHx : Op {
		readonly Register regLo;
		readonly Register regHi;

		public OpHx(Register regLo, Register regHi) {
			this.regLo = regLo;
			this.regHi = regHi;
		}

		public override void Encode(Encoder encoder, in Instruction instr, int operand) {
			if (!encoder.Verify(operand, OpKind.Register, instr.GetOpKind(operand)))
				return;
			var reg = instr.GetOpRegister(operand);
			if (!encoder.Verify(operand, reg, regLo, regHi))
				return;
			encoder.EncoderFlags |= (EncoderFlags)((uint)(reg - regLo) << (int)EncoderFlags.VvvvvShift);
		}
	}

	sealed class OpVMx : Op {
		readonly Register vsibIndexRegLo;
		readonly Register vsibIndexRegHi;

		public OpVMx(Register regLo, Register regHi) {
			vsibIndexRegLo = regLo;
			vsibIndexRegHi = regHi;
		}

		public override void Encode(Encoder encoder, in Instruction instr, int operand) =>
			encoder.AddRegOrMem(instr, operand, Register.None, Register.None, vsibIndexRegLo, vsibIndexRegHi, allowMemOp: true, allowRegOp: false);
	}

	sealed class OpIs4x : Op {
		readonly Register regLo;
		readonly Register regHi;

		public OpIs4x(Register regLo, Register regHi) {
			this.regLo = regLo;
			this.regHi = regHi;
		}

		public override void Encode(Encoder encoder, in Instruction instr, int operand) {
			if (!encoder.Verify(operand, OpKind.Register, instr.GetOpKind(operand)))
				return;
			var reg = instr.GetOpRegister(operand);
			if (!encoder.Verify(operand, reg, regLo, regHi))
				return;
			encoder.ImmSize = ImmSize.SizeIbReg;
			encoder.Immediate = (uint)(reg - regLo) << 4;
		}
	}
}
#endif
