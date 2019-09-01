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

#if !NO_GAS_FORMATTER && !NO_FORMATTER
using System;
using System.Diagnostics;

namespace Iced.Intel.GasFormatterInternal {
	enum InstrOpKind : byte {
		Register = OpKind.Register,
		NearBranch16 = OpKind.NearBranch16,
		NearBranch32 = OpKind.NearBranch32,
		NearBranch64 = OpKind.NearBranch64,
		FarBranch16 = OpKind.FarBranch16,
		FarBranch32 = OpKind.FarBranch32,
		Immediate8 = OpKind.Immediate8,
		Immediate8_2nd = OpKind.Immediate8_2nd,
		Immediate16 = OpKind.Immediate16,
		Immediate32 = OpKind.Immediate32,
		Immediate64 = OpKind.Immediate64,
		Immediate8to16 = OpKind.Immediate8to16,
		Immediate8to32 = OpKind.Immediate8to32,
		Immediate8to64 = OpKind.Immediate8to64,
		Immediate32to64 = OpKind.Immediate32to64,
		MemorySegSI = OpKind.MemorySegSI,
		MemorySegESI = OpKind.MemorySegESI,
		MemorySegRSI = OpKind.MemorySegRSI,
		MemorySegDI = OpKind.MemorySegDI,
		MemorySegEDI = OpKind.MemorySegEDI,
		MemorySegRDI = OpKind.MemorySegRDI,
		MemoryESDI = OpKind.MemoryESDI,
		MemoryESEDI = OpKind.MemoryESEDI,
		MemoryESRDI = OpKind.MemoryESRDI,
		Memory64 = OpKind.Memory64,
		Memory = OpKind.Memory,

		// Extra opkinds
		Sae,
		RnSae,
		RdSae,
		RuSae,
		RzSae,
		DeclareByte,
		DeclareWord,
		DeclareDword,
		DeclareQword,
	}

	struct InstrOpInfo {
		internal const int TEST_RegisterBits = 8;

		public string Mnemonic;
		public InstrOpInfoFlags Flags;
		public byte OpCount;
		public InstrOpKind Op0Kind;
		public InstrOpKind Op1Kind;
		public InstrOpKind Op2Kind;
		public InstrOpKind Op3Kind;
		public InstrOpKind Op4Kind;
		public byte Op0Register;
		public byte Op1Register;
		public byte Op2Register;
		public byte Op3Register;
		public byte Op4Register;
		public sbyte Op0Index;
		public sbyte Op1Index;
		public sbyte Op2Index;
		public sbyte Op3Index;
		public sbyte Op4Index;

		public readonly Register GetOpRegister(int operand) {
			switch (operand) {
			case 0: return (Register)Op0Register;
			case 1: return (Register)Op1Register;
			case 2: return (Register)Op2Register;
			case 3: return (Register)Op3Register;
			case 4: return (Register)Op4Register;
			default: throw new ArgumentOutOfRangeException(nameof(operand));
			}
		}

		public readonly InstrOpKind GetOpKind(int operand) {
			switch (operand) {
			case 0: return Op0Kind;
			case 1: return Op1Kind;
			case 2: return Op2Kind;
			case 3: return Op3Kind;
			case 4: return Op4Kind;
			default:
				Debug.Assert(Op0Kind == InstrOpKind.DeclareByte || Op0Kind == InstrOpKind.DeclareWord || Op0Kind == InstrOpKind.DeclareDword || Op0Kind == InstrOpKind.DeclareQword);
				return Op0Kind;
			}
		}

		public readonly int GetInstructionIndex(int operand) {
			int instructionOperand;
			switch (operand) {
			case 0: instructionOperand = Op0Index; break;
			case 1: instructionOperand = Op1Index; break;
			case 2: instructionOperand = Op2Index; break;
			case 3: instructionOperand = Op3Index; break;
			case 4: instructionOperand = Op4Index; break;
			default:
				Debug.Assert(Op0Kind == InstrOpKind.DeclareByte || Op0Kind == InstrOpKind.DeclareWord || Op0Kind == InstrOpKind.DeclareDword || Op0Kind == InstrOpKind.DeclareQword);
				instructionOperand = -1;
				break;
			}
			return instructionOperand < 0 ? -1 : instructionOperand;
		}

#if !NO_INSTR_INFO
		public readonly bool TryGetOpAccess(int operand, out OpAccess access) {
			int instructionOperand;
			switch (operand) {
			case 0: instructionOperand = Op0Index; break;
			case 1: instructionOperand = Op1Index; break;
			case 2: instructionOperand = Op2Index; break;
			case 3: instructionOperand = Op3Index; break;
			case 4: instructionOperand = Op4Index; break;
			default:
				Debug.Assert(Op0Kind == InstrOpKind.DeclareByte || Op0Kind == InstrOpKind.DeclareWord || Op0Kind == InstrOpKind.DeclareDword || Op0Kind == InstrOpKind.DeclareQword);
				instructionOperand = Op0Index;
				break;
			}
			if (instructionOperand < InstrInfo.OpAccess_INVALID) {
				access = (OpAccess)(-instructionOperand - 2);
				return true;
			}
			access = OpAccess.None;
			return false;
		}
#endif

		public readonly int GetOperandIndex(int instructionOperand) {
			int index;
			if (instructionOperand == Op0Index)
				index = 0;
			else if (instructionOperand == Op1Index)
				index = 1;
			else if (instructionOperand == Op2Index)
				index = 2;
			else if (instructionOperand == Op3Index)
				index = 3;
			else if (instructionOperand == Op4Index)
				index = 4;
			else
				index = -1;
			return index < OpCount ? index : -1;
		}

		public InstrOpInfo(string mnemonic, in Instruction instr, InstrOpInfoFlags flags) {
			Debug.Assert(DecoderConstants.MaxOpCount == 5);
			Mnemonic = mnemonic;
			Flags = flags;
			int opCount = instr.OpCount;
			OpCount = (byte)opCount;
			if ((flags & InstrOpInfoFlags.KeepOperandOrder) != 0) {
				Op0Kind = (InstrOpKind)instr.Op0Kind;
				Op1Kind = (InstrOpKind)instr.Op1Kind;
				Op2Kind = (InstrOpKind)instr.Op2Kind;
				Op3Kind = (InstrOpKind)instr.Op3Kind;
				Op4Kind = (InstrOpKind)instr.Op4Kind;
				Op0Register = (byte)instr.Op0Register;
				Op1Register = (byte)instr.Op1Register;
				Op2Register = (byte)instr.Op2Register;
				Op3Register = (byte)instr.Op3Register;
				Op4Register = (byte)instr.Op4Register;
			}
			else {
				switch (opCount) {
				case 0:
					Op0Kind = 0;
					Op1Kind = 0;
					Op2Kind = 0;
					Op3Kind = 0;
					Op4Kind = 0;
					Op0Register = 0;
					Op1Register = 0;
					Op2Register = 0;
					Op3Register = 0;
					Op4Register = 0;
					break;

				case 1:
					Op0Kind = (InstrOpKind)instr.Op0Kind;
					Op1Kind = 0;
					Op2Kind = 0;
					Op3Kind = 0;
					Op4Kind = 0;
					Op0Register = (byte)instr.Op0Register;
					Op1Register = 0;
					Op2Register = 0;
					Op3Register = 0;
					Op4Register = 0;
					break;

				case 2:
					Op0Kind = (InstrOpKind)instr.Op1Kind;
					Op1Kind = (InstrOpKind)instr.Op0Kind;
					Op2Kind = 0;
					Op3Kind = 0;
					Op4Kind = 0;
					Op0Register = (byte)instr.Op1Register;
					Op1Register = (byte)instr.Op0Register;
					Op2Register = 0;
					Op3Register = 0;
					Op4Register = 0;
					break;

				case 3:
					Op0Kind = (InstrOpKind)instr.Op2Kind;
					Op1Kind = (InstrOpKind)instr.Op1Kind;
					Op2Kind = (InstrOpKind)instr.Op0Kind;
					Op3Kind = 0;
					Op4Kind = 0;
					Op0Register = (byte)instr.Op2Register;
					Op1Register = (byte)instr.Op1Register;
					Op2Register = (byte)instr.Op0Register;
					Op3Register = 0;
					Op4Register = 0;
					break;

				case 4:
					Op0Kind = (InstrOpKind)instr.Op3Kind;
					Op1Kind = (InstrOpKind)instr.Op2Kind;
					Op2Kind = (InstrOpKind)instr.Op1Kind;
					Op3Kind = (InstrOpKind)instr.Op0Kind;
					Op4Kind = 0;
					Op0Register = (byte)instr.Op3Register;
					Op1Register = (byte)instr.Op2Register;
					Op2Register = (byte)instr.Op1Register;
					Op3Register = (byte)instr.Op0Register;
					Op4Register = 0;
					break;

				case 5:
					Op0Kind = (InstrOpKind)instr.Op4Kind;
					Op1Kind = (InstrOpKind)instr.Op3Kind;
					Op2Kind = (InstrOpKind)instr.Op2Kind;
					Op3Kind = (InstrOpKind)instr.Op1Kind;
					Op4Kind = (InstrOpKind)instr.Op0Kind;;
					Op0Register = (byte)instr.Op4Register;
					Op1Register = (byte)instr.Op3Register;
					Op2Register = (byte)instr.Op2Register;
					Op3Register = (byte)instr.Op1Register;
					Op4Register = (byte)instr.Op0Register;
					break;

				default:
					throw new InvalidOperationException();
				}
			}
			switch (OpCount) {
			case 0:
				Op0Index = InstrInfo.OpAccess_INVALID;
				Op1Index = InstrInfo.OpAccess_INVALID;
				Op2Index = InstrInfo.OpAccess_INVALID;
				Op3Index = InstrInfo.OpAccess_INVALID;
				Op4Index = InstrInfo.OpAccess_INVALID;
				break;

			case 1:
				Op0Index = 0;
				Op1Index = InstrInfo.OpAccess_INVALID;
				Op2Index = InstrInfo.OpAccess_INVALID;
				Op3Index = InstrInfo.OpAccess_INVALID;
				Op4Index = InstrInfo.OpAccess_INVALID;
				break;

			case 2:
				Op0Index = 1;
				Op1Index = 0;
				Op2Index = InstrInfo.OpAccess_INVALID;
				Op3Index = InstrInfo.OpAccess_INVALID;
				Op4Index = InstrInfo.OpAccess_INVALID;
				break;

			case 3:
				Op0Index = 2;
				Op1Index = 1;
				Op2Index = 0;
				Op3Index = InstrInfo.OpAccess_INVALID;
				Op4Index = InstrInfo.OpAccess_INVALID;
				break;

			case 4:
				Op0Index = 3;
				Op1Index = 2;
				Op2Index = 1;
				Op3Index = 0;
				Op4Index = InstrInfo.OpAccess_INVALID;
				break;

			case 5:
				Op0Index = 4;
				Op1Index = 3;
				Op2Index = 2;
				Op3Index = 1;
				Op4Index = 0;
				break;

			default:
				throw new InvalidOperationException();
			}
		}
	}

	abstract class InstrInfo {
		public const int OpAccess_INVALID = -1;
#if !NO_INSTR_INFO
		public const int OpAccess_None = -(int)(OpAccess.None + 2);
		public const int OpAccess_Read = -(int)(OpAccess.Read + 2);
		public const int OpAccess_CondRead = -(int)(OpAccess.CondRead + 2);
		public const int OpAccess_Write = -(int)(OpAccess.Write + 2);
		public const int OpAccess_CondWrite = -(int)(OpAccess.CondWrite + 2);
		public const int OpAccess_ReadWrite = -(int)(OpAccess.ReadWrite + 2);
		public const int OpAccess_ReadCondWrite = -(int)(OpAccess.ReadCondWrite + 2);
		public const int OpAccess_NoMemAccess = -(int)(OpAccess.NoMemAccess + 2);
#else
		public const int OpAccess_None = OpAccess_INVALID;
		public const int OpAccess_Read = OpAccess_INVALID;
		public const int OpAccess_CondRead = OpAccess_INVALID;
		public const int OpAccess_Write = OpAccess_INVALID;
		public const int OpAccess_CondWrite = OpAccess_INVALID;
		public const int OpAccess_ReadWrite = OpAccess_INVALID;
		public const int OpAccess_ReadCondWrite = OpAccess_INVALID;
		public const int OpAccess_NoMemAccess = OpAccess_INVALID;
#endif

		internal readonly Code TEST_Code;
		protected InstrInfo(Code code) => TEST_Code = code;

		public abstract void GetOpInfo(GasFormatterOptions options, in Instruction instr, out InstrOpInfo info);

		protected static int GetCodeSize(CodeSize codeSize) {
			switch (codeSize) {
			case CodeSize.Code16:	return 16;
			case CodeSize.Code32:	return 32;
			case CodeSize.Code64:	return 64;
			default:
			case CodeSize.Unknown:	return 0;
			}
		}

		protected static string GetMnemonic(GasFormatterOptions options, in Instruction instr, string mnemonic, string mnemonic_suffix, InstrOpInfoFlags flags) {
			if (options.ShowMnemonicSizeSuffix)
				return mnemonic_suffix;
			if ((flags & InstrOpInfoFlags.MnemonicSuffixIfMem) != 0 && MemorySizes.AllMemorySizes[(int)instr.MemorySize].bcstTo is null) {
				OpKind opKind;
				if ((opKind = instr.Op0Kind) == OpKind.Memory || opKind == OpKind.Memory64 ||
					(opKind = instr.Op1Kind) == OpKind.Memory || opKind == OpKind.Memory64 ||
					instr.Op2Kind == OpKind.Memory)
					return mnemonic_suffix;
			}
			return mnemonic;
		}
	}

	sealed class SimpleInstrInfo : InstrInfo {
		readonly string mnemonic;
		readonly string mnemonic_suffix;
		readonly InstrOpInfoFlags flags;

		public SimpleInstrInfo(Code code, string mnemonic) : this(code, mnemonic, mnemonic, InstrOpInfoFlags.None) { }
		public SimpleInstrInfo(Code code, string mnemonic, InstrOpInfoFlags flags) : this(code, mnemonic, mnemonic, flags) { }
		public SimpleInstrInfo(Code code, string mnemonic, string mnemonic_suffix) : this(code, mnemonic, mnemonic_suffix, InstrOpInfoFlags.None) { }

		public SimpleInstrInfo(Code code, string mnemonic, string mnemonic_suffix, InstrOpInfoFlags flags)
			: base(code) {
			this.mnemonic = mnemonic;
			this.mnemonic_suffix = mnemonic_suffix;
			this.flags = flags;
		}

		public override void GetOpInfo(GasFormatterOptions options, in Instruction instr, out InstrOpInfo info) =>
			info = new InstrOpInfo(GetMnemonic(options, instr, mnemonic, mnemonic_suffix, flags), instr, flags);
	}

	sealed class SimpleInstrInfo_AamAad : InstrInfo {
		readonly string mnemonic;

		public SimpleInstrInfo_AamAad(Code code, string mnemonic) : base(code) => this.mnemonic = mnemonic;

		public override void GetOpInfo(GasFormatterOptions options, in Instruction instr, out InstrOpInfo info) {
			if (instr.Immediate8 == 10) {
				info = default;
				info.Mnemonic = mnemonic;
			}
			else
				info = new InstrOpInfo(mnemonic, instr, InstrOpInfoFlags.None);
		}
	}

	sealed class SimpleInstrInfo_nop : InstrInfo {
		readonly int codeSize;
		readonly string mnemonic;
		readonly Register register;

		public SimpleInstrInfo_nop(Code code, int codeSize, string mnemonic, Register register)
			: base(code) {
			this.codeSize = codeSize;
			this.mnemonic = mnemonic;
			this.register = register;
		}

		public override void GetOpInfo(GasFormatterOptions options, in Instruction instr, out InstrOpInfo info) {
			int instrCodeSize = GetCodeSize(instr.CodeSize);
			if (instrCodeSize == 0 || (instrCodeSize & codeSize) != 0)
				info = new InstrOpInfo(mnemonic, instr, InstrOpInfoFlags.None);
			else {
				info = default;
				if (!options.ShowMnemonicSizeSuffix)
					info.Mnemonic = "xchg";
				else if (register == Register.AX)
					info.Mnemonic = "xchgw";
				else if (register == Register.EAX)
					info.Mnemonic = "xchgl";
				else if (register == Register.RAX)
					info.Mnemonic = "xchgq";
				else
					throw new InvalidOperationException();
				info.OpCount = 2;
				Debug.Assert(InstrOpKind.Register == 0);
				//info.Op0Kind = InstrOpKind.Register;
				//info.Op1Kind = InstrOpKind.Register;
				Debug.Assert(InstrOpInfo.TEST_RegisterBits == 8);
				info.Op0Register = (byte)register;
				Debug.Assert(InstrOpInfo.TEST_RegisterBits == 8);
				info.Op1Register = (byte)register;
				if (instr.Op0Register == instr.Op1Register) {
					info.Op0Index = OpAccess_None;
					info.Op1Index = OpAccess_None;
				}
				else {
					info.Op0Index = OpAccess_ReadWrite;
					info.Op1Index = OpAccess_ReadWrite;
				}
			}
		}
	}

	sealed class SimpleInstrInfo_STIG1 : InstrInfo {
		readonly string mnemonic;
		readonly bool pseudoOp;

		public SimpleInstrInfo_STIG1(Code code, string mnemonic) : this(code, mnemonic, false) { }

		public SimpleInstrInfo_STIG1(Code code, string mnemonic, bool pseudoOp)
			: base(code) {
			this.mnemonic = mnemonic;
			this.pseudoOp = pseudoOp;
		}

		public override void GetOpInfo(GasFormatterOptions options, in Instruction instr, out InstrOpInfo info) {
			info = default;
			info.Mnemonic = mnemonic;
			Debug.Assert(instr.OpCount == 2);
			Debug.Assert(instr.Op0Kind == OpKind.Register && instr.Op0Register == Register.ST0);
			if (!pseudoOp || !(options.UsePseudoOps && instr.Op1Register == Register.ST1)) {
				info.OpCount = 1;
				Debug.Assert(InstrOpKind.Register == 0);
				//info.Op0Kind = InstrOpKind.Register;
				info.Op0Register = (byte)instr.Op1Register;
				info.Op0Index = 1;
			}
		}
	}

	sealed class SimpleInstrInfo_STi_ST2 : InstrInfo {
		readonly string mnemonic;

		public SimpleInstrInfo_STi_ST2(Code code, string mnemonic)
			: base(code) {
			this.mnemonic = mnemonic;
		}

		public override void GetOpInfo(GasFormatterOptions options, in Instruction instr, out InstrOpInfo info) {
			const InstrOpInfoFlags flags = 0;
			if (options.UsePseudoOps && (instr.Op0Register == Register.ST1 || instr.Op1Register == Register.ST1)) {
				info = default;
				info.Mnemonic = mnemonic;
			}
			else {
				info = new InstrOpInfo(mnemonic, instr, flags);
				Debug.Assert(info.Op0Register == (int)Register.ST0);
				Debug.Assert(InstrOpInfo.TEST_RegisterBits == 8);
				info.Op0Register = (byte)Registers.Register_ST;
			}
		}
	}

	sealed class SimpleInstrInfo_ST_STi : InstrInfo {
		readonly string mnemonic;

		public SimpleInstrInfo_ST_STi(Code code, string mnemonic)
			: base(code) => this.mnemonic = mnemonic;

		public override void GetOpInfo(GasFormatterOptions options, in Instruction instr, out InstrOpInfo info) {
			info = new InstrOpInfo(mnemonic, instr, InstrOpInfoFlags.None);
			Debug.Assert(info.Op1Register == (int)Register.ST0);
			Debug.Assert(InstrOpInfo.TEST_RegisterBits == 8);
			info.Op1Register = (byte)Registers.Register_ST;
		}
	}

	sealed class SimpleInstrInfo_STi_ST : InstrInfo {
		readonly string mnemonic;

		public SimpleInstrInfo_STi_ST(Code code, string mnemonic)
			: base(code) => this.mnemonic = mnemonic;

		public override void GetOpInfo(GasFormatterOptions options, in Instruction instr, out InstrOpInfo info) {
			info = new InstrOpInfo(mnemonic, instr, InstrOpInfoFlags.None);
			Debug.Assert(info.Op0Register == (int)Register.ST0);
			Debug.Assert(InstrOpInfo.TEST_RegisterBits == 8);
			info.Op0Register = (byte)Registers.Register_ST;
		}
	}

	sealed class SimpleInstrInfo_as : InstrInfo {
		readonly int codeSize;
		readonly string mnemonic;

		public SimpleInstrInfo_as(Code code, int codeSize, string mnemonic)
			: base(code) {
			this.codeSize = codeSize;
			this.mnemonic = mnemonic;
		}

		public override void GetOpInfo(GasFormatterOptions options, in Instruction instr, out InstrOpInfo info) {
			InstrOpInfoFlags flags = 0;
			var instrCodeSize = GetCodeSize(instr.CodeSize);
			if (instrCodeSize != 0 && instrCodeSize != codeSize) {
				if (codeSize == 16)
					flags |= InstrOpInfoFlags.AddrSize16;
				else if (codeSize == 32)
					flags |= InstrOpInfoFlags.AddrSize32;
				else
					flags |= InstrOpInfoFlags.AddrSize64;
			}
			info = new InstrOpInfo(mnemonic, instr, flags);
		}
	}

	sealed class SimpleInstrInfo_maskmovq : InstrInfo {
		readonly string mnemonic;

		public SimpleInstrInfo_maskmovq(Code code, string mnemonic) : base(code) => this.mnemonic = mnemonic;

		public override void GetOpInfo(GasFormatterOptions options, in Instruction instr, out InstrOpInfo info) {
			Debug.Assert(instr.OpCount == 3);

			var instrCodeSize = GetCodeSize(instr.CodeSize);

			int codeSize;
			switch (instr.Op0Kind) {
			case OpKind.MemorySegDI:
				codeSize = 16;
				break;

			case OpKind.MemorySegEDI:
				codeSize = 32;
				break;

			case OpKind.MemorySegRDI:
				codeSize = 64;
				break;

			default:
				codeSize = instrCodeSize;
				break;
			}

			info = default;
			info.Mnemonic = mnemonic;
			info.OpCount = 2;
			info.Op0Kind = (InstrOpKind)instr.Op2Kind;
			info.Op0Register = (byte)instr.Op2Register;
			info.Op0Index = 2;
			info.Op1Kind = (InstrOpKind)instr.Op1Kind;
			info.Op1Register = (byte)instr.Op1Register;
			info.Op1Index = 1;
			if (instrCodeSize != 0 && instrCodeSize != codeSize) {
				if (codeSize == 16)
					info.Flags |= InstrOpInfoFlags.AddrSize16;
				else if (codeSize == 32)
					info.Flags |= InstrOpInfoFlags.AddrSize32;
				else
					info.Flags |= InstrOpInfoFlags.AddrSize64;
			}
		}
	}

	sealed class SimpleInstrInfo_pblendvb : InstrInfo {
		readonly string mnemonic;

		public SimpleInstrInfo_pblendvb(Code code, string mnemonic) : base(code) => this.mnemonic = mnemonic;

		public override void GetOpInfo(GasFormatterOptions options, in Instruction instr, out InstrOpInfo info) {
			info = default;
			Debug.Assert(instr.OpCount == 2);
			info.Mnemonic = mnemonic;
			info.OpCount = 3;
			Debug.Assert(InstrOpKind.Register == 0);
			//info.Op0Kind = InstrOpKind.Register;
			Debug.Assert(InstrOpInfo.TEST_RegisterBits == 8);
			info.Op0Register = (byte)Register.XMM0;
			info.Op0Index = OpAccess_Read;
			info.Op1Kind = (InstrOpKind)instr.Op1Kind;
			info.Op1Index = 1;
			info.Op1Register = (byte)instr.Op1Register;
			info.Op2Kind = (InstrOpKind)instr.Op0Kind;
			info.Op2Register = (byte)instr.Op0Register;
		}
	}

	sealed class SimpleInstrInfo_OpSize : InstrInfo {
		readonly CodeSize codeSize;
		readonly string[] mnemonics;

		public SimpleInstrInfo_OpSize(Code code, CodeSize codeSize, string mnemonic, string mnemonic16, string mnemonic32, string mnemonic64)
			: base(code) {
			this.codeSize = codeSize;
			mnemonics = new string[4];
			mnemonics[(int)CodeSize.Unknown] = mnemonic;
			mnemonics[(int)CodeSize.Code16] = mnemonic16;
			mnemonics[(int)CodeSize.Code32] = mnemonic32;
			mnemonics[(int)CodeSize.Code64] = mnemonic64;
		}

		public override void GetOpInfo(GasFormatterOptions options, in Instruction instr, out InstrOpInfo info) {
			string mnemonic;
			if (instr.CodeSize == codeSize && !options.ShowMnemonicSizeSuffix)
				mnemonic = mnemonics[(int)CodeSize.Unknown];
			else
				mnemonic = mnemonics[(int)codeSize];
			info = new InstrOpInfo(mnemonic, instr, InstrOpInfoFlags.None);
		}
	}

	sealed class SimpleInstrInfo_OpSize2_bnd : InstrInfo {
		readonly string[] mnemonics;

		public SimpleInstrInfo_OpSize2_bnd(Code code, string mnemonic, string mnemonic16, string mnemonic32, string mnemonic64)
			: base(code) {
			mnemonics = new string[4];
			mnemonics[(int)CodeSize.Unknown] = mnemonic;
			mnemonics[(int)CodeSize.Code16] = mnemonic16;
			mnemonics[(int)CodeSize.Code32] = mnemonic32;
			mnemonics[(int)CodeSize.Code64] = mnemonic64;
		}

		public override void GetOpInfo(GasFormatterOptions options, in Instruction instr, out InstrOpInfo info) {
			var flags = InstrOpInfoFlags.None;
			if (instr.HasRepnePrefix)
				flags |= InstrOpInfoFlags.BndPrefix;
			int codeSize = (int)instr.CodeSize;
			if (options.ShowMnemonicSizeSuffix)
				codeSize = (int)CodeSize.Code64;
			var mnemonic = mnemonics[codeSize];
			info = new InstrOpInfo(mnemonic, instr, flags);
		}
	}

	sealed class SimpleInstrInfo_OpSize3 : InstrInfo {
		readonly int codeSize;
		readonly string mnemonic;
		readonly string mnemonic_suffix;

		public SimpleInstrInfo_OpSize3(Code code, int codeSize, string mnemonic, string mnemonic_suffix)
			: base(code) {
			this.codeSize = codeSize;
			this.mnemonic = mnemonic;
			this.mnemonic_suffix = mnemonic_suffix;
		}

		public override void GetOpInfo(GasFormatterOptions options, in Instruction instr, out InstrOpInfo info) {
			var instrCodeSize = GetCodeSize(instr.CodeSize);
			string mnemonic;
			if (!options.ShowMnemonicSizeSuffix && (instrCodeSize == 0 || (instrCodeSize & codeSize) != 0))
				mnemonic = this.mnemonic;
			else
				mnemonic = mnemonic_suffix;
			info = new InstrOpInfo(mnemonic, instr, InstrOpInfoFlags.None);
		}
	}

	sealed class SimpleInstrInfo_os : InstrInfo {
		readonly int codeSize;
		readonly string mnemonic;
		readonly InstrOpInfoFlags flags;

		public SimpleInstrInfo_os(Code code, int codeSize, string mnemonic) : this(code, codeSize, mnemonic, InstrOpInfoFlags.None) { }
		public SimpleInstrInfo_os(Code code, int codeSize, string mnemonic, InstrOpInfoFlags flags)
			: base(code) {
			this.codeSize = codeSize;
			this.mnemonic = mnemonic;
			this.flags = flags;
		}

		public override void GetOpInfo(GasFormatterOptions options, in Instruction instr, out InstrOpInfo info) {
			var flags = this.flags;
			int instrCodeSize = GetCodeSize(instr.CodeSize);
			if (instrCodeSize != 0 && instrCodeSize != codeSize) {
				if (codeSize == 16)
					flags |= InstrOpInfoFlags.OpSize16;
				else if (codeSize == 32)
					flags |= InstrOpInfoFlags.OpSize32;
				else
					flags |= InstrOpInfoFlags.OpSize64;
			}
			info = new InstrOpInfo(GetMnemonic(options, instr, mnemonic, mnemonic, flags), instr, flags);
		}
	}

	sealed class SimpleInstrInfo_os2 : InstrInfo {
		readonly int codeSize;
		readonly string mnemonic;
		readonly string mnemonic_suffix;
		readonly InstrOpInfoFlags flags;

		public SimpleInstrInfo_os2(Code code, int codeSize, string mnemonic, string mnemonic_suffix) : this(code, codeSize, mnemonic, mnemonic_suffix, InstrOpInfoFlags.None) { }

		public SimpleInstrInfo_os2(Code code, int codeSize, string mnemonic, string mnemonic_suffix, InstrOpInfoFlags flags)
			: base(code) {
			this.codeSize = codeSize;
			this.mnemonic = mnemonic;
			this.mnemonic_suffix = mnemonic_suffix;
			this.flags = flags;
		}

		public override void GetOpInfo(GasFormatterOptions options, in Instruction instr, out InstrOpInfo info) {
			string mnemonic;
			int instrCodeSize = GetCodeSize(instr.CodeSize);
			if (instrCodeSize != 0 && instrCodeSize != codeSize)
				mnemonic = mnemonic_suffix;
			else
				mnemonic = GetMnemonic(options, instr, this.mnemonic, mnemonic_suffix, flags);
			info = new InstrOpInfo(mnemonic, instr, flags);
		}
	}

	sealed class SimpleInstrInfo_os2_bnd : InstrInfo {
		readonly int codeSize;
		readonly string mnemonic;
		readonly string mnemonic_suffix;

		public SimpleInstrInfo_os2_bnd(Code code, int codeSize, string mnemonic, string mnemonic_suffix)
			: base(code) {
			this.codeSize = codeSize;
			this.mnemonic = mnemonic;
			this.mnemonic_suffix = mnemonic_suffix;
		}

		public override void GetOpInfo(GasFormatterOptions options, in Instruction instr, out InstrOpInfo info) {
			var flags = InstrOpInfoFlags.None;
			if (instr.HasRepnePrefix)
				flags |= InstrOpInfoFlags.BndPrefix;
			string mnemonic;
			int instrCodeSize = GetCodeSize(instr.CodeSize);
			if (instrCodeSize != 0 && instrCodeSize != codeSize)
				mnemonic = mnemonic_suffix;
			else
				mnemonic = GetMnemonic(options, instr, this.mnemonic, mnemonic_suffix, flags);
			info = new InstrOpInfo(mnemonic, instr, flags);
		}
	}

	sealed class SimpleInstrInfo_os_bnd : InstrInfo {
		readonly int codeSize;
		readonly string mnemonic;

		public SimpleInstrInfo_os_bnd(Code code, int codeSize, string mnemonic)
			: base(code) {
			this.codeSize = codeSize;
			this.mnemonic = mnemonic;
		}

		public override void GetOpInfo(GasFormatterOptions options, in Instruction instr, out InstrOpInfo info) {
			var flags = InstrOpInfoFlags.None;
			if (instr.HasRepnePrefix)
				flags |= InstrOpInfoFlags.BndPrefix;
			int instrCodeSize = GetCodeSize(instr.CodeSize);
			if (instrCodeSize != 0 && instrCodeSize != codeSize) {
				if (codeSize == 16)
					flags |= InstrOpInfoFlags.OpSize16;
				else if (codeSize == 32)
					flags |= InstrOpInfoFlags.OpSize32;
				else
					flags |= InstrOpInfoFlags.OpSize64;
			}
			info = new InstrOpInfo(GetMnemonic(options, instr, mnemonic, mnemonic, flags), instr, flags);
		}
	}

	sealed class SimpleInstrInfo_os_mem : InstrInfo {
		readonly int codeSize;
		readonly string mnemonic;
		readonly string mnemonic_suffix;

		public SimpleInstrInfo_os_mem(Code code, int codeSize, string mnemonic, string mnemonic_suffix)
			: base(code) {
			this.codeSize = codeSize;
			this.mnemonic = mnemonic;
			this.mnemonic_suffix = mnemonic_suffix;
		}

		public override void GetOpInfo(GasFormatterOptions options, in Instruction instr, out InstrOpInfo info) {
			var flags = InstrOpInfoFlags.None;
			int instrCodeSize = GetCodeSize(instr.CodeSize);
			bool hasMemOp = instr.Op0Kind == OpKind.Memory || instr.Op1Kind == OpKind.Memory;
			if (hasMemOp && !(instrCodeSize == 0 || (instrCodeSize != 64 && instrCodeSize == codeSize) || (instrCodeSize == 64 && codeSize == 32))) {
				if (codeSize == 16)
					flags |= InstrOpInfoFlags.OpSize16;
				else if (codeSize == 32)
					flags |= InstrOpInfoFlags.OpSize32;
				else
					flags |= InstrOpInfoFlags.OpSize64;
			}
			var mnemonic = GetMnemonic(options, instr, this.mnemonic, mnemonic_suffix, flags);
			if (hasMemOp)
				mnemonic = this.mnemonic;
			info = new InstrOpInfo(mnemonic, instr, flags);
		}
	}

	sealed class SimpleInstrInfo_os_mem2 : InstrInfo {
		readonly int codeSize;
		readonly string mnemonic;
		readonly string mnemonic_suffix;

		public SimpleInstrInfo_os_mem2(Code code, int codeSize, string mnemonic, string mnemonic_suffix)
			: base(code) {
			this.codeSize = codeSize;
			this.mnemonic = mnemonic;
			this.mnemonic_suffix = mnemonic_suffix;
		}

		public override void GetOpInfo(GasFormatterOptions options, in Instruction instr, out InstrOpInfo info) {
			int instrCodeSize = GetCodeSize(instr.CodeSize);
			string mnemonic;
			if ((instrCodeSize != 0 && (instrCodeSize & codeSize) == 0))
				mnemonic = mnemonic_suffix;
			else
				mnemonic = this.mnemonic;
			info = new InstrOpInfo(mnemonic, instr, InstrOpInfoFlags.None);
		}
	}

	sealed class SimpleInstrInfo_os_mem_reg16 : InstrInfo {
		readonly int codeSize;
		readonly string mnemonic;

		public SimpleInstrInfo_os_mem_reg16(Code code, int codeSize, string mnemonic)
			: base(code) {
			this.codeSize = codeSize;
			this.mnemonic = mnemonic;
		}

		public override void GetOpInfo(GasFormatterOptions options, in Instruction instr, out InstrOpInfo info) {
			var flags = InstrOpInfoFlags.None;
			int instrCodeSize = GetCodeSize(instr.CodeSize);
			Debug.Assert(instr.OpCount == 1);
			if (instr.Op0Kind == OpKind.Memory) {
				if (!(instrCodeSize == 0 || (instrCodeSize != 64 && instrCodeSize == codeSize) || (instrCodeSize == 64 && codeSize == 32))) {
					if (codeSize == 16)
						flags |= InstrOpInfoFlags.OpSize16;
					else if (codeSize == 32)
						flags |= InstrOpInfoFlags.OpSize32;
					else
						flags |= InstrOpInfoFlags.OpSize64;
				}
			}
			info = new InstrOpInfo(mnemonic, instr, flags);
			if (instr.Op0Kind == OpKind.Register) {
				var reg = (Register)info.Op0Register;
				int regSize = 0;
				if (Register.AX <= reg && reg <= Register.R15W)
					regSize = 16;
				else if (Register.EAX <= reg && reg <= Register.R15D) {
					regSize = 32;
					reg = reg - Register.EAX + Register.AX;
				}
				else if (Register.RAX <= reg && reg <= Register.R15) {
					regSize = 64;
					reg = reg - Register.RAX + Register.AX;
				}
				Debug.Assert(regSize != 0);
				if (regSize != 0) {
					Debug.Assert(InstrOpInfo.TEST_RegisterBits == 8);
					info.Op0Register = (byte)reg;
					if (!((instrCodeSize != 64 && instrCodeSize == regSize) || (instrCodeSize == 64 && regSize == 32))) {
						if (codeSize == 16)
							info.Flags |= InstrOpInfoFlags.OpSize16;
						else if (codeSize == 32)
							info.Flags |= InstrOpInfoFlags.OpSize32;
						else
							info.Flags |= InstrOpInfoFlags.OpSize64;
					}
				}
			}
		}
	}

	sealed class SimpleInstrInfo_os_loop : InstrInfo {
		readonly int codeSize;
		readonly int regSize;
		readonly string mnemonic;
		readonly string mnemonic_suffix;

		public SimpleInstrInfo_os_loop(Code code, int codeSize, int regSize, string mnemonic, string mnemonic_suffix)
			: base(code) {
			this.codeSize = codeSize;
			this.regSize = regSize;
			this.mnemonic = mnemonic;
			this.mnemonic_suffix = mnemonic_suffix;
		}

		public override void GetOpInfo(GasFormatterOptions options, in Instruction instr, out InstrOpInfo info) {
			var flags = InstrOpInfoFlags.None;
			int instrCodeSize = GetCodeSize(instr.CodeSize);
			var mnemonic = this.mnemonic;
			if ((instrCodeSize != 0 && instrCodeSize != regSize) || options.ShowMnemonicSizeSuffix)
				mnemonic = mnemonic_suffix;
			if (instrCodeSize != 0 && instrCodeSize != codeSize) {
				if (codeSize == 16)
					flags |= InstrOpInfoFlags.OpSize16 | InstrOpInfoFlags.OpSizeIsByteDirective;
				else if (codeSize == 32)
					flags |= InstrOpInfoFlags.OpSize32 | InstrOpInfoFlags.OpSizeIsByteDirective;
				else
					flags |= InstrOpInfoFlags.OpSize64;
			}
			info = new InstrOpInfo(mnemonic, instr, flags);
		}
	}

	sealed class SimpleInstrInfo_os_jcc : InstrInfo {
		readonly int codeSize;
		readonly string mnemonic;

		public SimpleInstrInfo_os_jcc(Code code, int codeSize, string mnemonic)
			: base(code) {
			this.codeSize = codeSize;
			this.mnemonic = mnemonic;
		}

		public override void GetOpInfo(GasFormatterOptions options, in Instruction instr, out InstrOpInfo info) {
			var flags = InstrOpInfoFlags.None;
			int instrCodeSize = GetCodeSize(instr.CodeSize);
			if (instrCodeSize != 0 && instrCodeSize != codeSize) {
				if (codeSize == 16)
					flags |= InstrOpInfoFlags.OpSize16;
				else if (codeSize == 32)
					flags |= InstrOpInfoFlags.OpSize32;
				else
					flags |= InstrOpInfoFlags.OpSize64;
			}
			var prefixSeg = instr.SegmentPrefix;
			if (prefixSeg == Register.CS)
				flags |= InstrOpInfoFlags.JccNotTaken;
			else if (prefixSeg == Register.DS)
				flags |= InstrOpInfoFlags.JccTaken;
			if (instr.HasRepnePrefix)
				flags |= InstrOpInfoFlags.BndPrefix;
			info = new InstrOpInfo(GetMnemonic(options, instr, mnemonic, mnemonic, flags), instr, flags);
		}
	}

	sealed class SimpleInstrInfo_xbegin : InstrInfo {
		readonly int codeSize;
		readonly string mnemonic;

		public SimpleInstrInfo_xbegin(Code code, int codeSize, string mnemonic)
			: base(code) {
			this.codeSize = codeSize;
			this.mnemonic = mnemonic;
		}

		public override void GetOpInfo(GasFormatterOptions options, in Instruction instr, out InstrOpInfo info) {
			var flags = InstrOpInfoFlags.None;
			int instrCodeSize = GetCodeSize(instr.CodeSize);
			if (instrCodeSize == 0) {
				// Nothing
			}
			else if (instrCodeSize == 64) {
				if ((codeSize & 16) != 0)
					flags |= InstrOpInfoFlags.OpSize16;
			}
			else if ((instrCodeSize & codeSize) == 0) {
				if ((codeSize & 16) != 0)
					flags |= InstrOpInfoFlags.OpSize16;
				else if ((codeSize & 32) != 0)
					flags |= InstrOpInfoFlags.OpSize32;
			}
			info = new InstrOpInfo(mnemonic, instr, flags);
		}
	}

	sealed class SimpleInstrInfo_movabs : InstrInfo {
		readonly int memOpNumber;
		readonly string mnemonic;
		readonly string mnemonic_suffix;
		readonly string mnemonic64;
		readonly string mnemonic_suffix64;

		public SimpleInstrInfo_movabs(Code code, int memOpNumber, string mnemonic, string mnemonic_suffix, string mnemonic64, string mnemonic_suffix64)
			: base(code) {
			this.memOpNumber = memOpNumber;
			this.mnemonic = mnemonic;
			this.mnemonic_suffix = mnemonic_suffix;
			this.mnemonic64 = mnemonic64;
			this.mnemonic_suffix64 = mnemonic_suffix64;
		}

		public override void GetOpInfo(GasFormatterOptions options, in Instruction instr, out InstrOpInfo info) {
			var flags = InstrOpInfoFlags.None;
			int instrCodeSize = GetCodeSize(instr.CodeSize);
			var opKind = instr.GetOpKind(memOpNumber);
			int memSize;
			string mnemonic, mnemonic_suffix;
			if (opKind == OpKind.Memory64) {
				mnemonic = mnemonic64;
				mnemonic_suffix = mnemonic_suffix64;
				memSize = 64;
			}
			else {
				mnemonic = this.mnemonic;
				mnemonic_suffix = this.mnemonic_suffix;
				Debug.Assert(opKind == OpKind.Memory);
				int displSize = instr.MemoryDisplSize;
				memSize = displSize == 2 ? 16 : 32;
			}
			if (instrCodeSize == 0)
				instrCodeSize = memSize;
			if (instrCodeSize == 64) {
				if (memSize == 32)
					flags |= InstrOpInfoFlags.AddrSize32;
			}
			else if (instrCodeSize != memSize) {
				Debug.Assert(memSize == 16 || memSize == 32);
				if (memSize == 16)
					flags |= InstrOpInfoFlags.AddrSize16;
				else
					flags |= InstrOpInfoFlags.AddrSize32;
			}
			info = new InstrOpInfo(GetMnemonic(options, instr, mnemonic, mnemonic_suffix, flags), instr, flags);
		}
	}

	sealed class SimpleInstrInfo_er : InstrInfo {
		readonly int erIndex;
		readonly string mnemonic;
		readonly string mnemonic_suffix;
		readonly InstrOpInfoFlags flags;

		public SimpleInstrInfo_er(Code code, int erIndex, string mnemonic) : this(code, erIndex, mnemonic, mnemonic, InstrOpInfoFlags.None) { }

		public SimpleInstrInfo_er(Code code, int erIndex, string mnemonic, string mnemonic_suffix, InstrOpInfoFlags flags)
			: base(code) {
			this.erIndex = erIndex;
			this.mnemonic = mnemonic;
			this.mnemonic_suffix = mnemonic_suffix;
			this.flags = flags;
		}

		public override void GetOpInfo(GasFormatterOptions options, in Instruction instr, out InstrOpInfo info) {
			info = new InstrOpInfo(GetMnemonic(options, instr, mnemonic, mnemonic_suffix, flags), instr, flags);
			var rc = instr.RoundingControl;
			if (rc != RoundingControl.None) {
				InstrOpKind rcOpKind;
				switch (rc) {
				case RoundingControl.RoundToNearest:	rcOpKind = InstrOpKind.RnSae; break;
				case RoundingControl.RoundDown:			rcOpKind = InstrOpKind.RdSae; break;
				case RoundingControl.RoundUp:			rcOpKind = InstrOpKind.RuSae; break;
				case RoundingControl.RoundTowardZero:	rcOpKind = InstrOpKind.RzSae; break;
				default:
					return;
				}
				MoveOperands(ref info, erIndex, rcOpKind);
			}
		}

		internal static void MoveOperands(ref InstrOpInfo info, int index, InstrOpKind newOpKind) {
			Debug.Assert(info.OpCount <= 4);

			switch (index) {
			case 0:
				info.Op4Kind = info.Op3Kind;
				info.Op4Register = info.Op3Register;
				info.Op3Kind = info.Op2Kind;
				info.Op3Register = info.Op2Register;
				info.Op2Kind = info.Op1Kind;
				info.Op2Register = info.Op1Register;
				info.Op1Kind = info.Op0Kind;
				info.Op1Register = info.Op0Register;
				info.Op0Kind = newOpKind;
				info.Op4Index = info.Op3Index;
				info.Op3Index = info.Op2Index;
				info.Op2Index = info.Op1Index;
				info.Op1Index = info.Op0Index;
				info.Op0Index = OpAccess_None;
				info.OpCount++;
				break;

			case 1:
				info.Op4Kind = info.Op3Kind;
				info.Op4Register = info.Op3Register;
				info.Op3Kind = info.Op2Kind;
				info.Op3Register = info.Op2Register;
				info.Op2Kind = info.Op1Kind;
				info.Op2Register = info.Op1Register;
				info.Op1Kind = newOpKind;
				info.Op4Index = info.Op3Index;
				info.Op3Index = info.Op2Index;
				info.Op2Index = info.Op1Index;
				info.Op1Index = OpAccess_None;
				info.OpCount++;
				break;

			default:
				throw new InvalidOperationException();
			}
		}
	}

	sealed class SimpleInstrInfo_sae : InstrInfo {
		readonly int saeIndex;
		readonly string mnemonic;

		public SimpleInstrInfo_sae(Code code, int saeIndex, string mnemonic)
			: base(code) {
			this.saeIndex = saeIndex;
			this.mnemonic = mnemonic;
		}

		public override void GetOpInfo(GasFormatterOptions options, in Instruction instr, out InstrOpInfo info) {
			info = new InstrOpInfo(mnemonic, instr, InstrOpInfoFlags.None);
			if (instr.SuppressAllExceptions)
				SimpleInstrInfo_er.MoveOperands(ref info, saeIndex, InstrOpKind.Sae);
		}
	}

	sealed class SimpleInstrInfo_far : InstrInfo {
		readonly int codeSize;
		readonly string mnemonic;
		readonly string mnemonic_suffix;

		public SimpleInstrInfo_far(Code code, int codeSize, string mnemonic, string mnemonic_suffix)
			: base(code) {
			this.codeSize = codeSize;
			this.mnemonic = mnemonic;
			this.mnemonic_suffix = mnemonic_suffix;
		}

		public override void GetOpInfo(GasFormatterOptions options, in Instruction instr, out InstrOpInfo info) {
			var flags = InstrOpInfoFlags.IndirectOperand;
			var instrCodeSize = GetCodeSize(instr.CodeSize);
			string mnemonic;
			if (instrCodeSize == 0)
				instrCodeSize = codeSize;
			if (codeSize == 64) {
				flags |= InstrOpInfoFlags.OpSize64;
				Debug.Assert(this.mnemonic == mnemonic_suffix);
				mnemonic = this.mnemonic;
			}
			else {
				if (codeSize != instrCodeSize || options.ShowMnemonicSizeSuffix)
					mnemonic = mnemonic_suffix;
				else
					mnemonic = this.mnemonic;
			}
			info = new InstrOpInfo(mnemonic, instr, flags);
		}
	}

	sealed class SimpleInstrInfo_bnd2 : InstrInfo {
		readonly string mnemonic;
		readonly string mnemonic_suffix;
		readonly InstrOpInfoFlags flags;

		public SimpleInstrInfo_bnd2(Code code, string mnemonic, string mnemonic_suffix) : this(code, mnemonic, mnemonic_suffix, InstrOpInfoFlags.None) { }

		public SimpleInstrInfo_bnd2(Code code, string mnemonic, string mnemonic_suffix, InstrOpInfoFlags flags)
			: base(code) {
			this.mnemonic = mnemonic;
			this.mnemonic_suffix = mnemonic_suffix;
			this.flags = flags;
		}

		public override void GetOpInfo(GasFormatterOptions options, in Instruction instr, out InstrOpInfo info) {
			var flags = this.flags;
			if (instr.HasRepnePrefix)
				flags |= InstrOpInfoFlags.BndPrefix;
			info = new InstrOpInfo(GetMnemonic(options, instr, mnemonic, mnemonic_suffix, flags), instr, flags);
		}
	}

	sealed class SimpleInstrInfo_pops : InstrInfo {
		readonly string mnemonic;
		readonly string[] pseudo_ops;

		public SimpleInstrInfo_pops(Code code, string mnemonic, string[] pseudo_ops)
			: base(code) {
			this.mnemonic = mnemonic;
			this.pseudo_ops = pseudo_ops;
		}

		public override void GetOpInfo(GasFormatterOptions options, in Instruction instr, out InstrOpInfo info) {
			info = new InstrOpInfo(mnemonic, instr, InstrOpInfoFlags.None);
			int imm = instr.Immediate8;
			if (options.UsePseudoOps && (uint)imm < (uint)pseudo_ops.Length) {
				RemoveFirstImm8Operand(ref info);
				info.Mnemonic = pseudo_ops[imm];
			}
		}

		internal static void RemoveFirstImm8Operand(ref InstrOpInfo info) {
			Debug.Assert(info.Op0Kind == InstrOpKind.Immediate8);
			info.OpCount--;
			switch (info.OpCount) {
			case 0:
				info.Op0Index = OpAccess_INVALID;
				break;

			case 1:
				info.Op0Kind = info.Op1Kind;
				info.Op0Register = info.Op1Register;
				info.Op0Index = info.Op1Index;
				info.Op1Index = OpAccess_INVALID;
				break;

			case 2:
				info.Op0Kind = info.Op1Kind;
				info.Op0Register = info.Op1Register;
				info.Op1Kind = info.Op2Kind;
				info.Op1Register = info.Op2Register;
				info.Op0Index = info.Op1Index;
				info.Op1Index = info.Op2Index;
				info.Op2Index = OpAccess_INVALID;
				break;

			case 3:
				info.Op0Kind = info.Op1Kind;
				info.Op0Register = info.Op1Register;
				info.Op1Kind = info.Op2Kind;
				info.Op1Register = info.Op2Register;
				info.Op2Kind = info.Op3Kind;
				info.Op2Register = info.Op3Register;
				info.Op0Index = info.Op1Index;
				info.Op1Index = info.Op2Index;
				info.Op2Index = info.Op3Index;
				info.Op3Index = OpAccess_INVALID;
				break;

			case 4:
				info.Op0Kind = info.Op1Kind;
				info.Op0Register = info.Op1Register;
				info.Op1Kind = info.Op2Kind;
				info.Op1Register = info.Op2Register;
				info.Op2Kind = info.Op3Kind;
				info.Op2Register = info.Op3Register;
				info.Op3Kind = info.Op4Kind;
				info.Op3Register = info.Op4Register;
				info.Op0Index = info.Op1Index;
				info.Op1Index = info.Op2Index;
				info.Op2Index = info.Op3Index;
				info.Op3Index = info.Op4Index;
				info.Op4Index = OpAccess_INVALID;
				break;

			default:
				throw new InvalidOperationException();
			}
		}
	}

	sealed class SimpleInstrInfo_sae_pops : InstrInfo {
		readonly int saeIndex;
		readonly string mnemonic;
		readonly string[] pseudo_ops;

		public SimpleInstrInfo_sae_pops(Code code, int saeIndex, string mnemonic, string[] pseudo_ops)
			: base(code) {
			this.saeIndex = saeIndex;
			this.mnemonic = mnemonic;
			this.pseudo_ops = pseudo_ops;
		}

		public override void GetOpInfo(GasFormatterOptions options, in Instruction instr, out InstrOpInfo info) {
			info = new InstrOpInfo(mnemonic, instr, InstrOpInfoFlags.None);
			if (instr.SuppressAllExceptions)
				SimpleInstrInfo_er.MoveOperands(ref info, saeIndex, InstrOpKind.Sae);
			int imm = instr.Immediate8;
			if (options.UsePseudoOps && (uint)imm < (uint)pseudo_ops.Length) {
				SimpleInstrInfo_pops.RemoveFirstImm8Operand(ref info);
				info.Mnemonic = pseudo_ops[imm];
			}
		}
	}

	sealed class SimpleInstrInfo_pclmulqdq : InstrInfo {
		readonly string mnemonic;
		readonly string[] pseudo_ops;

		public SimpleInstrInfo_pclmulqdq(Code code, string mnemonic, string[] pseudo_ops)
			: base(code) {
			this.mnemonic = mnemonic;
			this.pseudo_ops = pseudo_ops;
		}

		public override void GetOpInfo(GasFormatterOptions options, in Instruction instr, out InstrOpInfo info) {
			info = new InstrOpInfo(mnemonic, instr, InstrOpInfoFlags.None);
			if (options.UsePseudoOps) {
				int index;
				int imm = instr.Immediate8;
				if (imm == 0)
					index = 0;
				else if (imm == 1)
					index = 1;
				else if (imm == 0x10)
					index = 2;
				else if (imm == 0x11)
					index = 3;
				else
					index = -1;
				if (index >= 0) {
					SimpleInstrInfo_pops.RemoveFirstImm8Operand(ref info);
					info.Mnemonic = pseudo_ops[index];
				}
			}
		}
	}

	sealed class SimpleInstrInfo_imul : InstrInfo {
		readonly string mnemonic;
		readonly string mnemonic_suffix;

		public SimpleInstrInfo_imul(Code code, string mnemonic, string mnemonic_suffix)
			: base(code) {
			this.mnemonic = mnemonic;
			this.mnemonic_suffix = mnemonic_suffix;
		}

		public override void GetOpInfo(GasFormatterOptions options, in Instruction instr, out InstrOpInfo info) {
			const InstrOpInfoFlags flags = 0;
			info = new InstrOpInfo(GetMnemonic(options, instr, mnemonic, mnemonic_suffix, flags), instr, flags);
			Debug.Assert(info.OpCount == 3);
			if (options.UsePseudoOps && info.Op1Kind == InstrOpKind.Register && info.Op2Kind == InstrOpKind.Register && info.Op1Register == info.Op2Register) {
				info.OpCount--;
				info.Op1Index = OpAccess_ReadWrite;
				info.Op2Index = OpAccess_INVALID;
			}
		}
	}

	sealed class SimpleInstrInfo_Reg16 : InstrInfo {
		readonly string mnemonic;

		public SimpleInstrInfo_Reg16(Code code, string mnemonic)
			: base(code) {
			this.mnemonic = mnemonic;
		}

		public override void GetOpInfo(GasFormatterOptions options, in Instruction instr, out InstrOpInfo info) {
			const InstrOpInfoFlags flags = InstrOpInfoFlags.None;
			info = new InstrOpInfo(mnemonic, instr, flags);
			if (Register.EAX <= (Register)info.Op0Register && (Register)info.Op0Register <= Register.R15D)
				info.Op0Register = (byte)((Register)info.Op0Register - Register.EAX + Register.AX);
			if (Register.EAX <= (Register)info.Op1Register && (Register)info.Op1Register <= Register.R15D)
				info.Op1Register = (byte)((Register)info.Op1Register - Register.EAX + Register.AX);
		}
	}

	sealed class SimpleInstrInfo_DeclareData : InstrInfo {
		readonly string mnemonic;
		readonly InstrOpKind opKind;

		public SimpleInstrInfo_DeclareData(Code code, string mnemonic)
			: base(code) {
			this.mnemonic = mnemonic;
			InstrOpKind opKind;
			switch (code) {
			case Code.DeclareByte: opKind = InstrOpKind.DeclareByte; break;
			case Code.DeclareWord: opKind = InstrOpKind.DeclareWord; break;
			case Code.DeclareDword: opKind = InstrOpKind.DeclareDword; break;
			case Code.DeclareQword: opKind = InstrOpKind.DeclareQword; break;
			default: throw new InvalidOperationException();
			}
			this.opKind = opKind;
		}

		public override void GetOpInfo(GasFormatterOptions options, in Instruction instr, out InstrOpInfo info) {
			info = new InstrOpInfo(mnemonic, instr, InstrOpInfoFlags.KeepOperandOrder | InstrOpInfoFlags.MnemonicIsDirective);
			info.OpCount = (byte)instr.DeclareDataCount;
			info.Op0Kind = opKind;
			info.Op1Kind = opKind;
			info.Op2Kind = opKind;
			info.Op3Kind = opKind;
			info.Op4Kind = opKind;
			info.Op0Index = OpAccess_Read;
			info.Op1Index = OpAccess_Read;
			info.Op2Index = OpAccess_Read;
			info.Op3Index = OpAccess_Read;
			info.Op4Index = OpAccess_Read;
		}
	}
}
#endif
