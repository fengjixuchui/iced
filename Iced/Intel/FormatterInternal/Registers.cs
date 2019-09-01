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

#if (!NO_GAS_FORMATTER || !NO_INTEL_FORMATTER || !NO_MASM_FORMATTER || !NO_NASM_FORMATTER) && !NO_FORMATTER
using System;
using Iced.Intel.Internal;

namespace Iced.Intel.FormatterInternal {
	static class Registers {
		static byte[] GetRegistersData() =>
			new byte[] {
				0x03, 0x3F, 0x3F, 0x3F,// ???
				0x02, 0x61, 0x6C,// al
				0x02, 0x63, 0x6C,// cl
				0x02, 0x64, 0x6C,// dl
				0x02, 0x62, 0x6C,// bl
				0x02, 0x61, 0x68,// ah
				0x02, 0x63, 0x68,// ch
				0x02, 0x64, 0x68,// dh
				0x02, 0x62, 0x68,// bh
				0x03, 0x73, 0x70, 0x6C,// spl
				0x03, 0x62, 0x70, 0x6C,// bpl
				0x03, 0x73, 0x69, 0x6C,// sil
				0x03, 0x64, 0x69, 0x6C,// dil
				0x03, 0x72, 0x38, 0x62,// r8b
				0x03, 0x72, 0x39, 0x62,// r9b
				0x04, 0x72, 0x31, 0x30, 0x62,// r10b
				0x04, 0x72, 0x31, 0x31, 0x62,// r11b
				0x04, 0x72, 0x31, 0x32, 0x62,// r12b
				0x04, 0x72, 0x31, 0x33, 0x62,// r13b
				0x04, 0x72, 0x31, 0x34, 0x62,// r14b
				0x04, 0x72, 0x31, 0x35, 0x62,// r15b
				0x02, 0x61, 0x78,// ax
				0x02, 0x63, 0x78,// cx
				0x02, 0x64, 0x78,// dx
				0x02, 0x62, 0x78,// bx
				0x02, 0x73, 0x70,// sp
				0x02, 0x62, 0x70,// bp
				0x02, 0x73, 0x69,// si
				0x02, 0x64, 0x69,// di
				0x03, 0x72, 0x38, 0x77,// r8w
				0x03, 0x72, 0x39, 0x77,// r9w
				0x04, 0x72, 0x31, 0x30, 0x77,// r10w
				0x04, 0x72, 0x31, 0x31, 0x77,// r11w
				0x04, 0x72, 0x31, 0x32, 0x77,// r12w
				0x04, 0x72, 0x31, 0x33, 0x77,// r13w
				0x04, 0x72, 0x31, 0x34, 0x77,// r14w
				0x04, 0x72, 0x31, 0x35, 0x77,// r15w
				0x03, 0x65, 0x61, 0x78,// eax
				0x03, 0x65, 0x63, 0x78,// ecx
				0x03, 0x65, 0x64, 0x78,// edx
				0x03, 0x65, 0x62, 0x78,// ebx
				0x03, 0x65, 0x73, 0x70,// esp
				0x03, 0x65, 0x62, 0x70,// ebp
				0x03, 0x65, 0x73, 0x69,// esi
				0x03, 0x65, 0x64, 0x69,// edi
				0x03, 0x72, 0x38, 0x64,// r8d
				0x03, 0x72, 0x39, 0x64,// r9d
				0x04, 0x72, 0x31, 0x30, 0x64,// r10d
				0x04, 0x72, 0x31, 0x31, 0x64,// r11d
				0x04, 0x72, 0x31, 0x32, 0x64,// r12d
				0x04, 0x72, 0x31, 0x33, 0x64,// r13d
				0x04, 0x72, 0x31, 0x34, 0x64,// r14d
				0x04, 0x72, 0x31, 0x35, 0x64,// r15d
				0x03, 0x72, 0x61, 0x78,// rax
				0x03, 0x72, 0x63, 0x78,// rcx
				0x03, 0x72, 0x64, 0x78,// rdx
				0x03, 0x72, 0x62, 0x78,// rbx
				0x03, 0x72, 0x73, 0x70,// rsp
				0x03, 0x72, 0x62, 0x70,// rbp
				0x03, 0x72, 0x73, 0x69,// rsi
				0x03, 0x72, 0x64, 0x69,// rdi
				0x02, 0x72, 0x38,// r8
				0x02, 0x72, 0x39,// r9
				0x03, 0x72, 0x31, 0x30,// r10
				0x03, 0x72, 0x31, 0x31,// r11
				0x03, 0x72, 0x31, 0x32,// r12
				0x03, 0x72, 0x31, 0x33,// r13
				0x03, 0x72, 0x31, 0x34,// r14
				0x03, 0x72, 0x31, 0x35,// r15
				0x03, 0x65, 0x69, 0x70,// eip
				0x03, 0x72, 0x69, 0x70,// rip
				0x02, 0x65, 0x73,// es
				0x02, 0x63, 0x73,// cs
				0x02, 0x73, 0x73,// ss
				0x02, 0x64, 0x73,// ds
				0x02, 0x66, 0x73,// fs
				0x02, 0x67, 0x73,// gs
				0x04, 0x78, 0x6D, 0x6D, 0x30,// xmm0
				0x04, 0x78, 0x6D, 0x6D, 0x31,// xmm1
				0x04, 0x78, 0x6D, 0x6D, 0x32,// xmm2
				0x04, 0x78, 0x6D, 0x6D, 0x33,// xmm3
				0x04, 0x78, 0x6D, 0x6D, 0x34,// xmm4
				0x04, 0x78, 0x6D, 0x6D, 0x35,// xmm5
				0x04, 0x78, 0x6D, 0x6D, 0x36,// xmm6
				0x04, 0x78, 0x6D, 0x6D, 0x37,// xmm7
				0x04, 0x78, 0x6D, 0x6D, 0x38,// xmm8
				0x04, 0x78, 0x6D, 0x6D, 0x39,// xmm9
				0x05, 0x78, 0x6D, 0x6D, 0x31, 0x30,// xmm10
				0x05, 0x78, 0x6D, 0x6D, 0x31, 0x31,// xmm11
				0x05, 0x78, 0x6D, 0x6D, 0x31, 0x32,// xmm12
				0x05, 0x78, 0x6D, 0x6D, 0x31, 0x33,// xmm13
				0x05, 0x78, 0x6D, 0x6D, 0x31, 0x34,// xmm14
				0x05, 0x78, 0x6D, 0x6D, 0x31, 0x35,// xmm15
				0x05, 0x78, 0x6D, 0x6D, 0x31, 0x36,// xmm16
				0x05, 0x78, 0x6D, 0x6D, 0x31, 0x37,// xmm17
				0x05, 0x78, 0x6D, 0x6D, 0x31, 0x38,// xmm18
				0x05, 0x78, 0x6D, 0x6D, 0x31, 0x39,// xmm19
				0x05, 0x78, 0x6D, 0x6D, 0x32, 0x30,// xmm20
				0x05, 0x78, 0x6D, 0x6D, 0x32, 0x31,// xmm21
				0x05, 0x78, 0x6D, 0x6D, 0x32, 0x32,// xmm22
				0x05, 0x78, 0x6D, 0x6D, 0x32, 0x33,// xmm23
				0x05, 0x78, 0x6D, 0x6D, 0x32, 0x34,// xmm24
				0x05, 0x78, 0x6D, 0x6D, 0x32, 0x35,// xmm25
				0x05, 0x78, 0x6D, 0x6D, 0x32, 0x36,// xmm26
				0x05, 0x78, 0x6D, 0x6D, 0x32, 0x37,// xmm27
				0x05, 0x78, 0x6D, 0x6D, 0x32, 0x38,// xmm28
				0x05, 0x78, 0x6D, 0x6D, 0x32, 0x39,// xmm29
				0x05, 0x78, 0x6D, 0x6D, 0x33, 0x30,// xmm30
				0x05, 0x78, 0x6D, 0x6D, 0x33, 0x31,// xmm31
				0x04, 0x79, 0x6D, 0x6D, 0x30,// ymm0
				0x04, 0x79, 0x6D, 0x6D, 0x31,// ymm1
				0x04, 0x79, 0x6D, 0x6D, 0x32,// ymm2
				0x04, 0x79, 0x6D, 0x6D, 0x33,// ymm3
				0x04, 0x79, 0x6D, 0x6D, 0x34,// ymm4
				0x04, 0x79, 0x6D, 0x6D, 0x35,// ymm5
				0x04, 0x79, 0x6D, 0x6D, 0x36,// ymm6
				0x04, 0x79, 0x6D, 0x6D, 0x37,// ymm7
				0x04, 0x79, 0x6D, 0x6D, 0x38,// ymm8
				0x04, 0x79, 0x6D, 0x6D, 0x39,// ymm9
				0x05, 0x79, 0x6D, 0x6D, 0x31, 0x30,// ymm10
				0x05, 0x79, 0x6D, 0x6D, 0x31, 0x31,// ymm11
				0x05, 0x79, 0x6D, 0x6D, 0x31, 0x32,// ymm12
				0x05, 0x79, 0x6D, 0x6D, 0x31, 0x33,// ymm13
				0x05, 0x79, 0x6D, 0x6D, 0x31, 0x34,// ymm14
				0x05, 0x79, 0x6D, 0x6D, 0x31, 0x35,// ymm15
				0x05, 0x79, 0x6D, 0x6D, 0x31, 0x36,// ymm16
				0x05, 0x79, 0x6D, 0x6D, 0x31, 0x37,// ymm17
				0x05, 0x79, 0x6D, 0x6D, 0x31, 0x38,// ymm18
				0x05, 0x79, 0x6D, 0x6D, 0x31, 0x39,// ymm19
				0x05, 0x79, 0x6D, 0x6D, 0x32, 0x30,// ymm20
				0x05, 0x79, 0x6D, 0x6D, 0x32, 0x31,// ymm21
				0x05, 0x79, 0x6D, 0x6D, 0x32, 0x32,// ymm22
				0x05, 0x79, 0x6D, 0x6D, 0x32, 0x33,// ymm23
				0x05, 0x79, 0x6D, 0x6D, 0x32, 0x34,// ymm24
				0x05, 0x79, 0x6D, 0x6D, 0x32, 0x35,// ymm25
				0x05, 0x79, 0x6D, 0x6D, 0x32, 0x36,// ymm26
				0x05, 0x79, 0x6D, 0x6D, 0x32, 0x37,// ymm27
				0x05, 0x79, 0x6D, 0x6D, 0x32, 0x38,// ymm28
				0x05, 0x79, 0x6D, 0x6D, 0x32, 0x39,// ymm29
				0x05, 0x79, 0x6D, 0x6D, 0x33, 0x30,// ymm30
				0x05, 0x79, 0x6D, 0x6D, 0x33, 0x31,// ymm31
				0x04, 0x7A, 0x6D, 0x6D, 0x30,// zmm0
				0x04, 0x7A, 0x6D, 0x6D, 0x31,// zmm1
				0x04, 0x7A, 0x6D, 0x6D, 0x32,// zmm2
				0x04, 0x7A, 0x6D, 0x6D, 0x33,// zmm3
				0x04, 0x7A, 0x6D, 0x6D, 0x34,// zmm4
				0x04, 0x7A, 0x6D, 0x6D, 0x35,// zmm5
				0x04, 0x7A, 0x6D, 0x6D, 0x36,// zmm6
				0x04, 0x7A, 0x6D, 0x6D, 0x37,// zmm7
				0x04, 0x7A, 0x6D, 0x6D, 0x38,// zmm8
				0x04, 0x7A, 0x6D, 0x6D, 0x39,// zmm9
				0x05, 0x7A, 0x6D, 0x6D, 0x31, 0x30,// zmm10
				0x05, 0x7A, 0x6D, 0x6D, 0x31, 0x31,// zmm11
				0x05, 0x7A, 0x6D, 0x6D, 0x31, 0x32,// zmm12
				0x05, 0x7A, 0x6D, 0x6D, 0x31, 0x33,// zmm13
				0x05, 0x7A, 0x6D, 0x6D, 0x31, 0x34,// zmm14
				0x05, 0x7A, 0x6D, 0x6D, 0x31, 0x35,// zmm15
				0x05, 0x7A, 0x6D, 0x6D, 0x31, 0x36,// zmm16
				0x05, 0x7A, 0x6D, 0x6D, 0x31, 0x37,// zmm17
				0x05, 0x7A, 0x6D, 0x6D, 0x31, 0x38,// zmm18
				0x05, 0x7A, 0x6D, 0x6D, 0x31, 0x39,// zmm19
				0x05, 0x7A, 0x6D, 0x6D, 0x32, 0x30,// zmm20
				0x05, 0x7A, 0x6D, 0x6D, 0x32, 0x31,// zmm21
				0x05, 0x7A, 0x6D, 0x6D, 0x32, 0x32,// zmm22
				0x05, 0x7A, 0x6D, 0x6D, 0x32, 0x33,// zmm23
				0x05, 0x7A, 0x6D, 0x6D, 0x32, 0x34,// zmm24
				0x05, 0x7A, 0x6D, 0x6D, 0x32, 0x35,// zmm25
				0x05, 0x7A, 0x6D, 0x6D, 0x32, 0x36,// zmm26
				0x05, 0x7A, 0x6D, 0x6D, 0x32, 0x37,// zmm27
				0x05, 0x7A, 0x6D, 0x6D, 0x32, 0x38,// zmm28
				0x05, 0x7A, 0x6D, 0x6D, 0x32, 0x39,// zmm29
				0x05, 0x7A, 0x6D, 0x6D, 0x33, 0x30,// zmm30
				0x05, 0x7A, 0x6D, 0x6D, 0x33, 0x31,// zmm31
				0x02, 0x6B, 0x30,// k0
				0x02, 0x6B, 0x31,// k1
				0x02, 0x6B, 0x32,// k2
				0x02, 0x6B, 0x33,// k3
				0x02, 0x6B, 0x34,// k4
				0x02, 0x6B, 0x35,// k5
				0x02, 0x6B, 0x36,// k6
				0x02, 0x6B, 0x37,// k7
				0x04, 0x62, 0x6E, 0x64, 0x30,// bnd0
				0x04, 0x62, 0x6E, 0x64, 0x31,// bnd1
				0x04, 0x62, 0x6E, 0x64, 0x32,// bnd2
				0x04, 0x62, 0x6E, 0x64, 0x33,// bnd3
				0x03, 0x63, 0x72, 0x30,// cr0
				0x03, 0x63, 0x72, 0x31,// cr1
				0x03, 0x63, 0x72, 0x32,// cr2
				0x03, 0x63, 0x72, 0x33,// cr3
				0x03, 0x63, 0x72, 0x34,// cr4
				0x03, 0x63, 0x72, 0x35,// cr5
				0x03, 0x63, 0x72, 0x36,// cr6
				0x03, 0x63, 0x72, 0x37,// cr7
				0x03, 0x63, 0x72, 0x38,// cr8
				0x03, 0x63, 0x72, 0x39,// cr9
				0x04, 0x63, 0x72, 0x31, 0x30,// cr10
				0x04, 0x63, 0x72, 0x31, 0x31,// cr11
				0x04, 0x63, 0x72, 0x31, 0x32,// cr12
				0x04, 0x63, 0x72, 0x31, 0x33,// cr13
				0x04, 0x63, 0x72, 0x31, 0x34,// cr14
				0x04, 0x63, 0x72, 0x31, 0x35,// cr15
				0x03, 0x64, 0x72, 0x30,// dr0
				0x03, 0x64, 0x72, 0x31,// dr1
				0x03, 0x64, 0x72, 0x32,// dr2
				0x03, 0x64, 0x72, 0x33,// dr3
				0x03, 0x64, 0x72, 0x34,// dr4
				0x03, 0x64, 0x72, 0x35,// dr5
				0x03, 0x64, 0x72, 0x36,// dr6
				0x03, 0x64, 0x72, 0x37,// dr7
				0x03, 0x64, 0x72, 0x38,// dr8
				0x03, 0x64, 0x72, 0x39,// dr9
				0x04, 0x64, 0x72, 0x31, 0x30,// dr10
				0x04, 0x64, 0x72, 0x31, 0x31,// dr11
				0x04, 0x64, 0x72, 0x31, 0x32,// dr12
				0x04, 0x64, 0x72, 0x31, 0x33,// dr13
				0x04, 0x64, 0x72, 0x31, 0x34,// dr14
				0x04, 0x64, 0x72, 0x31, 0x35,// dr15
				0x05, 0x73, 0x74, 0x28, 0x30, 0x29,// st(0)
				0x05, 0x73, 0x74, 0x28, 0x31, 0x29,// st(1)
				0x05, 0x73, 0x74, 0x28, 0x32, 0x29,// st(2)
				0x05, 0x73, 0x74, 0x28, 0x33, 0x29,// st(3)
				0x05, 0x73, 0x74, 0x28, 0x34, 0x29,// st(4)
				0x05, 0x73, 0x74, 0x28, 0x35, 0x29,// st(5)
				0x05, 0x73, 0x74, 0x28, 0x36, 0x29,// st(6)
				0x05, 0x73, 0x74, 0x28, 0x37, 0x29,// st(7)
				0x03, 0x6D, 0x6D, 0x30,// mm0
				0x03, 0x6D, 0x6D, 0x31,// mm1
				0x03, 0x6D, 0x6D, 0x32,// mm2
				0x03, 0x6D, 0x6D, 0x33,// mm3
				0x03, 0x6D, 0x6D, 0x34,// mm4
				0x03, 0x6D, 0x6D, 0x35,// mm5
				0x03, 0x6D, 0x6D, 0x36,// mm6
				0x03, 0x6D, 0x6D, 0x37,// mm7
				0x03, 0x74, 0x72, 0x30,// tr0
				0x03, 0x74, 0x72, 0x31,// tr1
				0x03, 0x74, 0x72, 0x32,// tr2
				0x03, 0x74, 0x72, 0x33,// tr3
				0x03, 0x74, 0x72, 0x34,// tr4
				0x03, 0x74, 0x72, 0x35,// tr5
				0x03, 0x74, 0x72, 0x36,// tr6
				0x03, 0x74, 0x72, 0x37,// tr7
				0x02, 0x73, 0x74,// st
			};

		public static string[] GetRegisters() {
			const int MaxStringLength = 5;
			const int StringsCount = DecoderConstants.NumberOfRegisters + 1;
			var reader = new DataReader(GetRegistersData(), MaxStringLength);
			var strings = new string[StringsCount];
			for (int i = 0; i < strings.Length; i++)
				strings[i] = reader.ReadAsciiString();
			if (reader.CanRead)
				throw new InvalidOperationException();
			return strings;
		}
	}
}
#endif
