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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Iced.Intel;
using Xunit;

namespace Iced.UnitTests.Intel.DecoderTests {
	public abstract class DecoderTest {
		(Decoder decoder, int length, bool canRead, ByteArrayCodeReader codeReader) CreateDecoder(int bitness, string hexBytes, DecoderOptions options) {
			var codeReader = new ByteArrayCodeReader(hexBytes);
			var decoder = Decoder.Create(bitness, codeReader, options);
			decoder.IP = bitness switch {
				16 => DecoderConstants.DEFAULT_IP16,
				32 => DecoderConstants.DEFAULT_IP32,
				64 => DecoderConstants.DEFAULT_IP64,
				_ => throw new ArgumentOutOfRangeException(nameof(bitness)),
			};
			Assert.Equal(bitness, decoder.Bitness);
			int length = Math.Min(IcedConstants.MaxInstructionLength, codeReader.Count);
			bool canRead = length < codeReader.Count;
			return (decoder, length, canRead, codeReader);
		}

		protected void DecodeMemOpsBase(int bitness, string hexBytes, Code code, Register register, Register prefixSeg, Register segReg, Register baseReg, Register indexReg, int scale, uint displ, int displSize, in ConstantOffsets constantOffsets, string encodedHexBytes, DecoderOptions options) {
			var (decoder, length, canRead, codeReader) = CreateDecoder(bitness, hexBytes, options);
			var instruction = decoder.Decode();
			Assert.False(decoder.InvalidNoMoreBytes);
			Assert.Equal(canRead, codeReader.CanReadByte);

			Assert.Equal(code, instruction.Code);
			Assert.Equal(2, instruction.OpCount);
			Assert.Equal(length, instruction.Length);
			Assert.False(instruction.HasRepPrefix);
			Assert.False(instruction.HasRepePrefix);
			Assert.False(instruction.HasRepnePrefix);
			Assert.False(instruction.HasLockPrefix);
			Assert.Equal(prefixSeg, instruction.SegmentPrefix);
			if (instruction.SegmentPrefix == Register.None)
				Assert.False(instruction.HasSegmentPrefix);
			else
				Assert.True(instruction.HasSegmentPrefix);

			Assert.Equal(OpKind.Memory, instruction.Op0Kind);
			Assert.Equal(segReg, instruction.MemorySegment);
			Assert.Equal(baseReg, instruction.MemoryBase);
			Assert.Equal(indexReg, instruction.MemoryIndex);
			Assert.Equal(displ, instruction.MemoryDisplacement);
			Assert.Equal((ulong)(int)displ, instruction.MemoryDisplacement64);
			Assert.Equal(1 << scale, instruction.MemoryIndexScale);
			Assert.Equal(displSize, instruction.MemoryDisplSize);

			Assert.Equal(OpKind.Register, instruction.Op1Kind);
			Assert.Equal(register, instruction.Op1Register);
			VerifyConstantOffsets(constantOffsets, decoder.GetConstantOffsets(instruction));
		}

		protected static IEnumerable<object[]> GetMemOpsData(int bitness) {
			var allTestCases = DecoderTestCases.GetMemoryTestCases(bitness);
			foreach (var tc in allTestCases)
				yield return new object[13] { tc.HexBytes, tc.Code, tc.Register, tc.SegmentPrefix, tc.SegmentRegister, tc.BaseRegister, tc.IndexRegister, tc.Scale, tc.Displacement, tc.DisplacementSize, tc.ConstantOffsets, tc.EncodedHexBytes, tc.DecoderOptions };
		}

		protected static IEnumerable<object[]> GetDecoderTestData(int bitness) {
			var allTestCases = DecoderTestCases.GetTestCases(bitness);
			object boxedBitness = bitness;
			foreach (var tc in allTestCases) {
				Debug.Assert(bitness == tc.Bitness);
				yield return new object[4] { boxedBitness, tc.LineNumber, tc.HexBytes, tc };
			}
		}

		protected static IEnumerable<object[]> GetMiscDecoderTestData(int bitness) {
			var allTestCases = DecoderTestCases.GetMiscTestCases(bitness);
			object boxedBitness = bitness;
			foreach (var tc in allTestCases) {
				Debug.Assert(bitness == tc.Bitness);
				yield return new object[4] { boxedBitness, tc.LineNumber, tc.HexBytes, tc };
			}
		}

		internal void DecoderTestBase(int bitness, int lineNo, string hexBytes, DecoderTestCase tc) {
			var (decoder, length, canRead, codeReader) = CreateDecoder(bitness, hexBytes, tc.DecoderOptions);
			ulong rip = decoder.IP;
			decoder.Decode(out var instruction);
			Assert.Equal(tc.InvalidNoMoreBytes, decoder.InvalidNoMoreBytes);
			Assert.Equal(canRead, codeReader.CanReadByte);
			Assert.Equal(tc.Code, instruction.Code);
			Assert.Equal(tc.Mnemonic, instruction.Mnemonic);
			Assert.Equal(instruction.Mnemonic, instruction.Code.Mnemonic());
			Assert.Equal(length, instruction.Length);
			Assert.Equal(rip, instruction.IP);
			Assert.Equal(decoder.IP, instruction.NextIP);
			Assert.Equal(tc.OpCount, instruction.OpCount);
			Assert.Equal(tc.ZeroingMasking, instruction.ZeroingMasking);
			Assert.Equal(!tc.ZeroingMasking, instruction.MergingMasking);
			Assert.Equal(tc.SuppressAllExceptions, instruction.SuppressAllExceptions);
			Assert.Equal(tc.IsBroadcast, instruction.IsBroadcast);
			Assert.Equal(tc.HasXacquirePrefix, instruction.HasXacquirePrefix);
			Assert.Equal(tc.HasXreleasePrefix, instruction.HasXreleasePrefix);
			Assert.Equal(tc.HasRepePrefix, instruction.HasRepPrefix);
			Assert.Equal(tc.HasRepePrefix, instruction.HasRepePrefix);
			Assert.Equal(tc.HasRepnePrefix, instruction.HasRepnePrefix);
			Assert.Equal(tc.HasLockPrefix, instruction.HasLockPrefix);
			switch (tc.VsibBitness) {
			case 0:
				Assert.False(instruction.IsVsib);
				Assert.False(instruction.IsVsib32);
				Assert.False(instruction.IsVsib64);
				Assert.False(instruction.TryGetVsib64(out _));
				break;

			case 32:
				Assert.True(instruction.IsVsib);
				Assert.True(instruction.IsVsib32);
				Assert.False(instruction.IsVsib64);
				Assert.True(instruction.TryGetVsib64(out bool vsib64) && !vsib64);
				break;

			case 64:
				Assert.True(instruction.IsVsib);
				Assert.False(instruction.IsVsib32);
				Assert.True(instruction.IsVsib64);
				Assert.True(instruction.TryGetVsib64(out vsib64) && vsib64);
				break;

			default:
				throw new InvalidOperationException();
			}
			Assert.Equal(tc.OpMask, instruction.OpMask);
			Assert.Equal(tc.OpMask != Register.None, instruction.HasOpMask);
			Assert.Equal(tc.RoundingControl, instruction.RoundingControl);
			Assert.Equal(tc.SegmentPrefix, instruction.SegmentPrefix);
			if (instruction.SegmentPrefix == Register.None)
				Assert.False(instruction.HasSegmentPrefix);
			else
				Assert.True(instruction.HasSegmentPrefix);
			for (int i = 0; i < tc.OpCount; i++) {
				var opKind = tc.GetOpKind(i);
				Assert.Equal(opKind, instruction.GetOpKind(i));
				switch (opKind) {
				case OpKind.Register:
					Assert.Equal(tc.GetOpRegister(i), instruction.GetOpRegister(i));
					break;

				case OpKind.NearBranch16:
					Assert.Equal(tc.NearBranch, instruction.NearBranch16);
					break;

				case OpKind.NearBranch32:
					Assert.Equal(tc.NearBranch, instruction.NearBranch32);
					break;

				case OpKind.NearBranch64:
					Assert.Equal(tc.NearBranch, instruction.NearBranch64);
					break;

				case OpKind.FarBranch16:
					Assert.Equal(tc.FarBranch, instruction.FarBranch16);
					Assert.Equal(tc.FarBranchSelector, instruction.FarBranchSelector);
					break;

				case OpKind.FarBranch32:
					Assert.Equal(tc.FarBranch, instruction.FarBranch32);
					Assert.Equal(tc.FarBranchSelector, instruction.FarBranchSelector);
					break;

				case OpKind.Immediate8:
					Assert.Equal((byte)tc.Immediate, instruction.Immediate8);
					break;

				case OpKind.Immediate8_2nd:
					Assert.Equal(tc.Immediate_2nd, instruction.Immediate8_2nd);
					break;

				case OpKind.Immediate16:
					Assert.Equal((ushort)tc.Immediate, instruction.Immediate16);
					break;

				case OpKind.Immediate32:
					Assert.Equal((uint)tc.Immediate, instruction.Immediate32);
					break;

				case OpKind.Immediate64:
					Assert.Equal(tc.Immediate, instruction.Immediate64);
					break;

				case OpKind.Immediate8to16:
					Assert.Equal((short)tc.Immediate, instruction.Immediate8to16);
					break;

				case OpKind.Immediate8to32:
					Assert.Equal((int)tc.Immediate, instruction.Immediate8to32);
					break;

				case OpKind.Immediate8to64:
					Assert.Equal((long)tc.Immediate, instruction.Immediate8to64);
					break;

				case OpKind.Immediate32to64:
					Assert.Equal((long)tc.Immediate, instruction.Immediate32to64);
					break;

				case OpKind.MemorySegSI:
				case OpKind.MemorySegESI:
				case OpKind.MemorySegRSI:
				case OpKind.MemorySegDI:
				case OpKind.MemorySegEDI:
				case OpKind.MemorySegRDI:
					Assert.Equal(tc.MemorySegment, instruction.MemorySegment);
					Assert.Equal(tc.MemorySize, instruction.MemorySize);
					break;

				case OpKind.MemoryESDI:
				case OpKind.MemoryESEDI:
				case OpKind.MemoryESRDI:
					Assert.Equal(tc.MemorySize, instruction.MemorySize);
					break;

				case OpKind.Memory64:
					Assert.Equal(tc.MemorySegment, instruction.MemorySegment);
					Assert.Equal(tc.MemoryAddress64, instruction.MemoryAddress64);
					Assert.Equal(tc.MemorySize, instruction.MemorySize);
					break;

				case OpKind.Memory:
					Assert.Equal(tc.MemorySegment, instruction.MemorySegment);
					Assert.Equal(tc.MemoryBase, instruction.MemoryBase);
					Assert.Equal(tc.MemoryIndex, instruction.MemoryIndex);
					Assert.Equal(tc.MemoryIndexScale, instruction.MemoryIndexScale);
					Assert.Equal(tc.MemoryDisplacement, instruction.MemoryDisplacement);
					Assert.Equal((ulong)(int)tc.MemoryDisplacement, instruction.MemoryDisplacement64);
					Assert.Equal(tc.MemoryDisplSize, instruction.MemoryDisplSize);
					Assert.Equal(tc.MemorySize, instruction.MemorySize);
					break;

				default:
					throw new InvalidOperationException();
				}
			}
			if (tc.OpCount >= 1) {
				Assert.Equal(tc.Op0Kind, instruction.Op0Kind);
				if (tc.Op0Kind == OpKind.Register)
					Assert.Equal(tc.Op0Register, instruction.Op0Register);
				if (tc.OpCount >= 2) {
					Assert.Equal(tc.Op1Kind, instruction.Op1Kind);
					if (tc.Op1Kind == OpKind.Register)
						Assert.Equal(tc.Op1Register, instruction.Op1Register);
					if (tc.OpCount >= 3) {
						Assert.Equal(tc.Op2Kind, instruction.Op2Kind);
						if (tc.Op2Kind == OpKind.Register)
							Assert.Equal(tc.Op2Register, instruction.Op2Register);
						if (tc.OpCount >= 4) {
							Assert.Equal(tc.Op3Kind, instruction.Op3Kind);
							if (tc.Op3Kind == OpKind.Register)
								Assert.Equal(tc.Op3Register, instruction.Op3Register);
							if (tc.OpCount >= 5) {
								Assert.Equal(tc.Op4Kind, instruction.Op4Kind);
								if (tc.Op4Kind == OpKind.Register)
									Assert.Equal(tc.Op4Register, instruction.Op4Register);
								Assert.Equal(5, tc.OpCount);
							}
						}
					}
				}
			}
			VerifyConstantOffsets(tc.ConstantOffsets, decoder.GetConstantOffsets(instruction));
		}

		protected static void VerifyConstantOffsets(in ConstantOffsets expected, in ConstantOffsets actual) {
			Assert.Equal(expected.ImmediateOffset, actual.ImmediateOffset);
			Assert.Equal(expected.ImmediateSize, actual.ImmediateSize);
			Assert.Equal(expected.ImmediateOffset2, actual.ImmediateOffset2);
			Assert.Equal(expected.ImmediateSize2, actual.ImmediateSize2);
			Assert.Equal(expected.DisplacementOffset, actual.DisplacementOffset);
			Assert.Equal(expected.DisplacementSize, actual.DisplacementSize);
		}
	}
}
