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

using System;
using System.IO;
using Generator.Constants;
using Generator.Enums;
using Generator.IO;

namespace Generator.Decoder.CSharp {
	[Generator(TargetLanguage.CSharp, GeneratorNames.Code_MemorySize)]
	sealed class CSharpInstructionMemorySizesGenerator {
		readonly IdentifierConverter idConverter;
		readonly GeneratorOptions generatorOptions;

		public CSharpInstructionMemorySizesGenerator(GeneratorOptions generatorOptions) {
			idConverter = CSharpIdentifierConverter.Create();
			this.generatorOptions = generatorOptions;
		}

		public void Generate() {
			var data = InstructionMemorySizesTable.Table;
			const string ClassName = "InstructionMemorySizes";
			var memSizeName = MemorySizeEnum.Instance.Name(idConverter);
			using (var writer = new FileWriter(TargetLanguage.CSharp, FileUtils.OpenWrite(Path.Combine(CSharpConstants.GetDirectory(generatorOptions, CSharpConstants.IcedNamespace), ClassName + ".g.cs")))) {
				writer.WriteFileHeader();

				writer.WriteLine($"namespace {CSharpConstants.IcedNamespace} {{");
				using (writer.Indent()) {
					writer.WriteLine($"static class {ClassName} {{");
					using (writer.Indent()) {
						writer.WriteCommentLine("0 = memory size");
						writer.WriteCommentLine("1 = broadcast memory size");
						writer.WriteLineNoIndent($"#if {CSharpConstants.HasSpanDefine}");
						writer.WriteLine($"internal static System.ReadOnlySpan<byte> Sizes => new byte[{IcedConstantsType.Instance.Name(idConverter)}.{IcedConstantsType.Instance[IcedConstants.NumberOfCodeValuesName].Name(idConverter)} * 2] {{");
						writer.WriteLineNoIndent("#else");
						writer.WriteLine($"internal static readonly byte[] Sizes = new byte[{IcedConstantsType.Instance.Name(idConverter)}.{IcedConstantsType.Instance[IcedConstants.NumberOfCodeValuesName].Name(idConverter)} * 2] {{");
						writer.WriteLineNoIndent("#endif");
						using (writer.Indent()) {
							foreach (var d in data) {
								if (d.mem.Value > byte.MaxValue)
									throw new InvalidOperationException();
								string value;
								if (d.mem.Value == 0)
									value = "0";
								else
									value = $"(byte){memSizeName}.{d.mem.Name(idConverter)}";
								writer.WriteLine($"{value},// {d.codeEnum.Name(idConverter)}");
							}
							foreach (var d in data) {
								if (d.bcst.Value > byte.MaxValue)
									throw new InvalidOperationException();
								string value;
								if (d.bcst.Value == 0)
									value = "0";
								else
									value = $"(byte){memSizeName}.{d.bcst.Name(idConverter)}";
								writer.WriteLine($"{value},// {d.codeEnum.Name(idConverter)}");
							}
						}
						writer.WriteLine("};");
					}
					writer.WriteLine("}");
				}
				writer.WriteLine("}");
			}
		}
	}
}
