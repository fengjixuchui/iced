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

namespace Generator.Constants {
	abstract class ConstantsGenerator {
		public abstract void Generate(ConstantsType constantsType);

		protected readonly GenTypes genTypes;

		protected ConstantsGenerator(GenTypes genTypes) => this.genTypes = genTypes;

		public void Generate() {
			var allConstants = new ConstantsType[] {
				genTypes.GetConstantsType(TypeIds.IcedConstants),
				genTypes.GetConstantsType(TypeIds.DecoderTestParserConstants),
				genTypes.GetConstantsType(TypeIds.DecoderConstants),
				genTypes.GetConstantsType(TypeIds.InstructionInfoKeys),
				genTypes.GetConstantsType(TypeIds.MiscInstrInfoTestConstants),
				genTypes.GetConstantsType(TypeIds.RflagsBitsConstants),
				genTypes.GetConstantsType(TypeIds.MiscSectionNames),
				genTypes.GetConstantsType(TypeIds.OpCodeInfoKeys),
				genTypes.GetConstantsType(TypeIds.OpCodeInfoFlags),
			};

			foreach (var constantsType in allConstants)
				Generate(constantsType);
		}
	}
}
