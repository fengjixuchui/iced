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
using System.IO;
using Iced.Intel;

namespace Iced.UnitTests.Intel.FormatterTests {
	static class OptionsTestsReader {
		public static IEnumerable<OptionsInstructionInfo> ReadFile(string filename) {
			int lineNo = 0;
			foreach (var line in File.ReadLines(filename)) {
				lineNo++;
				if (line.Length == 0 || line[0] == '#')
					continue;
				OptionsInstructionInfo testCase;
				try {
					testCase = ReadTestCase(line, lineNo);
				}
				catch (Exception ex) {
					throw new InvalidOperationException($"Error parsing options test case file '{filename}', line {lineNo}: {ex.Message}");
				}
				yield return testCase;
			}
		}

		static readonly char[] seps = new char[] { ',' };
		static readonly char[] optsseps = new char[] { ' ' };
		static OptionsInstructionInfo ReadTestCase(string line, int lineNo) {
			var parts = line.Split(seps);
			if (parts.Length != 4)
				throw new InvalidOperationException($"Invalid number of commas ({parts.Length - 1} commas)");

			int bitness = NumberConverter.ToInt32(parts[0].Trim());
			var hexBytes = parts[1].Trim();
			HexUtils.ToByteArray(hexBytes);
			var code = ToCode(parts[2].Trim());

			var properties = new List<(OptionsProps property, object value)>();
			foreach (var part in parts[3].Split(optsseps, StringSplitOptions.RemoveEmptyEntries)) {
				var (prop, value) = OptionsParser.ParseOption(part);
				properties.Add((prop, value));
			}

			return new OptionsInstructionInfo(bitness, hexBytes, code, properties);
		}

		static Code ToCode(string value) {
			if (!ToEnumConverter.TryCode(value, out var code))
				throw new InvalidOperationException($"Invalid Code value: '{value}'");
			return code;
		}
	}
}
#endif
