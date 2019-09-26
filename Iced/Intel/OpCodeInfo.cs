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
using System.Diagnostics;
using System.Text;
using Iced.Intel.EncoderInternal;

namespace Iced.Intel {
	/// <summary>
	/// Opcode info
	/// </summary>
	public sealed class OpCodeInfo {
		[Flags]
		enum Flags : uint {
			None					= 0,
			Mode16					= 0x00000001,
			Mode32					= 0x00000002,
			Mode64					= 0x00000004,
			Fwait					= 0x00000008,
			LIG						= 0x00000010,
			WIG						= 0x00000020,
			WIG32					= 0x00000040,
			W						= 0x00000080,
			Broadcast				= 0x00000100,
			RoundingControl			= 0x00000200,
			SuppressAllExceptions	= 0x00000400,
			OpMaskRegister			= 0x00000800,
			ZeroingMasking			= 0x00001000,
			LockPrefix				= 0x00002000,
			XacquirePrefix			= 0x00004000,
			XreleasePrefix			= 0x00008000,
			RepPrefix				= 0x00010000,
			RepnePrefix				= 0x00020000,
			BndPrefix				= 0x00040000,
			HintTakenPrefix			= 0x00080000,
			NotrackPrefix			= 0x00100000,
			IsInstruction			= 0x00200000,
			NonZeroOpMaskRegister	= 0x00400000,
		}

		readonly string toOpCodeStringValue;
		readonly string toInstructionStringValue;
		readonly Flags flags;
		readonly ushort code;
		readonly ushort opCode;
		readonly byte encoding;
		readonly byte operandSize;
		readonly byte addressSize;
		readonly byte l;
		readonly byte tupleType;
		readonly byte table;
		readonly byte mandatoryPrefix;
		readonly sbyte groupIndex;
		readonly byte op0Kind;
		readonly byte op1Kind;
		readonly byte op2Kind;
		readonly byte op3Kind;
		readonly byte op4Kind;
		readonly LKind lkind;

		internal OpCodeInfo(uint dword3, uint dword2, uint dword1, StringBuilder sb) {
			var code = (Code)(dword1 & (uint)EncFlags1.CodeMask);
			Debug.Assert((uint)code < (uint)DecoderConstants.NumberOfCodeValues);
			Debug.Assert((uint)code <= ushort.MaxValue);
			this.code = (ushort)code;
			if (!(code == Code.INVALID || code >= Code.DeclareByte))
				flags |= Flags.IsInstruction;
			opCode = (ushort)(dword1 >> (int)EncFlags1.OpCodeShift);

			byte[] opKinds;
			encoding = (byte)((dword1 >> (int)EncFlags1.EncodingShift) & (uint)EncFlags1.EncodingMask);
			switch ((EncodingKind)encoding) {
			case EncodingKind.Legacy:
				opKinds = OpCodeOperandKinds.LegacyOpKinds;
				op0Kind = opKinds[(int)(dword3 & (uint)LegacyFlags3.OpMask)];
				op1Kind = opKinds[(int)((dword3 >> (int)LegacyFlags3.Op1Shift) & (uint)LegacyFlags3.OpMask)];
				op2Kind = opKinds[(int)((dword3 >> (int)LegacyFlags3.Op2Shift) & (uint)LegacyFlags3.OpMask)];
				op3Kind = opKinds[(int)((dword3 >> (int)LegacyFlags3.Op3Shift) & (uint)LegacyFlags3.OpMask)];

				mandatoryPrefix = (MandatoryPrefixByte)((dword2 >> (int)LegacyFlags.MandatoryPrefixShift) & (uint)LegacyFlags.MandatoryPrefixMask) switch {
					MandatoryPrefixByte.None => (byte)((dword2 & (uint)LegacyFlags.HasMandatoryPrefix) != 0 ? MandatoryPrefix.PNP : MandatoryPrefix.None),
					MandatoryPrefixByte.P66 => (byte)MandatoryPrefix.P66,
					MandatoryPrefixByte.PF3 => (byte)MandatoryPrefix.PF3,
					MandatoryPrefixByte.PF2 => (byte)MandatoryPrefix.PF2,
					_ => throw new InvalidOperationException(),
				};

				table = (LegacyOpCodeTable)((dword2 >> (int)LegacyFlags.OpCodeTableShift) & (uint)LegacyFlags.OpCodeTableMask) switch {
					LegacyOpCodeTable.Normal => (byte)OpCodeTableKind.Normal,
					LegacyOpCodeTable.Table0F => (byte)OpCodeTableKind.T0F,
					LegacyOpCodeTable.Table0F38 => (byte)OpCodeTableKind.T0F38,
					LegacyOpCodeTable.Table0F3A => (byte)OpCodeTableKind.T0F3A,
					_ => throw new InvalidOperationException(),
				};

				groupIndex = (sbyte)((dword2 & (uint)LegacyFlags.HasGroupIndex) == 0 ? -1 : (int)((dword2 >> (int)LegacyFlags.GroupShift) & 7));
				tupleType = (byte)TupleType.None;

				if (!IsInstruction)
					flags |= Flags.Mode16 | Flags.Mode32 | Flags.Mode64;
				else {
					flags |= (Encodable)((dword2 >> (int)LegacyFlags.EncodableShift) & (uint)LegacyFlags.EncodableMask) switch {
						Encodable.Any => Flags.Mode16 | Flags.Mode32 | Flags.Mode64,
						Encodable.Only1632 => Flags.Mode16 | Flags.Mode32,
						Encodable.Only64 => Flags.Mode64,
						_ => throw new InvalidOperationException(),
					};
				}

				flags |= (AllowedPrefixes)((dword2 >> (int)LegacyFlags.AllowedPrefixesShift) & (uint)LegacyFlags.AllowedPrefixesMask) switch {
					AllowedPrefixes.None => Flags.None,
					AllowedPrefixes.Bnd => Flags.BndPrefix,
					AllowedPrefixes.BndNotrack => Flags.BndPrefix | Flags.NotrackPrefix,
					AllowedPrefixes.HintTakenBnd => Flags.HintTakenPrefix | Flags.BndPrefix,
					AllowedPrefixes.Lock => Flags.LockPrefix,
					AllowedPrefixes.Rep => Flags.RepPrefix,
					AllowedPrefixes.RepeRepne => Flags.RepPrefix | Flags.RepnePrefix,
					AllowedPrefixes.XacquireXreleaseLock => Flags.XacquirePrefix | Flags.XreleasePrefix | Flags.LockPrefix,
					AllowedPrefixes.Xrelease => Flags.XreleasePrefix,
					_ => throw new InvalidOperationException(),
				};
				if ((dword2 & (uint)LegacyFlags.Fwait) != 0)
					flags |= Flags.Fwait;

				switch ((OperandSize)((dword2 >> (int)LegacyFlags.Legacy_OpSizeShift) & (uint)LegacyFlags.Legacy_OperandSizeMask)) {
				case EncoderInternal.OperandSize.None:
					operandSize = 0;
					break;
				case EncoderInternal.OperandSize.Size16:
					operandSize = 16;
					break;
				case EncoderInternal.OperandSize.Size32:
					operandSize = 32;
					break;
				case EncoderInternal.OperandSize.Size64:
					operandSize = 64;
					break;
				}

				switch ((AddressSize)((dword2 >> (int)LegacyFlags.Legacy_AddrSizeShift) & (uint)LegacyFlags.Legacy_AddressSizeMask)) {
				case EncoderInternal.AddressSize.None:
					addressSize = 0;
					break;
				case EncoderInternal.AddressSize.Size16:
					addressSize = 16;
					break;
				case EncoderInternal.AddressSize.Size32:
					addressSize = 32;
					break;
				case EncoderInternal.AddressSize.Size64:
					addressSize = 64;
					break;
				}

				l = 0;
				lkind = LKind.None;
				break;

			case EncodingKind.VEX:
				opKinds = OpCodeOperandKinds.VexOpKinds;
				op0Kind = opKinds[(int)(dword3 & (uint)VexFlags3.OpMask)];
				op1Kind = opKinds[(int)((dword3 >> (int)VexFlags3.Op1Shift) & (uint)VexFlags3.OpMask)];
				op2Kind = opKinds[(int)((dword3 >> (int)VexFlags3.Op2Shift) & (uint)VexFlags3.OpMask)];
				op3Kind = opKinds[(int)((dword3 >> (int)VexFlags3.Op3Shift) & (uint)VexFlags3.OpMask)];
				op4Kind = opKinds[(int)((dword3 >> (int)VexFlags3.Op4Shift) & (uint)VexFlags3.OpMask)];

				mandatoryPrefix = (MandatoryPrefixByte)((dword2 >> (int)VexFlags.MandatoryPrefixShift) & (uint)VexFlags.MandatoryPrefixMask) switch {
					MandatoryPrefixByte.None => (byte)MandatoryPrefix.PNP,
					MandatoryPrefixByte.P66 => (byte)MandatoryPrefix.P66,
					MandatoryPrefixByte.PF3 => (byte)MandatoryPrefix.PF3,
					MandatoryPrefixByte.PF2 => (byte)MandatoryPrefix.PF2,
					_ => throw new InvalidOperationException(),
				};

				table = (VexOpCodeTable)((dword2 >> (int)VexFlags.OpCodeTableShift) & (uint)VexFlags.OpCodeTableMask) switch {
					VexOpCodeTable.Table0F => (byte)OpCodeTableKind.T0F,
					VexOpCodeTable.Table0F38 => (byte)OpCodeTableKind.T0F38,
					VexOpCodeTable.Table0F3A => (byte)OpCodeTableKind.T0F3A,
					_ => throw new InvalidOperationException(),
				};

				groupIndex = (sbyte)((dword2 & (uint)VexFlags.HasGroupIndex) == 0 ? -1 : (int)((dword2 >> (int)VexFlags.GroupShift) & 7));
				tupleType = (byte)TupleType.None;

				flags |= (Encodable)((dword2 >> (int)VexFlags.EncodableShift) & (uint)VexFlags.EncodableMask) switch {
					Encodable.Any => Flags.Mode16 | Flags.Mode32 | Flags.Mode64,
					Encodable.Only1632 => Flags.Mode16 | Flags.Mode32,
					Encodable.Only64 => Flags.Mode64,
					_ => throw new InvalidOperationException(),
				};
				operandSize = 0;
				addressSize = 0;
				switch ((VexFlags)((dword2 >> (int)VexFlags.VEX_LShift) & (int)VexFlags.VEX_LMask)) {
				case VexFlags.LZ:
					lkind = LKind.LZ;
					l = 0;
					break;
				case VexFlags.L0:
					lkind = LKind.L0;
					l = 0;
					break;
				case VexFlags.L1:
					lkind = LKind.L0;
					l = 1;
					break;
				case VexFlags.L128:
					lkind = LKind.L128;
					l = 0;
					break;
				case VexFlags.L256:
					lkind = LKind.L128;
					l = 1;
					break;
				case VexFlags.LIG:
					lkind = LKind.None;
					l = 0;
					flags |= Flags.LIG;
					break;
				default:
					throw new InvalidOperationException();
				}

				if ((dword2 & (uint)VexFlags.VEX_W1) != 0)
					flags |= Flags.W;
				if ((dword2 & (uint)VexFlags.VEX_WIG) != 0)
					flags |= Flags.WIG;
				if ((dword2 & (uint)VexFlags.VEX_WIG32) != 0)
					flags |= Flags.WIG32;
				break;

			case EncodingKind.EVEX:
				opKinds = OpCodeOperandKinds.EvexOpKinds;
				op0Kind = opKinds[(int)(dword3 & (uint)EvexFlags3.OpMask)];
				op1Kind = opKinds[(int)((dword3 >> (int)EvexFlags3.Op1Shift) & (uint)EvexFlags3.OpMask)];
				op2Kind = opKinds[(int)((dword3 >> (int)EvexFlags3.Op2Shift) & (uint)EvexFlags3.OpMask)];
				op3Kind = opKinds[(int)((dword3 >> (int)EvexFlags3.Op3Shift) & (uint)EvexFlags3.OpMask)];

				mandatoryPrefix = (MandatoryPrefixByte)((dword2 >> (int)EvexFlags.MandatoryPrefixShift) & (uint)EvexFlags.MandatoryPrefixMask) switch {
					MandatoryPrefixByte.None => (byte)MandatoryPrefix.PNP,
					MandatoryPrefixByte.P66 => (byte)MandatoryPrefix.P66,
					MandatoryPrefixByte.PF3 => (byte)MandatoryPrefix.PF3,
					MandatoryPrefixByte.PF2 => (byte)MandatoryPrefix.PF2,
					_ => throw new InvalidOperationException(),
				};

				table = (EvexOpCodeTable)((dword2 >> (int)EvexFlags.OpCodeTableShift) & (uint)EvexFlags.OpCodeTableMask) switch {
					EvexOpCodeTable.Table0F => (byte)OpCodeTableKind.T0F,
					EvexOpCodeTable.Table0F38 => (byte)OpCodeTableKind.T0F38,
					EvexOpCodeTable.Table0F3A => (byte)OpCodeTableKind.T0F3A,
					_ => throw new InvalidOperationException(),
				};

				groupIndex = (sbyte)((dword2 & (uint)EvexFlags.HasGroupIndex) == 0 ? -1 : (int)((dword2 >> (int)EvexFlags.GroupShift) & 7));
				tupleType = (byte)((dword2 >> (int)EvexFlags.TupleTypeShift) & (uint)EvexFlags.TupleTypeMask);

				flags |= (Encodable)((dword2 >> (int)EvexFlags.EncodableShift) & (uint)EvexFlags.EncodableMask) switch {
					Encodable.Any => Flags.Mode16 | Flags.Mode32 | Flags.Mode64,
					Encodable.Only1632 => Flags.Mode16 | Flags.Mode32,
					Encodable.Only64 => Flags.Mode64,
					_ => throw new InvalidOperationException(),
				};
				operandSize = 0;
				addressSize = 0;
				l = (byte)((dword2 >> (int)EvexFlags.EVEX_LShift) & 3);

				if ((dword2 & (uint)EvexFlags.EVEX_W1) != 0)
					flags |= Flags.W;
				if ((dword2 & (uint)EvexFlags.EVEX_LIG) != 0)
					flags |= Flags.LIG;
				if ((dword2 & (uint)EvexFlags.EVEX_WIG) != 0)
					flags |= Flags.WIG;
				if ((dword2 & (uint)EvexFlags.EVEX_WIG32) != 0)
					flags |= Flags.WIG32;
				if ((dword2 & (uint)EvexFlags.EVEX_b) != 0)
					flags |= Flags.Broadcast;
				if ((dword2 & (uint)EvexFlags.EVEX_er) != 0)
					flags |= Flags.RoundingControl;
				if ((dword2 & (uint)EvexFlags.EVEX_sae) != 0)
					flags |= Flags.SuppressAllExceptions;
				if ((dword2 & (uint)EvexFlags.EVEX_k1) != 0)
					flags |= Flags.OpMaskRegister;
				if ((dword2 & (uint)EvexFlags.EVEX_z) != 0)
					flags |= Flags.ZeroingMasking;
				lkind = LKind.L128;
				switch (code) {
				case Code.EVEX_Vpgatherdd_xmm_k1_vm32x:
				case Code.EVEX_Vpgatherdd_ymm_k1_vm32y:
				case Code.EVEX_Vpgatherdd_zmm_k1_vm32z:
				case Code.EVEX_Vpgatherdq_xmm_k1_vm32x:
				case Code.EVEX_Vpgatherdq_ymm_k1_vm32x:
				case Code.EVEX_Vpgatherdq_zmm_k1_vm32y:
				case Code.EVEX_Vpgatherqd_xmm_k1_vm64x:
				case Code.EVEX_Vpgatherqd_xmm_k1_vm64y:
				case Code.EVEX_Vpgatherqd_ymm_k1_vm64z:
				case Code.EVEX_Vpgatherqq_xmm_k1_vm64x:
				case Code.EVEX_Vpgatherqq_ymm_k1_vm64y:
				case Code.EVEX_Vpgatherqq_zmm_k1_vm64z:
				case Code.EVEX_Vgatherdps_xmm_k1_vm32x:
				case Code.EVEX_Vgatherdps_ymm_k1_vm32y:
				case Code.EVEX_Vgatherdps_zmm_k1_vm32z:
				case Code.EVEX_Vgatherdpd_xmm_k1_vm32x:
				case Code.EVEX_Vgatherdpd_ymm_k1_vm32x:
				case Code.EVEX_Vgatherdpd_zmm_k1_vm32y:
				case Code.EVEX_Vgatherqps_xmm_k1_vm64x:
				case Code.EVEX_Vgatherqps_xmm_k1_vm64y:
				case Code.EVEX_Vgatherqps_ymm_k1_vm64z:
				case Code.EVEX_Vgatherqpd_xmm_k1_vm64x:
				case Code.EVEX_Vgatherqpd_ymm_k1_vm64y:
				case Code.EVEX_Vgatherqpd_zmm_k1_vm64z:
				case Code.EVEX_Vpscatterdd_vm32x_k1_xmm:
				case Code.EVEX_Vpscatterdd_vm32y_k1_ymm:
				case Code.EVEX_Vpscatterdd_vm32z_k1_zmm:
				case Code.EVEX_Vpscatterdq_vm32x_k1_xmm:
				case Code.EVEX_Vpscatterdq_vm32x_k1_ymm:
				case Code.EVEX_Vpscatterdq_vm32y_k1_zmm:
				case Code.EVEX_Vpscatterqd_vm64x_k1_xmm:
				case Code.EVEX_Vpscatterqd_vm64y_k1_xmm:
				case Code.EVEX_Vpscatterqd_vm64z_k1_ymm:
				case Code.EVEX_Vpscatterqq_vm64x_k1_xmm:
				case Code.EVEX_Vpscatterqq_vm64y_k1_ymm:
				case Code.EVEX_Vpscatterqq_vm64z_k1_zmm:
				case Code.EVEX_Vscatterdps_vm32x_k1_xmm:
				case Code.EVEX_Vscatterdps_vm32y_k1_ymm:
				case Code.EVEX_Vscatterdps_vm32z_k1_zmm:
				case Code.EVEX_Vscatterdpd_vm32x_k1_xmm:
				case Code.EVEX_Vscatterdpd_vm32x_k1_ymm:
				case Code.EVEX_Vscatterdpd_vm32y_k1_zmm:
				case Code.EVEX_Vscatterqps_vm64x_k1_xmm:
				case Code.EVEX_Vscatterqps_vm64y_k1_xmm:
				case Code.EVEX_Vscatterqps_vm64z_k1_ymm:
				case Code.EVEX_Vscatterqpd_vm64x_k1_xmm:
				case Code.EVEX_Vscatterqpd_vm64y_k1_ymm:
				case Code.EVEX_Vscatterqpd_vm64z_k1_zmm:
				case Code.EVEX_Vgatherpf0dps_vm32z_k1:
				case Code.EVEX_Vgatherpf0dpd_vm32y_k1:
				case Code.EVEX_Vgatherpf1dps_vm32z_k1:
				case Code.EVEX_Vgatherpf1dpd_vm32y_k1:
				case Code.EVEX_Vscatterpf0dps_vm32z_k1:
				case Code.EVEX_Vscatterpf0dpd_vm32y_k1:
				case Code.EVEX_Vscatterpf1dps_vm32z_k1:
				case Code.EVEX_Vscatterpf1dpd_vm32y_k1:
				case Code.EVEX_Vgatherpf0qps_vm64z_k1:
				case Code.EVEX_Vgatherpf0qpd_vm64z_k1:
				case Code.EVEX_Vgatherpf1qps_vm64z_k1:
				case Code.EVEX_Vgatherpf1qpd_vm64z_k1:
				case Code.EVEX_Vscatterpf0qps_vm64z_k1:
				case Code.EVEX_Vscatterpf0qpd_vm64z_k1:
				case Code.EVEX_Vscatterpf1qps_vm64z_k1:
				case Code.EVEX_Vscatterpf1qpd_vm64z_k1:
					flags |= Flags.NonZeroOpMaskRegister;
					break;
				}
				break;

			case EncodingKind.XOP:
				opKinds = OpCodeOperandKinds.XopOpKinds;
				op0Kind = opKinds[(int)(dword3 & (uint)XopFlags3.OpMask)];
				op1Kind = opKinds[(int)((dword3 >> (int)XopFlags3.Op1Shift) & (uint)XopFlags3.OpMask)];
				op2Kind = opKinds[(int)((dword3 >> (int)XopFlags3.Op2Shift) & (uint)XopFlags3.OpMask)];
				op3Kind = opKinds[(int)((dword3 >> (int)XopFlags3.Op3Shift) & (uint)XopFlags3.OpMask)];

				mandatoryPrefix = (MandatoryPrefixByte)((dword2 >> (int)XopFlags.MandatoryPrefixShift) & (uint)XopFlags.MandatoryPrefixMask) switch {
					MandatoryPrefixByte.None => (byte)MandatoryPrefix.PNP,
					MandatoryPrefixByte.P66 => (byte)MandatoryPrefix.P66,
					MandatoryPrefixByte.PF3 => (byte)MandatoryPrefix.PF3,
					MandatoryPrefixByte.PF2 => (byte)MandatoryPrefix.PF2,
					_ => throw new InvalidOperationException(),
				};

				table = (XopOpCodeTable)((dword2 >> (int)XopFlags.OpCodeTableShift) & (uint)XopFlags.OpCodeTableMask) switch {
					XopOpCodeTable.XOP8 => (byte)OpCodeTableKind.XOP8,
					XopOpCodeTable.XOP9 => (byte)OpCodeTableKind.XOP9,
					XopOpCodeTable.XOPA => (byte)OpCodeTableKind.XOPA,
					_ => throw new InvalidOperationException(),
				};

				groupIndex = (sbyte)((dword2 & (uint)XopFlags.HasGroupIndex) == 0 ? -1 : (int)((dword2 >> (int)XopFlags.GroupShift) & 7));
				tupleType = (byte)TupleType.None;

				flags |= (Encodable)((dword2 >> (int)XopFlags.EncodableShift) & (uint)XopFlags.EncodableMask) switch {
					Encodable.Any => Flags.Mode16 | Flags.Mode32 | Flags.Mode64,
					Encodable.Only1632 => Flags.Mode16 | Flags.Mode32,
					Encodable.Only64 => Flags.Mode64,
					_ => throw new InvalidOperationException(),
				};
				operandSize = 0;
				addressSize = 0;
				l = (byte)((dword2 >> (int)XopFlags.XOP_LShift) & 1);

				if ((dword2 & (uint)XopFlags.XOP_W1) != 0)
					flags |= Flags.W;
				if ((dword2 & (uint)XopFlags.XOP_WIG32) != 0)
					flags |= Flags.WIG32;
				lkind = (dword2 & (uint)XopFlags.XOP_L0_L1) != 0 ? LKind.L0 : LKind.L128;
				break;

			case EncodingKind.D3NOW:
				op0Kind = (byte)OpCodeOperandKind.mm_reg;
				op1Kind = (byte)OpCodeOperandKind.mm_or_mem;
				mandatoryPrefix = (byte)MandatoryPrefix.None;
				table = (byte)OpCodeTableKind.T0F;
				groupIndex = -1;
				tupleType = (byte)TupleType.None;

				flags |= (Encodable)((dword2 >> (int)D3nowFlags.EncodableShift) & (uint)D3nowFlags.EncodableMask) switch {
					Encodable.Any => Flags.Mode16 | Flags.Mode32 | Flags.Mode64,
					Encodable.Only1632 => Flags.Mode16 | Flags.Mode32,
					Encodable.Only64 => Flags.Mode64,
					_ => throw new InvalidOperationException(),
				};
				operandSize = 0;
				addressSize = 0;
				l = 0;
				lkind = LKind.None;
				break;

			default:
				throw new InvalidOperationException();
			}

			toOpCodeStringValue = new OpCodeFormatter(this, sb, lkind).Format();
			toInstructionStringValue = new InstructionFormatter(this, sb).Format();
		}

		/// <summary>
		/// Gets the code
		/// </summary>
		public Code Code => (Code)code;

		/// <summary>
		/// Gets the encoding
		/// </summary>
		public EncodingKind Encoding => (EncodingKind)encoding;

		/// <summary>
		/// true if it's an instruction, false if it's eg. <see cref="Code.INVALID"/>, db, dw, dd, dq
		/// </summary>
		public bool IsInstruction => (flags & Flags.IsInstruction) != 0;

		/// <summary>
		/// true if it's an instruction available in 16-bit mode
		/// </summary>
		public bool Mode16 => (flags & Flags.Mode16) != 0;

		/// <summary>
		/// true if it's an instruction available in 32-bit mode
		/// </summary>
		public bool Mode32 => (flags & Flags.Mode32) != 0;

		/// <summary>
		/// true if it's an instruction available in 64-bit mode
		/// </summary>
		public bool Mode64 => (flags & Flags.Mode64) != 0;

		/// <summary>
		/// true if an fwait (9B) instruction is added before the instruction
		/// </summary>
		public bool Fwait => (flags & Flags.Fwait) != 0;

		/// <summary>
		/// (Legacy encoding) Gets the required operand size (16,32,64) or 0 if no operand size prefix (66) or REX.W prefix is needed
		/// </summary>
		public int OperandSize => operandSize;

		/// <summary>
		/// (Legacy encoding) Gets the required address size (16,32,64) or 0 if no address size prefix (67) is needed
		/// </summary>
		public int AddressSize => addressSize;

		/// <summary>
		/// (VEX/XOP/EVEX) L / L'L value or default value if <see cref="IsLIG"/> is true
		/// </summary>
		public uint L => l;

		/// <summary>
		/// (VEX/XOP/EVEX) W value or default value if <see cref="IsWIG"/> or <see cref="IsWIG32"/> is true
		/// </summary>
		public uint W => (flags & Flags.W) != 0 ? 1U : 0;

		/// <summary>
		/// (VEX/XOP/EVEX) true if the L / L'L fields are ignored.
		/// EVEX: if reg-only ops and {er} (EVEX.b is set), L'L is the rounding control and not ignored.
		/// </summary>
		public bool IsLIG => (flags & Flags.LIG) != 0;

		/// <summary>
		/// (VEX/XOP/EVEX) true if the W field is ignored in 16/32/64-bit modes
		/// </summary>
		public bool IsWIG => (flags & Flags.WIG) != 0;

		/// <summary>
		/// (VEX/XOP/EVEX) true if the W field is ignored in 16/32-bit modes (but not 64-bit mode)
		/// </summary>
		public bool IsWIG32 => (flags & Flags.WIG32) != 0;

		/// <summary>
		/// (EVEX) Gets the tuple type
		/// </summary>
		public TupleType TupleType => (TupleType)tupleType;

		/// <summary>
		/// (EVEX) true if the instruction supports broadcasting (EVEX.b bit) (if it has a memory operand)
		/// </summary>
		public bool CanBroadcast => (flags & Flags.Broadcast) != 0;

		/// <summary>
		/// (EVEX) true if the instruction supports rounding control
		/// </summary>
		public bool CanUseRoundingControl => (flags & Flags.RoundingControl) != 0;

		/// <summary>
		/// (EVEX) true if the instruction supports suppress all exceptions
		/// </summary>
		public bool CanSuppressAllExceptions => (flags & Flags.SuppressAllExceptions) != 0;

		/// <summary>
		/// (EVEX) true if an opmask register can be used
		/// </summary>
		public bool CanUseOpMaskRegister => (flags & Flags.OpMaskRegister) != 0;

		/// <summary>
		/// (EVEX) true if a non-zero opmask register must be used
		/// </summary>
		public bool RequireNonZeroOpMaskRegister => (flags & Flags.NonZeroOpMaskRegister) != 0;

		/// <summary>
		/// (EVEX) true if the instruction supports zeroing masking if opmask register k1-k7 is used
		/// </summary>
		public bool CanUseZeroingMasking => (flags & Flags.ZeroingMasking) != 0;

		/// <summary>
		/// true if the LOCK (F0) prefix can be used
		/// </summary>
		public bool CanUseLockPrefix => (flags & Flags.LockPrefix) != 0;

		/// <summary>
		/// true if the XACQUIRE (F2) prefix can be used
		/// </summary>
		public bool CanUseXacquirePrefix => (flags & Flags.XacquirePrefix) != 0;

		/// <summary>
		/// true if the XRELEASE (F3) prefix can be used
		/// </summary>
		public bool CanUseXreleasePrefix => (flags & Flags.XreleasePrefix) != 0;

		/// <summary>
		/// true if the REP / REPE (F3) prefixes can be used
		/// </summary>
		public bool CanUseRepPrefix => (flags & Flags.RepPrefix) != 0;

		/// <summary>
		/// true if the REPNE (F2) prefix can be used
		/// </summary>
		public bool CanUseRepnePrefix => (flags & Flags.RepnePrefix) != 0;

		/// <summary>
		/// true if the BND (F2) prefix can be used
		/// </summary>
		public bool CanUseBndPrefix => (flags & Flags.BndPrefix) != 0;

		/// <summary>
		/// true if the HINT-TAKEN (3E) and HINT-NOT-TAKEN (2E) prefixes can be used
		/// </summary>
		public bool CanUseHintTakenPrefix => (flags & Flags.HintTakenPrefix) != 0;

		/// <summary>
		/// true if the NOTRACK (3E) prefix can be used
		/// </summary>
		public bool CanUseNotrackPrefix => (flags & Flags.NotrackPrefix) != 0;

		/// <summary>
		/// Gets the opcode table
		/// </summary>
		public OpCodeTableKind Table => (OpCodeTableKind)table;

		/// <summary>
		/// Gets the mandatory prefix
		/// </summary>
		public MandatoryPrefix MandatoryPrefix => (MandatoryPrefix)mandatoryPrefix;

		/// <summary>
		/// Gets the opcode. 00000000xxh if it's 1-byte, 0000yyxxh if it's 2-byte (yy != 00, and yy is the first byte and xx the second byte).
		/// It doesn't include the table value, see <see cref="Table"/>.
		/// Example values: 0xDFC0 (<see cref="Code.Ffreep_sti"/>), 0x01D8 (<see cref="Code.Vmrunw"/>), 0x2A (<see cref="Code.Sub_r8_rm8"/>, <see cref="Code.Cvtpi2ps_xmm_mmm64"/>, etc).
		/// </summary>
		public uint OpCode => opCode;

		/// <summary>
		/// true if it's part of a group
		/// </summary>
		public bool IsGroup => GroupIndex >= 0;

		/// <summary>
		/// Group index (0-7) or -1. If it's 0-7, it's stored in the 'reg' field of the modrm byte.
		/// </summary>
		public int GroupIndex => groupIndex;

		/// <summary>
		/// Gets the number of operands
		/// </summary>
		public int OpCount => InstructionOpCounts.OpCount[code];

		/// <summary>
		/// Gets operand #0's opkind
		/// </summary>
		public OpCodeOperandKind Op0Kind => (OpCodeOperandKind)op0Kind;

		/// <summary>
		/// Gets operand #1's opkind
		/// </summary>
		public OpCodeOperandKind Op1Kind => (OpCodeOperandKind)op1Kind;

		/// <summary>
		/// Gets operand #2's opkind
		/// </summary>
		public OpCodeOperandKind Op2Kind => (OpCodeOperandKind)op2Kind;

		/// <summary>
		/// Gets operand #3's opkind
		/// </summary>
		public OpCodeOperandKind Op3Kind => (OpCodeOperandKind)op3Kind;

		/// <summary>
		/// Gets operand #4's opkind
		/// </summary>
		public OpCodeOperandKind Op4Kind => (OpCodeOperandKind)op4Kind;

		/// <summary>
		/// Gets an operand's opkind
		/// </summary>
		/// <param name="operand">Operand number</param>
		/// <returns></returns>
		public OpCodeOperandKind GetOpKind(int operand) {
			switch (operand) {
			case 0: return Op0Kind;
			case 1: return Op1Kind;
			case 2: return Op2Kind;
			case 3: return Op3Kind;
			case 4: return Op4Kind;
			default: throw new ArgumentOutOfRangeException(nameof(operand));
			}
		}

		/// <summary>
		/// Gets the opcode string, eg. "VEX.128.66.0F38.W0 78 /r", see also <see cref="ToInstructionString()"/>
		/// </summary>
		/// <returns></returns>
		public string ToOpCodeString() => toOpCodeStringValue;

		/// <summary>
		/// Gets the instruction string, eg. "VPBROADCASTB xmm1, xmm2/m8", see also <see cref="ToOpCodeString()"/>
		/// </summary>
		/// <returns></returns>
		public string ToInstructionString() => toInstructionStringValue;

		/// <summary>
		/// Gets the instruction string, eg. "VPBROADCASTB xmm1, xmm2/m8", see also <see cref="ToOpCodeString()"/>
		/// </summary>
		/// <returns></returns>
		public override string ToString() => ToInstructionString();
	}
}
#endif
