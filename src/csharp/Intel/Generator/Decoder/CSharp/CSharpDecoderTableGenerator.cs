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

using Generator.IO;

namespace Generator.Decoder.CSharp {
	[Generator(TargetLanguage.CSharp)]
	sealed class CSharpDecoderTableGenerator {
		readonly GenTypes genTypes;

		public CSharpDecoderTableGenerator(GeneratorContext generatorContext) => genTypes = generatorContext.Types;

		public void Generate() {
			var serializers = new CSharpDecoderTableSerializer[] {
				new CSharpDecoderTableSerializer(genTypes, "OpCodeHandlersTables_Legacy", DecoderTableSerializerInfo.Legacy(genTypes)),
				new CSharpDecoderTableSerializer(genTypes, "OpCodeHandlersTables_VEX", DecoderTableSerializerInfo.Vex(genTypes)),
				new CSharpDecoderTableSerializer(genTypes, "OpCodeHandlersTables_EVEX", DecoderTableSerializerInfo.Evex(genTypes)),
				new CSharpDecoderTableSerializer(genTypes, "OpCodeHandlersTables_XOP", DecoderTableSerializerInfo.Xop(genTypes)),
			};

			foreach (var serializer in serializers) {
				var filename = CSharpConstants.GetFilename(genTypes, CSharpConstants.DecoderNamespace, serializer.ClassName + ".g.cs");
				using (var writer = new FileWriter(TargetLanguage.CSharp, FileUtils.OpenWrite(filename)))
					serializer.Serialize(writer);
			}
		}
	}
}
