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

namespace Generator.Enums {
	abstract class EnumsGenerator {
		protected readonly GenTypes genTypes;

		protected EnumsGenerator(GenTypes genTypes) =>
			this.genTypes = genTypes;

		public abstract void Generate(EnumType enumType);

		public void Generate() {
			var allEnums = new EnumType[] {
				genTypes[TypeIds.Code],
				genTypes[TypeIds.CodeSize],
				genTypes[TypeIds.ConditionCode],
				genTypes[TypeIds.CpuidFeature],
				genTypes[TypeIds.DecoderError],
				genTypes[TypeIds.DecoderOptions],
				genTypes[TypeIds.DecoderTestOptions],
				genTypes[TypeIds.EvexOpCodeHandlerKind],
				genTypes[TypeIds.HandlerFlags],
				genTypes[TypeIds.LegacyHandlerFlags],
				genTypes[TypeIds.MemorySize],
				genTypes[TypeIds.OpCodeHandlerKind],
				genTypes[TypeIds.PseudoOpsKind],
				genTypes[TypeIds.Register],
				genTypes[TypeIds.SerializedDataKind],
				genTypes[TypeIds.TupleType],
				genTypes[TypeIds.VexOpCodeHandlerKind],
				genTypes[TypeIds.Mnemonic],
				genTypes[TypeIds.FormatterFlowControl],
				genTypes[TypeIds.GasCtorKind],
				genTypes[TypeIds.GasSizeOverride],
				genTypes[TypeIds.GasInstrOpInfoFlags],
				genTypes[TypeIds.GasInstrOpKind],
				genTypes[TypeIds.IntelCtorKind],
				genTypes[TypeIds.IntelSizeOverride],
				genTypes[TypeIds.IntelBranchSizeInfo],
				genTypes[TypeIds.IntelInstrOpInfoFlags],
				genTypes[TypeIds.IntelInstrOpKind],
				genTypes[TypeIds.MasmCtorKind],
				genTypes[TypeIds.MasmInstrOpInfoFlags],
				genTypes[TypeIds.MasmInstrOpKind],
				genTypes[TypeIds.MasmSymbolTestFlags],
				genTypes[TypeIds.NasmCtorKind],
				genTypes[TypeIds.NasmSignExtendInfo],
				genTypes[TypeIds.NasmSizeOverride],
				genTypes[TypeIds.NasmBranchSizeInfo],
				genTypes[TypeIds.NasmInstrOpInfoFlags],
				genTypes[TypeIds.NasmMemorySizeInfo],
				genTypes[TypeIds.NasmFarMemorySizeInfo],
				genTypes[TypeIds.NasmInstrOpKind],
				genTypes[TypeIds.FastFmtFlags],
				genTypes[TypeIds.MemorySizeOptions],
				genTypes[TypeIds.NumberBase],
				genTypes[TypeIds.FormatMnemonicOptions],
				genTypes[TypeIds.PrefixKind],
				genTypes[TypeIds.DecoratorKind],
				genTypes[TypeIds.NumberKind],
				genTypes[TypeIds.FormatterTextKind],
				genTypes[TypeIds.SymbolFlags],
				genTypes[TypeIds.OptionsProps],
				genTypes[TypeIds.CC_b],
				genTypes[TypeIds.CC_ae],
				genTypes[TypeIds.CC_e],
				genTypes[TypeIds.CC_ne],
				genTypes[TypeIds.CC_be],
				genTypes[TypeIds.CC_a],
				genTypes[TypeIds.CC_p],
				genTypes[TypeIds.CC_np],
				genTypes[TypeIds.CC_l],
				genTypes[TypeIds.CC_ge],
				genTypes[TypeIds.CC_le],
				genTypes[TypeIds.CC_g],
				genTypes[TypeIds.RoundingControl],
				genTypes[TypeIds.OpKind],
				genTypes[TypeIds.Instruction_CodeFlags],
				genTypes[TypeIds.Instruction_MemoryFlags],
				genTypes[TypeIds.Instruction_OpKindFlags],
				genTypes[TypeIds.VectorLength],
				genTypes[TypeIds.MandatoryPrefixByte],
				genTypes[TypeIds.StateFlags],
				genTypes[TypeIds.OpSize],
				genTypes[TypeIds.EncodingKind],
				genTypes[TypeIds.FlowControl],
				genTypes[TypeIds.OpCodeOperandKind],
				genTypes[TypeIds.RflagsBits],
				genTypes[TypeIds.OpAccess],
				genTypes[TypeIds.MemorySizeFlags],
				genTypes[TypeIds.RegisterFlags],
				genTypes[TypeIds.LegacyOpCodeTable],
				genTypes[TypeIds.VexOpCodeTable],
				genTypes[TypeIds.XopOpCodeTable],
				genTypes[TypeIds.EvexOpCodeTable],
				genTypes[TypeIds.MandatoryPrefix],
				genTypes[TypeIds.OpCodeTableKind],
				genTypes[TypeIds.DisplSize],
				genTypes[TypeIds.ImmSize],
				genTypes[TypeIds.EncoderFlags],
				genTypes[TypeIds.WBit],
				genTypes[TypeIds.LBit],
				genTypes[TypeIds.LKind],
				genTypes[TypeIds.RepPrefixKind],
				genTypes[TypeIds.RelocKind],
				genTypes[TypeIds.BlockEncoderOptions],
				genTypes[TypeIds.EncFlags2],
				genTypes[TypeIds.EncFlags3],
				genTypes[TypeIds.OpCodeInfoFlags1],
				genTypes[TypeIds.OpCodeInfoFlags2],
				genTypes[TypeIds.DecOptionValue],
				genTypes[TypeIds.InstrStrFmtOption],
			};

			foreach (var enumType in allEnums)
				Generate(enumType);
		}
	}
}
