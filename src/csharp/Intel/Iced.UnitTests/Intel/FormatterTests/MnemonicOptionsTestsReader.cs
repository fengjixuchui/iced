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

#if GAS || INTEL || MASM || NASM
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Iced.Intel;

namespace Iced.UnitTests.Intel.FormatterTests {
	readonly struct MnemonicOptionsTestCase {
		public readonly string HexBytes;
		public readonly Code Code;
		public readonly int Bitness;
		public readonly string FormattedString;
		public readonly FormatMnemonicOptions Flags;

		public MnemonicOptionsTestCase(string hexBytes, Code code, int bitness, string formattedString, FormatMnemonicOptions flags) {
			HexBytes = hexBytes;
			Code = code;
			Bitness = bitness;
			FormattedString = formattedString;
			Flags = flags;
		}
	}

	static class MnemonicOptionsTestsReader {
		static class Dicts {
			// GENERATOR-BEGIN: Dicts
			// ⚠️This was generated by GENERATOR!🦹‍♂️
			internal static readonly Dictionary<string, FormatMnemonicOptions> ToFormatMnemonicOptions = new Dictionary<string, FormatMnemonicOptions>(2, StringComparer.Ordinal) {
				{ "noprefixes", FormatMnemonicOptions.NoPrefixes },
				{ "nomnemonic", FormatMnemonicOptions.NoMnemonic },
			};
			// GENERATOR-END: Dicts
		}

		public static IEnumerable<MnemonicOptionsTestCase> ReadFile(string filename) {
			Debug.Assert(File.Exists(filename));
			int lineNo = 0;
			foreach (var line in File.ReadLines(filename)) {
				lineNo++;
				if (line.Length == 0 || line[0] == '#')
					continue;

				MnemonicOptionsTestCase? tc;
				try {
					tc = ParseLine(line);
				}
				catch (Exception ex) {
					throw new Exception($"Invalid line {lineNo} ({filename}): {ex.Message}");
				}
				if (tc.HasValue)
					yield return tc.GetValueOrDefault();
			}

		}

		static readonly char[] commaSeparator = new char[] { ',' };
		static readonly char[] spaceSeparator = new char[] { ' ' };
		static MnemonicOptionsTestCase? ParseLine(string line) {
			var elems = line.Split(commaSeparator);
			if (elems.Length != 5)
				throw new Exception($"Invalid number of commas: {elems.Length - 1}");

			var hexBytes = elems[0].Trim();
			var codeStr = elems[1].Trim();
			if (CodeUtils.IsIgnored(codeStr))
				return null;
			var code = ToEnumConverter.GetCode(codeStr);
			var bitness = NumberConverter.ToInt32(elems[2].Trim());
			var formattedString = elems[3].Trim().Replace('|', ',');
			var flags = FormatMnemonicOptions.None;
			foreach (var value in elems[4].Split(spaceSeparator, StringSplitOptions.RemoveEmptyEntries)) {
				if (!Dicts.ToFormatMnemonicOptions.TryGetValue(value, out var f))
					throw new InvalidOperationException($"Invalid flags value: {value}");
				flags |= f;
			}
			return new MnemonicOptionsTestCase(hexBytes, code, bitness, formattedString, flags);
		}
	}
}
#endif
