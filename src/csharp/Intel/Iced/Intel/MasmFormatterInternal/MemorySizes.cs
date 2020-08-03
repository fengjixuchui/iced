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

#if MASM
using System;
using Iced.Intel.FormatterInternal;

namespace Iced.Intel.MasmFormatterInternal {
	static class MemorySizes {
#pragma warning disable CS8618 // Non-nullable field 'dword_ptr' is uninitialized. Consider declaring the field as nullable.
		internal static FormatterString[] dword_ptr;
		internal static FormatterString[] qword_ptr;
		internal static FormatterString[] mmword_ptr;
		internal static FormatterString[] xmmword_ptr;
		internal static FormatterString[] oword_ptr;
#pragma warning restore CS8618

		public readonly struct Info {
			public readonly bool isBroadcast;
			public readonly int size;
			public readonly FormatterString[] keywords;
			public Info(bool isBroadcast, int size, FormatterString[] keywords) {
				this.isBroadcast = isBroadcast;
				this.size = size;
				this.keywords = keywords;
			}
		}

		public static readonly Info[] AllMemorySizes = GetMemorySizes();
		enum Size {
			S0,
			S1,
			S2,
			S4,
			S6,
			S8,
			S10,
			S14,
			S16,
			S28,
			S32,
			S64,
			S94,
			S108,
			S512,
		}
		enum MemoryKeywords {
			None,
			byte_ptr,
			dword_bcst,
			dword_ptr,
			fpuenv14_ptr,
			fpuenv28_ptr,
			fpustate108_ptr,
			fpustate94_ptr,
			fword_ptr,
			oword_ptr,
			qword_bcst,
			qword_ptr,
			tbyte_ptr,
			word_ptr,
			xmmword_ptr,
			ymmword_ptr,
			zmmword_ptr,
		}
		static Info[] GetMemorySizes() {
			var ptr = new FormatterString("ptr");
			var dword_ptr = new[] { new FormatterString("dword"), ptr };
			MemorySizes.dword_ptr = dword_ptr;
			var qword_ptr = new[] { new FormatterString("qword"), ptr };
			MemorySizes.qword_ptr = qword_ptr;
			var mmword_ptr = new[] { new FormatterString("mmword"), ptr };
			MemorySizes.mmword_ptr = mmword_ptr;
			var xmmword_ptr = new[] { new FormatterString("xmmword"), ptr };
			MemorySizes.xmmword_ptr = xmmword_ptr;
			var oword_ptr = new[] { new FormatterString("oword"), ptr };
			MemorySizes.oword_ptr = oword_ptr;

			var byte_ptr = new[] { new FormatterString("byte"), ptr };
			var word_ptr = new[] { new FormatterString("word"), ptr };
			var ymmword_ptr = new[] { new FormatterString("ymmword"), ptr };
			var zmmword_ptr = new[] { new FormatterString("zmmword"), ptr };
			var fword_ptr = new[] { new FormatterString("fword"), ptr };
			var tbyte_ptr = new[] { new FormatterString("tbyte"), ptr };
			var fpuenv14_ptr = new[] { new FormatterString("fpuenv14"), ptr };
			var fpuenv28_ptr = new[] { new FormatterString("fpuenv28"), ptr };
			var fpustate108_ptr = new[] { new FormatterString("fpustate108"), ptr };
			var fpustate94_ptr = new[] { new FormatterString("fpustate94"), ptr };
			var dword_bcst = new[] { new FormatterString("dword"), new FormatterString("bcst") };
			var qword_bcst = new[] { new FormatterString("qword"), new FormatterString("bcst") };

			var infos = new Info[IcedConstants.NumberOfMemorySizes];
			const int SizeKindShift = 5;
			const int MemoryKeywordsMask = 0x1F;
			var data = new ushort[IcedConstants.NumberOfMemorySizes] {
				// GENERATOR-BEGIN: MemorySizes
				// ⚠️This was generated by GENERATOR!🦹‍♂️
				(ushort)((uint)MemoryKeywords.None | ((uint)Size.S0 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.byte_ptr | ((uint)Size.S1 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.word_ptr | ((uint)Size.S2 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.dword_ptr | ((uint)Size.S4 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.qword_ptr | ((uint)Size.S8 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.qword_ptr | ((uint)Size.S8 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.xmmword_ptr | ((uint)Size.S16 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.ymmword_ptr | ((uint)Size.S32 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.zmmword_ptr | ((uint)Size.S64 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.byte_ptr | ((uint)Size.S1 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.word_ptr | ((uint)Size.S2 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.dword_ptr | ((uint)Size.S4 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.qword_ptr | ((uint)Size.S8 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.xmmword_ptr | ((uint)Size.S16 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.ymmword_ptr | ((uint)Size.S32 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.zmmword_ptr | ((uint)Size.S64 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.dword_ptr | ((uint)Size.S4 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.fword_ptr | ((uint)Size.S6 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.tbyte_ptr | ((uint)Size.S10 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.word_ptr | ((uint)Size.S2 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.dword_ptr | ((uint)Size.S4 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.qword_ptr | ((uint)Size.S8 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.dword_ptr | ((uint)Size.S4 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.qword_ptr | ((uint)Size.S8 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.qword_ptr | ((uint)Size.S8 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.oword_ptr | ((uint)Size.S16 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.fword_ptr | ((uint)Size.S6 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.fword_ptr | ((uint)Size.S10 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.word_ptr | ((uint)Size.S2 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.dword_ptr | ((uint)Size.S4 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.qword_ptr | ((uint)Size.S8 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.tbyte_ptr | ((uint)Size.S10 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.xmmword_ptr | ((uint)Size.S16 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.word_ptr | ((uint)Size.S2 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.fpuenv14_ptr | ((uint)Size.S14 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.fpuenv28_ptr | ((uint)Size.S28 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.fpustate94_ptr | ((uint)Size.S94 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.fpustate108_ptr | ((uint)Size.S108 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.None | ((uint)Size.S512 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.None | ((uint)Size.S512 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.None | ((uint)Size.S0 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.None | ((uint)Size.S0 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.tbyte_ptr | ((uint)Size.S10 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.None | ((uint)Size.S64 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.None | ((uint)Size.S0 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.word_ptr | ((uint)Size.S2 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.word_ptr | ((uint)Size.S2 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.dword_ptr | ((uint)Size.S4 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.dword_ptr | ((uint)Size.S4 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.dword_ptr | ((uint)Size.S4 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.dword_ptr | ((uint)Size.S4 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.dword_ptr | ((uint)Size.S4 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.qword_ptr | ((uint)Size.S8 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.qword_ptr | ((uint)Size.S8 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.qword_ptr | ((uint)Size.S8 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.qword_ptr | ((uint)Size.S8 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.qword_ptr | ((uint)Size.S8 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.qword_ptr | ((uint)Size.S8 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.qword_ptr | ((uint)Size.S8 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.qword_ptr | ((uint)Size.S8 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.xmmword_ptr | ((uint)Size.S16 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.xmmword_ptr | ((uint)Size.S16 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.xmmword_ptr | ((uint)Size.S16 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.xmmword_ptr | ((uint)Size.S16 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.xmmword_ptr | ((uint)Size.S16 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.xmmword_ptr | ((uint)Size.S16 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.xmmword_ptr | ((uint)Size.S16 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.xmmword_ptr | ((uint)Size.S16 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.xmmword_ptr | ((uint)Size.S16 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.xmmword_ptr | ((uint)Size.S16 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.xmmword_ptr | ((uint)Size.S16 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.xmmword_ptr | ((uint)Size.S16 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.xmmword_ptr | ((uint)Size.S16 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.ymmword_ptr | ((uint)Size.S32 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.ymmword_ptr | ((uint)Size.S32 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.ymmword_ptr | ((uint)Size.S32 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.ymmword_ptr | ((uint)Size.S32 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.ymmword_ptr | ((uint)Size.S32 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.ymmword_ptr | ((uint)Size.S32 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.ymmword_ptr | ((uint)Size.S32 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.ymmword_ptr | ((uint)Size.S32 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.ymmword_ptr | ((uint)Size.S32 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.ymmword_ptr | ((uint)Size.S32 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.ymmword_ptr | ((uint)Size.S32 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.ymmword_ptr | ((uint)Size.S32 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.ymmword_ptr | ((uint)Size.S32 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.ymmword_ptr | ((uint)Size.S32 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.ymmword_ptr | ((uint)Size.S32 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.ymmword_ptr | ((uint)Size.S32 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.zmmword_ptr | ((uint)Size.S64 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.zmmword_ptr | ((uint)Size.S64 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.zmmword_ptr | ((uint)Size.S64 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.zmmword_ptr | ((uint)Size.S64 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.zmmword_ptr | ((uint)Size.S64 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.zmmword_ptr | ((uint)Size.S64 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.zmmword_ptr | ((uint)Size.S64 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.zmmword_ptr | ((uint)Size.S64 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.zmmword_ptr | ((uint)Size.S64 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.zmmword_ptr | ((uint)Size.S64 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.zmmword_ptr | ((uint)Size.S64 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.zmmword_ptr | ((uint)Size.S64 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.zmmword_ptr | ((uint)Size.S64 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.dword_bcst | ((uint)Size.S4 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.dword_bcst | ((uint)Size.S4 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.dword_bcst | ((uint)Size.S4 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.dword_bcst | ((uint)Size.S4 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.dword_bcst | ((uint)Size.S4 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.qword_bcst | ((uint)Size.S8 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.qword_bcst | ((uint)Size.S8 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.qword_bcst | ((uint)Size.S8 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.dword_bcst | ((uint)Size.S4 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.qword_bcst | ((uint)Size.S8 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.dword_bcst | ((uint)Size.S4 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.dword_bcst | ((uint)Size.S4 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.qword_bcst | ((uint)Size.S8 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.qword_bcst | ((uint)Size.S8 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.qword_bcst | ((uint)Size.S8 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.dword_bcst | ((uint)Size.S4 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.qword_bcst | ((uint)Size.S8 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.dword_bcst | ((uint)Size.S4 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.dword_bcst | ((uint)Size.S4 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.qword_bcst | ((uint)Size.S8 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.qword_bcst | ((uint)Size.S8 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.qword_bcst | ((uint)Size.S8 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.dword_bcst | ((uint)Size.S4 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.qword_bcst | ((uint)Size.S8 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.dword_bcst | ((uint)Size.S4 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.dword_bcst | ((uint)Size.S4 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.dword_bcst | ((uint)Size.S4 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.qword_bcst | ((uint)Size.S8 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.qword_bcst | ((uint)Size.S8 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.qword_bcst | ((uint)Size.S8 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.qword_bcst | ((uint)Size.S8 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.qword_bcst | ((uint)Size.S8 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.qword_bcst | ((uint)Size.S8 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.dword_bcst | ((uint)Size.S4 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.dword_bcst | ((uint)Size.S4 << SizeKindShift)),
				(ushort)((uint)MemoryKeywords.dword_bcst | ((uint)Size.S4 << SizeKindShift)),
				// GENERATOR-END: MemorySizes
			};
			var sizes = new ushort[] {
				0,
				1,
				2,
				4,
				6,
				8,
				10,
				14,
				16,
				28,
				32,
				64,
				94,
				108,
				512,
			};

			for (int i = 0; i < infos.Length; i++) {
				var d = data[i];
				var keywords = ((MemoryKeywords)(d & MemoryKeywordsMask)) switch {
					MemoryKeywords.None => Array2.Empty<FormatterString>(),
					MemoryKeywords.byte_ptr => byte_ptr,
					MemoryKeywords.dword_bcst => dword_bcst,
					MemoryKeywords.dword_ptr => dword_ptr,
					MemoryKeywords.fpuenv14_ptr => fpuenv14_ptr,
					MemoryKeywords.fpuenv28_ptr => fpuenv28_ptr,
					MemoryKeywords.fpustate108_ptr => fpustate108_ptr,
					MemoryKeywords.fpustate94_ptr => fpustate94_ptr,
					MemoryKeywords.fword_ptr => fword_ptr,
					MemoryKeywords.oword_ptr => oword_ptr,
					MemoryKeywords.qword_bcst => qword_bcst,
					MemoryKeywords.qword_ptr => qword_ptr,
					MemoryKeywords.tbyte_ptr => tbyte_ptr,
					MemoryKeywords.word_ptr => word_ptr,
					MemoryKeywords.xmmword_ptr => xmmword_ptr,
					MemoryKeywords.ymmword_ptr => ymmword_ptr,
					MemoryKeywords.zmmword_ptr => zmmword_ptr,
					_ => throw new InvalidOperationException(),
				};
				infos[i] = new Info(i >= (int)IcedConstants.FirstBroadcastMemorySize, sizes[d >> SizeKindShift], keywords);
			}

			return infos;
		}
	}
}
#endif
