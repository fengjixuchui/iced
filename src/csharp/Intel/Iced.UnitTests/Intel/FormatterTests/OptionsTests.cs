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
using System.Linq;
using Iced.Intel;
using Xunit;

namespace Iced.UnitTests.Intel.FormatterTests {
	// GENERATOR-BEGIN: OptionsProps
	// ⚠️This was generated by GENERATOR!🦹‍♂️
	enum OptionsProps {
		AddLeadingZeroToHexNumbers,
		AlwaysShowScale,
		AlwaysShowSegmentRegister,
		BinaryDigitGroupSize,
		BinaryPrefix,
		BinarySuffix,
		BranchLeadingZeroes,
		DecimalDigitGroupSize,
		DecimalPrefix,
		DecimalSuffix,
		DigitSeparator,
		DisplacementLeadingZeroes,
		FirstOperandCharIndex,
		GasNakedRegisters,
		GasShowMnemonicSizeSuffix,
		GasSpaceAfterMemoryOperandComma,
		HexDigitGroupSize,
		HexPrefix,
		HexSuffix,
		IP,
		LeadingZeroes,
		MasmAddDsPrefix32,
		MemorySizeOptions,
		NasmShowSignExtendedImmediateSize,
		NumberBase,
		OctalDigitGroupSize,
		OctalPrefix,
		OctalSuffix,
		PreferST0,
		RipRelativeAddresses,
		ScaleBeforeIndex,
		ShowBranchSize,
		ShowSymbolAddress,
		ShowZeroDisplacements,
		SignedImmediateOperands,
		SignedMemoryDisplacements,
		SmallHexNumbersInDecimal,
		SpaceAfterMemoryBracket,
		SpaceAfterOperandSeparator,
		SpaceBetweenMemoryAddOperators,
		SpaceBetweenMemoryMulOperators,
		TabSize,
		UppercaseAll,
		UppercaseDecorators,
		UppercaseHex,
		UppercaseKeywords,
		UppercaseMnemonics,
		UppercasePrefixes,
		UppercaseRegisters,
		UsePseudoOps,
		CC_b,
		CC_ae,
		CC_e,
		CC_ne,
		CC_be,
		CC_a,
		CC_p,
		CC_np,
		CC_l,
		CC_ge,
		CC_le,
		CC_g,
		DecoderOptions,
	}
	// GENERATOR-END: OptionsProps

	static class OptionsPropsUtils {
		public static DecoderOptions GetDecoderOptions(IList<(OptionsProps property, object value)> props) {
			var options = DecoderOptions.None;
			foreach (var (prop, value) in props) {
				if (prop == OptionsProps.DecoderOptions)
					options |= (DecoderOptions)value;
			}
			return options;
		}

		public static void Initialize(FormatterOptions options, OptionsProps property, object value) {
			switch (property) {
			case OptionsProps.AddLeadingZeroToHexNumbers: options.AddLeadingZeroToHexNumbers = (bool)value; break;
			case OptionsProps.AlwaysShowScale: options.AlwaysShowScale = (bool)value; break;
			case OptionsProps.AlwaysShowSegmentRegister: options.AlwaysShowSegmentRegister = (bool)value; break;
			case OptionsProps.BinaryDigitGroupSize: options.BinaryDigitGroupSize = (int)value; break;
			case OptionsProps.BinaryPrefix: options.BinaryPrefix = (string)value; break;
			case OptionsProps.BinarySuffix: options.BinarySuffix = (string)value; break;
			case OptionsProps.BranchLeadingZeroes: options.BranchLeadingZeroes = (bool)value; break;
			case OptionsProps.DecimalDigitGroupSize: options.DecimalDigitGroupSize = (int)value; break;
			case OptionsProps.DecimalPrefix: options.DecimalPrefix = (string)value; break;
			case OptionsProps.DecimalSuffix: options.DecimalSuffix = (string)value; break;
			case OptionsProps.DigitSeparator: options.DigitSeparator = (string)value; break;
			case OptionsProps.DisplacementLeadingZeroes: options.DisplacementLeadingZeroes = (bool)value; break;
			case OptionsProps.FirstOperandCharIndex: options.FirstOperandCharIndex = (int)value; break;
			case OptionsProps.GasNakedRegisters: options.GasNakedRegisters = (bool)value; break;
			case OptionsProps.GasShowMnemonicSizeSuffix: options.GasShowMnemonicSizeSuffix = (bool)value; break;
			case OptionsProps.GasSpaceAfterMemoryOperandComma: options.GasSpaceAfterMemoryOperandComma = (bool)value; break;
			case OptionsProps.HexDigitGroupSize: options.HexDigitGroupSize = (int)value; break;
			case OptionsProps.HexPrefix: options.HexPrefix = (string)value; break;
			case OptionsProps.HexSuffix: options.HexSuffix = (string)value; break;
			case OptionsProps.LeadingZeroes: options.LeadingZeroes = (bool)value; break;
			case OptionsProps.MasmAddDsPrefix32: options.MasmAddDsPrefix32 = (bool)value; break;
			case OptionsProps.MemorySizeOptions: options.MemorySizeOptions = (MemorySizeOptions)value; break;
			case OptionsProps.NasmShowSignExtendedImmediateSize: options.NasmShowSignExtendedImmediateSize = (bool)value; break;
			case OptionsProps.NumberBase: options.NumberBase = (NumberBase)value; break;
			case OptionsProps.OctalDigitGroupSize: options.OctalDigitGroupSize = (int)value; break;
			case OptionsProps.OctalPrefix: options.OctalPrefix = (string)value; break;
			case OptionsProps.OctalSuffix: options.OctalSuffix = (string)value; break;
			case OptionsProps.PreferST0: options.PreferST0 = (bool)value; break;
			case OptionsProps.RipRelativeAddresses: options.RipRelativeAddresses = (bool)value; break;
			case OptionsProps.ScaleBeforeIndex: options.ScaleBeforeIndex = (bool)value; break;
			case OptionsProps.ShowBranchSize: options.ShowBranchSize = (bool)value; break;
			case OptionsProps.ShowSymbolAddress: options.ShowSymbolAddress = (bool)value; break;
			case OptionsProps.ShowZeroDisplacements: options.ShowZeroDisplacements = (bool)value; break;
			case OptionsProps.SignedImmediateOperands: options.SignedImmediateOperands = (bool)value; break;
			case OptionsProps.SignedMemoryDisplacements: options.SignedMemoryDisplacements = (bool)value; break;
			case OptionsProps.SmallHexNumbersInDecimal: options.SmallHexNumbersInDecimal = (bool)value; break;
			case OptionsProps.SpaceAfterMemoryBracket: options.SpaceAfterMemoryBracket = (bool)value; break;
			case OptionsProps.SpaceAfterOperandSeparator: options.SpaceAfterOperandSeparator = (bool)value; break;
			case OptionsProps.SpaceBetweenMemoryAddOperators: options.SpaceBetweenMemoryAddOperators = (bool)value; break;
			case OptionsProps.SpaceBetweenMemoryMulOperators: options.SpaceBetweenMemoryMulOperators = (bool)value; break;
			case OptionsProps.TabSize: options.TabSize = (int)value; break;
			case OptionsProps.UppercaseAll: options.UppercaseAll = (bool)value; break;
			case OptionsProps.UppercaseDecorators: options.UppercaseDecorators = (bool)value; break;
			case OptionsProps.UppercaseHex: options.UppercaseHex = (bool)value; break;
			case OptionsProps.UppercaseKeywords: options.UppercaseKeywords = (bool)value; break;
			case OptionsProps.UppercaseMnemonics: options.UppercaseMnemonics = (bool)value; break;
			case OptionsProps.UppercasePrefixes: options.UppercasePrefixes = (bool)value; break;
			case OptionsProps.UppercaseRegisters: options.UppercaseRegisters = (bool)value; break;
			case OptionsProps.UsePseudoOps: options.UsePseudoOps = (bool)value; break;
			case OptionsProps.CC_b: options.CC_b = (CC_b)value; break;
			case OptionsProps.CC_ae: options.CC_ae = (CC_ae)value; break;
			case OptionsProps.CC_e: options.CC_e = (CC_e)value; break;
			case OptionsProps.CC_ne: options.CC_ne = (CC_ne)value; break;
			case OptionsProps.CC_be: options.CC_be = (CC_be)value; break;
			case OptionsProps.CC_a: options.CC_a = (CC_a)value; break;
			case OptionsProps.CC_p: options.CC_p = (CC_p)value; break;
			case OptionsProps.CC_np: options.CC_np = (CC_np)value; break;
			case OptionsProps.CC_l: options.CC_l = (CC_l)value; break;
			case OptionsProps.CC_ge: options.CC_ge = (CC_ge)value; break;
			case OptionsProps.CC_le: options.CC_le = (CC_le)value; break;
			case OptionsProps.CC_g: options.CC_g = (CC_g)value; break;
			case OptionsProps.IP:
			case OptionsProps.DecoderOptions:
				break;
			default: throw new InvalidOperationException();
			}
		}

		public static void Initialize(Decoder decoder, OptionsProps property, object value) {
			switch (property) {
			case OptionsProps.IP:
				decoder.IP = (ulong)value;
				break;
			}
		}

		public static void Initialize(FormatterOptions options, IEnumerable<(OptionsProps property, object value)> properties) {
			foreach (var info in properties)
				Initialize(options, info.property, info.value);
		}

		public static void Initialize(Decoder decoder, IEnumerable<(OptionsProps property, object value)> properties) {
			foreach (var info in properties)
				Initialize(decoder, info.property, info.value);
		}
	}

	public readonly struct OptionsInstructionInfo {
		public readonly int Bitness;
		public readonly string HexBytes;
		public readonly Code Code;
		public readonly DecoderOptions DecoderOptions;
		readonly List<(OptionsProps property, object value)> properties;
		internal OptionsInstructionInfo(int bitness, string hexBytes, Code code, List<(OptionsProps property, object value)> properties) {
			Bitness = bitness;
			HexBytes = hexBytes;
			Code = code;
			this.properties = properties;
			DecoderOptions = OptionsPropsUtils.GetDecoderOptions(properties);
		}

		internal void Initialize(FormatterOptions options) => OptionsPropsUtils.Initialize(options, properties);
		internal void Initialize(Decoder decoder) => OptionsPropsUtils.Initialize(decoder, properties);
	}

	public abstract class OptionsTests {
		protected static IEnumerable<object[]> GetFormatData(string formatterDir, string formattedStringsFile, string optionsFile = null) {
			OptionsInstructionInfo[] infos;
			HashSet<int> ignored;
			if (optionsFile is null)
				(infos, ignored) = FormatterOptionsTests.AllInfos;
			else {
				var infosFilename = FileUtils.GetFormatterFilename(Path.Combine(formatterDir, optionsFile));
				ignored = new HashSet<int>();
				infos = OptionsTestsReader.ReadFile(infosFilename, ignored).ToArray();
			}
			var formattedStrings = FileUtils.ReadRawStrings(Path.Combine(formatterDir, formattedStringsFile)).ToArray();
			formattedStrings = Utils.Filter(formattedStrings, ignored);
			if (infos.Length != formattedStrings.Length)
				throw new ArgumentException($"(infos.Length) {infos.Length} != (formattedStrings.Length) {formattedStrings.Length} . infos[0].HexBytes = {(infos.Length == 0 ? "<EMPTY>" : infos[0].HexBytes)} & formattedStrings[0] = {(formattedStrings.Length == 0 ? "<EMPTY>" : formattedStrings[0])}");
			var res = new object[infos.Length][];
			for (int i = 0; i < infos.Length; i++)
				res[i] = new object[3] { i, infos[i], formattedStrings[i] };
			return res;
		}

		protected void FormatBase(int index, OptionsInstructionInfo info, string formattedString, Formatter formatter) {
			info.Initialize(formatter.Options);
			FormatterTestUtils.SimpleFormatTest(info.Bitness, info.HexBytes, info.Code, info.DecoderOptions, formattedString, formatter, decoder => info.Initialize(decoder));
		}

		protected void TestOptionsBase(FormatterOptions options) {
			{
				int min = int.MaxValue, max = int.MinValue;
				foreach (var value in ToEnumConverter.GetNumberBaseValues()) {
					min = Math.Min(min, (int)value);
					max = Math.Max(max, (int)value);
					options.NumberBase = value;
				}
				Assert.Throws<ArgumentOutOfRangeException>(() => options.NumberBase = (NumberBase)(min - 1));
				Assert.Throws<ArgumentOutOfRangeException>(() => options.NumberBase = (NumberBase)(max + 1));
				Assert.Throws<ArgumentOutOfRangeException>(() => options.NumberBase = (NumberBase)int.MinValue);
				Assert.Throws<ArgumentOutOfRangeException>(() => options.NumberBase = (NumberBase)int.MaxValue);
			}

			{
				int min = int.MaxValue, max = int.MinValue;
				foreach (var value in ToEnumConverter.GetMemorySizeOptionsValues()) {
					min = Math.Min(min, (int)value);
					max = Math.Max(max, (int)value);
					options.MemorySizeOptions = value;
				}
				Assert.Throws<ArgumentOutOfRangeException>(() => options.MemorySizeOptions = (MemorySizeOptions)(min - 1));
				Assert.Throws<ArgumentOutOfRangeException>(() => options.MemorySizeOptions = (MemorySizeOptions)(max + 1));
				Assert.Throws<ArgumentOutOfRangeException>(() => options.MemorySizeOptions = (MemorySizeOptions)int.MinValue);
				Assert.Throws<ArgumentOutOfRangeException>(() => options.MemorySizeOptions = (MemorySizeOptions)int.MaxValue);
			}
		}
	}
}
#endif
