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

#if ENCODER && BLOCK_ENCODER
using System;
using Iced.Intel;
using Xunit;

namespace Iced.UnitTests.Intel.EncoderTests {
	public sealed class BlockEncoderTest16_br8 : BlockEncoderTest {
		const int bitness = 16;
		const ulong origRip = 0x8000;
		const ulong newRip = 0xF000;

		[Fact]
		void Br8_fwd() {
			var originalData = new byte[] {
				/*0000*/ 0xB0, 0x00,// mov al,0
				/*0002*/ 0xE2, 0x22,// loop 8026h
				/*0004*/ 0xB0, 0x01,// mov al,1
				/*0006*/ 0x67, 0xE2, 0x1D,// loop 8026h
				/*0009*/ 0xB0, 0x02,// mov al,2
				/*000B*/ 0xE1, 0x19,// loope 8026h
				/*000D*/ 0xB0, 0x03,// mov al,3
				/*000F*/ 0x67, 0xE1, 0x14,// loope 8026h
				/*0012*/ 0xB0, 0x04,// mov al,4
				/*0014*/ 0xE0, 0x10,// loopne 8026h
				/*0016*/ 0xB0, 0x05,// mov al,5
				/*0018*/ 0x67, 0xE0, 0x0B,// loopne 8026h
				/*001B*/ 0xB0, 0x06,// mov al,6
				/*001D*/ 0x67, 0xE3, 0x06,// jecxz 8026h
				/*0020*/ 0xB0, 0x07,// mov al,7
				/*0022*/ 0xE3, 0x02,// jcxz 8026h
				/*0024*/ 0xB0, 0x08,// mov al,8
				/*0026*/ 0x90,// nop
			};
			var newData = new byte[] {
				/*0000*/ 0xB0, 0x00,// mov al,0
				/*0002*/ 0xE2, 0x22,// loop 0F026h
				/*0004*/ 0xB0, 0x01,// mov al,1
				/*0006*/ 0x67, 0xE2, 0x1D,// loop 0F026h
				/*0009*/ 0xB0, 0x02,// mov al,2
				/*000B*/ 0xE1, 0x19,// loope 0F026h
				/*000D*/ 0xB0, 0x03,// mov al,3
				/*000F*/ 0x67, 0xE1, 0x14,// loope 0F026h
				/*0012*/ 0xB0, 0x04,// mov al,4
				/*0014*/ 0xE0, 0x10,// loopne 0F026h
				/*0016*/ 0xB0, 0x05,// mov al,5
				/*0018*/ 0x67, 0xE0, 0x0B,// loopne 0F026h
				/*001B*/ 0xB0, 0x06,// mov al,6
				/*001D*/ 0x67, 0xE3, 0x06,// jecxz 0F026h
				/*0020*/ 0xB0, 0x07,// mov al,7
				/*0022*/ 0xE3, 0x02,// jcxz 0F026h
				/*0024*/ 0xB0, 0x08,// mov al,8
				/*0026*/ 0x90,// nop
			};
			var expectedInstructionOffsets = new uint[] {
				0x0000,
				0x0002,
				0x0004,
				0x0006,
				0x0009,
				0x000B,
				0x000D,
				0x000F,
				0x0012,
				0x0014,
				0x0016,
				0x0018,
				0x001B,
				0x001D,
				0x0020,
				0x0022,
				0x0024,
				0x0026,
			};
			var expectedRelocInfos = Array.Empty<RelocInfo>();
			const BlockEncoderOptions options = BlockEncoderOptions.None;
			EncodeBase(bitness, origRip, originalData, newRip, newData, options, decoderOptions, expectedInstructionOffsets, expectedRelocInfos);
		}

		[Fact]
		void Br8_bwd() {
			var originalData = new byte[] {
				/*0000*/ 0x90,// nop
				/*0001*/ 0xB0, 0x00,// mov al,0
				/*0003*/ 0xE2, 0xFB,// loop 8000h
				/*0005*/ 0xB0, 0x01,// mov al,1
				/*0007*/ 0x67, 0xE2, 0xF6,// loop 8000h
				/*000A*/ 0xB0, 0x02,// mov al,2
				/*000C*/ 0xE1, 0xF2,// loope 8000h
				/*000E*/ 0xB0, 0x03,// mov al,3
				/*0010*/ 0x67, 0xE1, 0xED,// loope 8000h
				/*0013*/ 0xB0, 0x04,// mov al,4
				/*0015*/ 0xE0, 0xE9,// loopne 8000h
				/*0017*/ 0xB0, 0x05,// mov al,5
				/*0019*/ 0x67, 0xE0, 0xE4,// loopne 8000h
				/*001C*/ 0xB0, 0x06,// mov al,6
				/*001E*/ 0x67, 0xE3, 0xDF,// jecxz 8000h
				/*0021*/ 0xB0, 0x07,// mov al,7
				/*0023*/ 0xE3, 0xDB,// jcxz 8000h
				/*0025*/ 0xB0, 0x08,// mov al,8
			};
			var newData = new byte[] {
				/*0000*/ 0x90,// nop
				/*0001*/ 0xB0, 0x00,// mov al,0
				/*0003*/ 0xE2, 0xFB,// loop 0F000h
				/*0005*/ 0xB0, 0x01,// mov al,1
				/*0007*/ 0x67, 0xE2, 0xF6,// loop 0F000h
				/*000A*/ 0xB0, 0x02,// mov al,2
				/*000C*/ 0xE1, 0xF2,// loope 0F000h
				/*000E*/ 0xB0, 0x03,// mov al,3
				/*0010*/ 0x67, 0xE1, 0xED,// loope 0F000h
				/*0013*/ 0xB0, 0x04,// mov al,4
				/*0015*/ 0xE0, 0xE9,// loopne 0F000h
				/*0017*/ 0xB0, 0x05,// mov al,5
				/*0019*/ 0x67, 0xE0, 0xE4,// loopne 0F000h
				/*001C*/ 0xB0, 0x06,// mov al,6
				/*001E*/ 0x67, 0xE3, 0xDF,// jecxz 0F000h
				/*0021*/ 0xB0, 0x07,// mov al,7
				/*0023*/ 0xE3, 0xDB,// jcxz 0F000h
				/*0025*/ 0xB0, 0x08,// mov al,8
			};
			var expectedInstructionOffsets = new uint[] {
				0x0000,
				0x0001,
				0x0003,
				0x0005,
				0x0007,
				0x000A,
				0x000C,
				0x000E,
				0x0010,
				0x0013,
				0x0015,
				0x0017,
				0x0019,
				0x001C,
				0x001E,
				0x0021,
				0x0023,
				0x0025,
			};
			var expectedRelocInfos = Array.Empty<RelocInfo>();
			const BlockEncoderOptions options = BlockEncoderOptions.None;
			EncodeBase(bitness, origRip, originalData, newRip, newData, options, decoderOptions, expectedInstructionOffsets, expectedRelocInfos);
		}

		[Fact]
		void Br8_fwd_os() {
			var originalData = new byte[] {
				/*0000*/ 0xB0, 0x00,// mov al,0
				/*0002*/ 0x66, 0xE2, 0x29,// loopd 0000802Eh
				/*0005*/ 0xB0, 0x01,// mov al,1
				/*0007*/ 0x66, 0x67, 0xE2, 0x23,// loopd 0000802Eh
				/*000B*/ 0xB0, 0x02,// mov al,2
				/*000D*/ 0x66, 0xE1, 0x1E,// looped 0000802Eh
				/*0010*/ 0xB0, 0x03,// mov al,3
				/*0012*/ 0x66, 0x67, 0xE1, 0x18,// looped 0000802Eh
				/*0016*/ 0xB0, 0x04,// mov al,4
				/*0018*/ 0x66, 0xE0, 0x13,// loopned 0000802Eh
				/*001B*/ 0xB0, 0x05,// mov al,5
				/*001D*/ 0x66, 0x67, 0xE0, 0x0D,// loopned 0000802Eh
				/*0021*/ 0xB0, 0x06,// mov al,6
				/*0023*/ 0x66, 0x67, 0xE3, 0x07,// jecxz 0000802Eh
				/*0027*/ 0xB0, 0x07,// mov al,7
				/*0029*/ 0x66, 0xE3, 0x02,// jcxz 0000802Eh
				/*002C*/ 0xB0, 0x08,// mov al,8
				/*002E*/ 0x90,// nop
			};
			var newData = new byte[] {
				/*0000*/ 0xB0, 0x00,// mov al,0
				/*0002*/ 0x66, 0xE2, 0x29,// loopd 0000802Dh
				/*0005*/ 0xB0, 0x01,// mov al,1
				/*0007*/ 0x66, 0x67, 0xE2, 0x23,// loopd 0000802Dh
				/*000B*/ 0xB0, 0x02,// mov al,2
				/*000D*/ 0x66, 0xE1, 0x1E,// looped 0000802Dh
				/*0010*/ 0xB0, 0x03,// mov al,3
				/*0012*/ 0x66, 0x67, 0xE1, 0x18,// looped 0000802Dh
				/*0016*/ 0xB0, 0x04,// mov al,4
				/*0018*/ 0x66, 0xE0, 0x13,// loopned 0000802Dh
				/*001B*/ 0xB0, 0x05,// mov al,5
				/*001D*/ 0x66, 0x67, 0xE0, 0x0D,// loopned 0000802Dh
				/*0021*/ 0xB0, 0x06,// mov al,6
				/*0023*/ 0x66, 0x67, 0xE3, 0x07,// jecxz 0000802Dh
				/*0027*/ 0xB0, 0x07,// mov al,7
				/*0029*/ 0x66, 0xE3, 0x02,// jcxz 0000802Dh
				/*002C*/ 0xB0, 0x08,// mov al,8
				/*002E*/ 0x90,// nop
			};
			var expectedInstructionOffsets = new uint[] {
				0x0000,
				0x0002,
				0x0005,
				0x0007,
				0x000B,
				0x000D,
				0x0010,
				0x0012,
				0x0016,
				0x0018,
				0x001B,
				0x001D,
				0x0021,
				0x0023,
				0x0027,
				0x0029,
				0x002C,
				0x002E,
			};
			var expectedRelocInfos = Array.Empty<RelocInfo>();
			const BlockEncoderOptions options = BlockEncoderOptions.None;
			EncodeBase(bitness, origRip, originalData, origRip - 1, newData, options, decoderOptions, expectedInstructionOffsets, expectedRelocInfos);
		}

		[Fact]
		void Br8_short_other_short() {
			var originalData = new byte[] {
				/*0000*/ 0xB0, 0x00,// mov al,0
				/*0002*/ 0xE2, 0x22,// loop 8026h
				/*0004*/ 0xB0, 0x01,// mov al,1
				/*0006*/ 0x67, 0xE2, 0x1D,// loop 8026h
				/*0009*/ 0xB0, 0x02,// mov al,2
				/*000B*/ 0xE1, 0x19,// loope 8026h
				/*000D*/ 0xB0, 0x03,// mov al,3
				/*000F*/ 0x67, 0xE1, 0x14,// loope 8026h
				/*0012*/ 0xB0, 0x04,// mov al,4
				/*0014*/ 0xE0, 0x10,// loopne 8026h
				/*0016*/ 0xB0, 0x05,// mov al,5
				/*0018*/ 0x67, 0xE0, 0x0B,// loopne 8026h
				/*001B*/ 0xB0, 0x06,// mov al,6
				/*001D*/ 0x67, 0xE3, 0x06,// jecxz 8026h
				/*0020*/ 0xB0, 0x07,// mov al,7
				/*0022*/ 0xE3, 0x02,// jcxz 8026h
				/*0024*/ 0xB0, 0x08,// mov al,8
			};
			var newData = new byte[] {
				/*0000*/ 0xB0, 0x00,// mov al,0
				/*0002*/ 0xE2, 0x23,// loop 8026h
				/*0004*/ 0xB0, 0x01,// mov al,1
				/*0006*/ 0x67, 0xE2, 0x1E,// loop 8026h
				/*0009*/ 0xB0, 0x02,// mov al,2
				/*000B*/ 0xE1, 0x1A,// loope 8026h
				/*000D*/ 0xB0, 0x03,// mov al,3
				/*000F*/ 0x67, 0xE1, 0x15,// loope 8026h
				/*0012*/ 0xB0, 0x04,// mov al,4
				/*0014*/ 0xE0, 0x11,// loopne 8026h
				/*0016*/ 0xB0, 0x05,// mov al,5
				/*0018*/ 0x67, 0xE0, 0x0C,// loopne 8026h
				/*001B*/ 0xB0, 0x06,// mov al,6
				/*001D*/ 0x67, 0xE3, 0x07,// jecxz 8026h
				/*0020*/ 0xB0, 0x07,// mov al,7
				/*0022*/ 0xE3, 0x03,// jcxz 8026h
				/*0024*/ 0xB0, 0x08,// mov al,8
			};
			var expectedInstructionOffsets = new uint[] {
				0x0000,
				0x0002,
				0x0004,
				0x0006,
				0x0009,
				0x000B,
				0x000D,
				0x000F,
				0x0012,
				0x0014,
				0x0016,
				0x0018,
				0x001B,
				0x001D,
				0x0020,
				0x0022,
				0x0024,
			};
			var expectedRelocInfos = Array.Empty<RelocInfo>();
			const BlockEncoderOptions options = BlockEncoderOptions.None;
			EncodeBase(bitness, origRip, originalData, origRip - 1, newData, options, decoderOptions, expectedInstructionOffsets, expectedRelocInfos);
		}

		[Fact]
		void Br8_short_other_near() {
			var originalData = new byte[] {
				/*0000*/ 0xB0, 0x00,// mov al,0
				/*0002*/ 0xE2, 0x22,// loop 8026h
				/*0004*/ 0xB0, 0x01,// mov al,1
				/*0006*/ 0x67, 0xE2, 0x1E,// loop 8027h
				/*0009*/ 0xB0, 0x02,// mov al,2
				/*000B*/ 0xE1, 0x1B,// loope 8028h
				/*000D*/ 0xB0, 0x03,// mov al,3
				/*000F*/ 0x67, 0xE1, 0x17,// loope 8029h
				/*0012*/ 0xB0, 0x04,// mov al,4
				/*0014*/ 0xE0, 0x14,// loopne 802Ah
				/*0016*/ 0xB0, 0x05,// mov al,5
				/*0018*/ 0x67, 0xE0, 0x10,// loopne 802Bh
				/*001B*/ 0xB0, 0x06,// mov al,6
				/*001D*/ 0x67, 0xE3, 0x0C,// jecxz 802Ch
				/*0020*/ 0xB0, 0x07,// mov al,7
				/*0022*/ 0xE3, 0x09,// jcxz 802Dh
				/*0024*/ 0xB0, 0x08,// mov al,8
			};
			var newData = new byte[] {
				/*0000*/ 0xB0, 0x00,// mov al,0
				/*0002*/ 0xE2, 0x02,// loop 9006h
				/*0004*/ 0xEB, 0x03,// jmp short 9009h
				/*0006*/ 0xE9, 0x1D, 0xF0,// jmp near ptr 8026h
				/*0009*/ 0xB0, 0x01,// mov al,1
				/*000B*/ 0x67, 0xE2, 0x02,// loop 9010h
				/*000E*/ 0xEB, 0x03,// jmp short 9013h
				/*0010*/ 0xE9, 0x14, 0xF0,// jmp near ptr 8027h
				/*0013*/ 0xB0, 0x02,// mov al,2
				/*0015*/ 0xE1, 0x02,// loope 9019h
				/*0017*/ 0xEB, 0x03,// jmp short 901Ch
				/*0019*/ 0xE9, 0x0C, 0xF0,// jmp near ptr 8028h
				/*001C*/ 0xB0, 0x03,// mov al,3
				/*001E*/ 0x67, 0xE1, 0x02,// loope 9023h
				/*0021*/ 0xEB, 0x03,// jmp short 9026h
				/*0023*/ 0xE9, 0x03, 0xF0,// jmp near ptr 8029h
				/*0026*/ 0xB0, 0x04,// mov al,4
				/*0028*/ 0xE0, 0x02,// loopne 902Ch
				/*002A*/ 0xEB, 0x03,// jmp short 902Fh
				/*002C*/ 0xE9, 0xFB, 0xEF,// jmp near ptr 802Ah
				/*002F*/ 0xB0, 0x05,// mov al,5
				/*0031*/ 0x67, 0xE0, 0x02,// loopne 9036h
				/*0034*/ 0xEB, 0x03,// jmp short 9039h
				/*0036*/ 0xE9, 0xF2, 0xEF,// jmp near ptr 802Bh
				/*0039*/ 0xB0, 0x06,// mov al,6
				/*003B*/ 0x67, 0xE3, 0x02,// jecxz 9040h
				/*003E*/ 0xEB, 0x03,// jmp short 9043h
				/*0040*/ 0xE9, 0xE9, 0xEF,// jmp near ptr 802Ch
				/*0043*/ 0xB0, 0x07,// mov al,7
				/*0045*/ 0xE3, 0x02,// jcxz 9049h
				/*0047*/ 0xEB, 0x03,// jmp short 904Ch
				/*0049*/ 0xE9, 0xE1, 0xEF,// jmp near ptr 802Dh
				/*004C*/ 0xB0, 0x08,// mov al,8
			};
			var expectedInstructionOffsets = new uint[] {
				0x0000,
				uint.MaxValue,
				0x0009,
				uint.MaxValue,
				0x0013,
				uint.MaxValue,
				0x001C,
				uint.MaxValue,
				0x0026,
				uint.MaxValue,
				0x002F,
				uint.MaxValue,
				0x0039,
				uint.MaxValue,
				0x0043,
				uint.MaxValue,
				0x004C,
			};
			var expectedRelocInfos = Array.Empty<RelocInfo>();
			const BlockEncoderOptions options = BlockEncoderOptions.None;
			EncodeBase(bitness, origRip, originalData, origRip + 0x1000, newData, options, decoderOptions, expectedInstructionOffsets, expectedRelocInfos);
		}

		[Fact]
		void Br8_same_br() {
			var originalData = new byte[] {
				/*0000*/ 0xE2, 0xFE,// loop 8000h
				/*0002*/ 0xE2, 0xFC,// loop 8000h
				/*0004*/ 0xE2, 0xFA,// loop 8000h
			};
			var newData = new byte[] {
				/*0000*/ 0xE2, 0xFE,// loop 8000h
				/*0002*/ 0xE2, 0xFC,// loop 8000h
				/*0004*/ 0xE2, 0xFA,// loop 8000h
			};
			var expectedInstructionOffsets = new uint[] {
				0x0000,
				0x0002,
				0x0004,
			};
			var expectedRelocInfos = Array.Empty<RelocInfo>();
			const BlockEncoderOptions options = BlockEncoderOptions.None;
			EncodeBase(bitness, origRip, originalData, origRip, newData, options, decoderOptions, expectedInstructionOffsets, expectedRelocInfos);
		}
	}
}
#endif
