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
using Generator.IO;

namespace Generator.Tables.Rust {
	[Generator(TargetLanguage.Rust)]
	sealed class RustTupleTypeTableGenerator {
		readonly IdentifierConverter idConverter;
		readonly GeneratorContext generatorContext;

		public RustTupleTypeTableGenerator(GeneratorContext generatorContext) {
			idConverter = RustIdentifierConverter.Create();
			this.generatorContext = generatorContext;
		}

		public void Generate() {
			var infos = generatorContext.Types.GetObject<TupleTypeTable>(TypeIds.TupleTypeTable).Data;
			var filename = generatorContext.Types.Dirs.GetRustFilename("tuple_type_tbl.rs");
			var updater = new FileUpdater(TargetLanguage.Rust, "TupleTypeTable", filename);
			updater.Generate(writer => WriteTable(writer, infos));
		}

		void WriteTable(FileWriter writer, TupleTypeInfo[] infos) {
			var tupleTypeName = generatorContext.Types[TypeIds.TupleType].Name(idConverter);
			writer.WriteLine(RustConstants.AttributeNoRustFmt);
			writer.WriteLine($"static TUPLE_TYPE_TBL: [u8; {infos.Length * 2}] = [");
			using (writer.Indent()) {
				foreach (var info in infos) {
					writer.WriteCommentLine($"{tupleTypeName}.{info.Value.Name(idConverter)}");
					if (info.N > byte.MaxValue)
						throw new InvalidOperationException();
					if (info.Nbcst > byte.MaxValue)
						throw new InvalidOperationException();
					writer.Write($"0x{info.N:X2},");
					writer.WriteCommentLine("N");
					writer.Write($"0x{info.Nbcst:X2},");
					writer.WriteCommentLine("Nbcst");
				}
			}
			writer.WriteLine("];");
		}
	}
}
