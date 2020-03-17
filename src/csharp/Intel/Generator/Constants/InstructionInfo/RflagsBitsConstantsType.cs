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

namespace Generator.Constants.InstructionInfo {
	static class RflagsBitsConstantsType {
		public static readonly ConstantsType Instance = new ConstantsType(TypeIds.RflagsBitsConstants, ConstantsTypeFlags.None, null, GetConstants());

		static Constant[] GetConstants() {
			return new Constant[] {
				new Constant(ConstantKind.Char, "AF", 'a'),
				new Constant(ConstantKind.Char, "CF", 'c'),
				new Constant(ConstantKind.Char, "OF", 'o'),
				new Constant(ConstantKind.Char, "PF", 'p'),
				new Constant(ConstantKind.Char, "SF", 's'),
				new Constant(ConstantKind.Char, "ZF", 'z'),
				new Constant(ConstantKind.Char, "IF", 'i'),
				new Constant(ConstantKind.Char, "DF", 'd'),
				new Constant(ConstantKind.Char, "AC", 'A'),
			};
		}
	}
}
