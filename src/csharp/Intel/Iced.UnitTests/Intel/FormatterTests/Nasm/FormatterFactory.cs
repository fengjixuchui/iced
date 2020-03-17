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

#if NASM
using Iced.Intel;

namespace Iced.UnitTests.Intel.FormatterTests.Nasm {
	static class FormatterFactory {
		static FormatterOptions CreateOptions() {
			var options = FormatterOptions.CreateNasm();
			options.UppercaseHex = false;
			options.HexPrefix = "0x";
			options.HexSuffix = null;
			options.OctalPrefix = "0o";
			options.OctalSuffix = null;
			options.BinaryPrefix = "0b";
			options.BinarySuffix = null;
			return options;
		}

		public static NasmFormatter Create_MemDefault() {
			var options = CreateOptions();
			options.MemorySizeOptions = MemorySizeOptions.Default;
			options.NasmShowSignExtendedImmediateSize = true;
			options.ShowBranchSize = false;
			options.RipRelativeAddresses = true;
			options.SignedImmediateOperands = false;
			options.SpaceAfterOperandSeparator = false;
			return new NasmFormatter(options);
		}

		public static NasmFormatter Create_MemAlways() {
			var options = CreateOptions();
			options.MemorySizeOptions = MemorySizeOptions.Always;
			options.NasmShowSignExtendedImmediateSize = true;
			options.ShowBranchSize = true;
			options.RipRelativeAddresses = false;
			options.SignedImmediateOperands = true;
			options.SpaceAfterOperandSeparator = true;
			return new NasmFormatter(options);
		}

		public static NasmFormatter Create_MemMinimum() {
			var options = CreateOptions();
			options.MemorySizeOptions = MemorySizeOptions.Minimum;
			options.NasmShowSignExtendedImmediateSize = true;
			options.ShowBranchSize = true;
			options.RipRelativeAddresses = false;
			options.SignedImmediateOperands = true;
			options.SpaceAfterOperandSeparator = true;
			return new NasmFormatter(options);
		}

		public static NasmFormatter Create() {
			var options = CreateOptions();
			options.MemorySizeOptions = MemorySizeOptions.Default;
			options.NasmShowSignExtendedImmediateSize = true;
			options.ShowBranchSize = false;
			options.RipRelativeAddresses = true;
			return new NasmFormatter(options);
		}

		public static NasmFormatter Create_Options() {
			var options = CreateOptions();
			options.MemorySizeOptions = MemorySizeOptions.Default;
			options.NasmShowSignExtendedImmediateSize = false;
			options.ShowBranchSize = false;
			options.RipRelativeAddresses = true;
			return new NasmFormatter(options);
		}

		public static NasmFormatter Create_Registers() {
			var options = CreateOptions();
			return new NasmFormatter(options);
		}

		public static NasmFormatter Create_Numbers() {
			var options = CreateOptions();
			options.UppercaseHex = true;
			options.HexPrefix = null;
			options.HexSuffix = null;
			options.OctalPrefix = null;
			options.OctalSuffix = null;
			options.BinaryPrefix = null;
			options.BinarySuffix = null;
			return new NasmFormatter(options);
		}

		public static (Formatter formatter, ISymbolResolver symbolResolver) Create_Resolver(ISymbolResolver symbolResolver) {
			var options = CreateOptions();
			options.MemorySizeOptions = MemorySizeOptions.Default;
			options.NasmShowSignExtendedImmediateSize = false;
			options.ShowBranchSize = false;
			options.RipRelativeAddresses = true;
			return (new NasmFormatter(options, symbolResolver), symbolResolver);
		}
	}
}
#endif
