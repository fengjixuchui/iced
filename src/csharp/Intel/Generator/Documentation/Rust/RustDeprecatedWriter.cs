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

using Generator.Constants;
using Generator.Enums;
using Generator.IO;

namespace Generator.Documentation.Rust {
	sealed class RustDeprecatedWriter : DeprecatedWriter {
		readonly IdentifierConverter idConverter;

		public RustDeprecatedWriter(IdentifierConverter idConverter) =>
			this.idConverter = idConverter;

		public override void WriteDeprecated(FileWriter writer, EnumValue value) {
			if (value.DeprecatedInfo.IsDeprecated) {
				var newValue = value.DeclaringType[value.DeprecatedInfo.NewName];
				WriteDeprecated(writer, value.DeprecatedInfo.Version, newValue.Name(idConverter));
			}
		}

		public override void WriteDeprecated(FileWriter writer, Constant value) {
			if (value.DeprecatedInfo.IsDeprecated) {
				var newValue = value.DeclaringType[value.DeprecatedInfo.NewName];
				WriteDeprecated(writer, value.DeprecatedInfo.Version, newValue.Name(idConverter));
			}
		}

		static void WriteDeprecated(FileWriter writer, string version, string newName) =>
			writer.WriteLine($"#[deprecated(since = \"{version}\", note = \"Use {newName} instead\")]");
	}
}
