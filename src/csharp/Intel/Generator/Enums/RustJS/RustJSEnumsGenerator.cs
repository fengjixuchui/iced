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
using System.Collections.Generic;
using System.IO;
using Generator.Documentation.Rust;
using Generator.IO;

namespace Generator.Enums.RustJS {
	[Generator(TargetLanguage.RustJS, GeneratorNames.Enums)]
	sealed class RustJSEnumsGenerator : EnumsGenerator {
		readonly IdentifierConverter idConverter;
		readonly Dictionary<TypeId, PartialEnumFileInfo?> toPartialFileInfo;
		readonly RustDocCommentWriter docWriter;

		sealed class PartialEnumFileInfo {
			public readonly string Id;
			public readonly string Filename;
			public readonly string[] Attributes;

			public PartialEnumFileInfo(string id, string filename, string? attribute = null) {
				Id = id;
				Filename = filename;
				Attributes = attribute is null ? Array.Empty<string>() : new string[] { attribute };
			}

			public PartialEnumFileInfo(string id, string filename, string[] attributes) {
				Id = id;
				Filename = filename;
				Attributes = attributes;
			}
		}

		public RustJSEnumsGenerator(GeneratorOptions generatorOptions) {
			idConverter = RustJSIdentifierConverter.Create();
			docWriter = new RustDocCommentWriter(idConverter, ".");

			toPartialFileInfo = new Dictionary<TypeId, PartialEnumFileInfo?>();
			toPartialFileInfo.Add(TypeIds.BlockEncoderOptions, new PartialEnumFileInfo("Enum", Path.Combine(generatorOptions.RustJSDir, "block_encoder_options.rs"), new[] { RustConstants.AttributeCopyClone, RustConstants.AttributeAllowNonCamelCaseTypes }));
			toPartialFileInfo.Add(TypeIds.CC_a, new PartialEnumFileInfo("CC_a", Path.Combine(generatorOptions.RustJSDir, "cc.rs"), new[] { RustConstants.AttributeCopyClone, RustConstants.AttributeAllowNonCamelCaseTypes }));
			toPartialFileInfo.Add(TypeIds.CC_ae, new PartialEnumFileInfo("CC_ae", Path.Combine(generatorOptions.RustJSDir, "cc.rs"), new[] { RustConstants.AttributeCopyClone, RustConstants.AttributeAllowNonCamelCaseTypes }));
			toPartialFileInfo.Add(TypeIds.CC_b, new PartialEnumFileInfo("CC_b", Path.Combine(generatorOptions.RustJSDir, "cc.rs"), new[] { RustConstants.AttributeCopyClone, RustConstants.AttributeAllowNonCamelCaseTypes }));
			toPartialFileInfo.Add(TypeIds.CC_be, new PartialEnumFileInfo("CC_be", Path.Combine(generatorOptions.RustJSDir, "cc.rs"), new[] { RustConstants.AttributeCopyClone, RustConstants.AttributeAllowNonCamelCaseTypes }));
			toPartialFileInfo.Add(TypeIds.CC_e, new PartialEnumFileInfo("CC_e", Path.Combine(generatorOptions.RustJSDir, "cc.rs"), new[] { RustConstants.AttributeCopyClone, RustConstants.AttributeAllowNonCamelCaseTypes }));
			toPartialFileInfo.Add(TypeIds.CC_g, new PartialEnumFileInfo("CC_g", Path.Combine(generatorOptions.RustJSDir, "cc.rs"), new[] { RustConstants.AttributeCopyClone, RustConstants.AttributeAllowNonCamelCaseTypes }));
			toPartialFileInfo.Add(TypeIds.CC_ge, new PartialEnumFileInfo("CC_ge", Path.Combine(generatorOptions.RustJSDir, "cc.rs"), new[] { RustConstants.AttributeCopyClone, RustConstants.AttributeAllowNonCamelCaseTypes }));
			toPartialFileInfo.Add(TypeIds.CC_l, new PartialEnumFileInfo("CC_l", Path.Combine(generatorOptions.RustJSDir, "cc.rs"), new[] { RustConstants.AttributeCopyClone, RustConstants.AttributeAllowNonCamelCaseTypes }));
			toPartialFileInfo.Add(TypeIds.CC_le, new PartialEnumFileInfo("CC_le", Path.Combine(generatorOptions.RustJSDir, "cc.rs"), new[] { RustConstants.AttributeCopyClone, RustConstants.AttributeAllowNonCamelCaseTypes }));
			toPartialFileInfo.Add(TypeIds.CC_ne, new PartialEnumFileInfo("CC_ne", Path.Combine(generatorOptions.RustJSDir, "cc.rs"), new[] { RustConstants.AttributeCopyClone, RustConstants.AttributeAllowNonCamelCaseTypes }));
			toPartialFileInfo.Add(TypeIds.CC_np, new PartialEnumFileInfo("CC_np", Path.Combine(generatorOptions.RustJSDir, "cc.rs"), new[] { RustConstants.AttributeCopyClone, RustConstants.AttributeAllowNonCamelCaseTypes }));
			toPartialFileInfo.Add(TypeIds.CC_p, new PartialEnumFileInfo("CC_p", Path.Combine(generatorOptions.RustJSDir, "cc.rs"), new[] { RustConstants.AttributeCopyClone, RustConstants.AttributeAllowNonCamelCaseTypes }));
			toPartialFileInfo.Add(TypeIds.Code, new PartialEnumFileInfo("Enum", Path.Combine(generatorOptions.RustJSDir, "code.rs"), new[] { RustConstants.AttributeCopyClone, RustConstants.AttributeAllowNonCamelCaseTypes }));
			toPartialFileInfo.Add(TypeIds.CodeSize, new PartialEnumFileInfo("Enum", Path.Combine(generatorOptions.RustJSDir, "code_size.rs"), RustConstants.AttributeCopyClone));
			toPartialFileInfo.Add(TypeIds.ConditionCode, new PartialEnumFileInfo("Enum", Path.Combine(generatorOptions.RustJSDir, "condition_code.rs"), new[] { RustConstants.AttributeCopyClone, RustConstants.AttributeAllowNonCamelCaseTypes }));
			toPartialFileInfo.Add(TypeIds.CpuidFeature, new PartialEnumFileInfo("Enum", Path.Combine(generatorOptions.RustJSDir, "cpuid_feature.rs"), new[] { RustConstants.AttributeCopyClone, RustConstants.AttributeAllowNonCamelCaseTypes }));
			toPartialFileInfo.Add(TypeIds.DecoderOptions, new PartialEnumFileInfo("Enum", Path.Combine(generatorOptions.RustJSDir, "decoder_options.rs"), new[] { RustConstants.AttributeCopyClone, RustConstants.AttributeAllowNonCamelCaseTypes }));
			toPartialFileInfo.Add(TypeIds.EncodingKind, new PartialEnumFileInfo("Enum", Path.Combine(generatorOptions.RustJSDir, "encoding_kind.rs"), new[] { RustConstants.AttributeCopyClone }));
			toPartialFileInfo.Add(TypeIds.FlowControl, new PartialEnumFileInfo("Enum", Path.Combine(generatorOptions.RustJSDir, "flow_control.rs"), new[] { RustConstants.AttributeCopyClone }));
			toPartialFileInfo.Add(TypeIds.FormatMnemonicOptions, new PartialEnumFileInfo("Enum", Path.Combine(generatorOptions.RustJSDir, "format_mnemonic_options.rs"), RustConstants.AttributeCopyClone));
			toPartialFileInfo.Add(TypeIds.MandatoryPrefix, new PartialEnumFileInfo("Enum", Path.Combine(generatorOptions.RustJSDir, "mandatory_prefix.rs"), new[] { RustConstants.AttributeCopyClone }));
			toPartialFileInfo.Add(TypeIds.MemorySize, new PartialEnumFileInfo("Enum", Path.Combine(generatorOptions.RustJSDir, "memory_size.rs"), new[] { RustConstants.AttributeCopyClone, RustConstants.AttributeAllowNonCamelCaseTypes }));
			toPartialFileInfo.Add(TypeIds.MemorySizeOptions, new PartialEnumFileInfo("Enum", Path.Combine(generatorOptions.RustJSDir, "memory_size_options.rs"), RustConstants.AttributeCopyClone));
			toPartialFileInfo.Add(TypeIds.Mnemonic, new PartialEnumFileInfo("Enum", Path.Combine(generatorOptions.RustJSDir, "mnemonic.rs"), new[] { RustConstants.AttributeCopyClone }));
			toPartialFileInfo.Add(TypeIds.OpAccess, new PartialEnumFileInfo("Enum", Path.Combine(generatorOptions.RustJSDir, "op_access.rs"), new[] { RustConstants.AttributeCopyClone }));
			toPartialFileInfo.Add(TypeIds.OpCodeOperandKind, new PartialEnumFileInfo("Enum", Path.Combine(generatorOptions.RustJSDir, "op_code_operand_kind.rs"), new[] { RustConstants.AttributeCopyClone, RustConstants.AttributeAllowNonCamelCaseTypes }));
			toPartialFileInfo.Add(TypeIds.OpCodeTableKind, new PartialEnumFileInfo("Enum", Path.Combine(generatorOptions.RustJSDir, "op_code_table_kind.rs"), new[] { RustConstants.AttributeCopyClone }));
			toPartialFileInfo.Add(TypeIds.OpKind, new PartialEnumFileInfo("Enum", Path.Combine(generatorOptions.RustJSDir, "op_kind.rs"), new[] { RustConstants.AttributeCopyClone, RustConstants.AttributeAllowNonCamelCaseTypes }));
			toPartialFileInfo.Add(TypeIds.Register, new PartialEnumFileInfo("Enum", Path.Combine(generatorOptions.RustJSDir, "register.rs"), new[] { RustConstants.AttributeCopyClone }));
			toPartialFileInfo.Add(TypeIds.RepPrefixKind, new PartialEnumFileInfo("Enum", Path.Combine(generatorOptions.RustJSDir, "rep_prefix_kind.rs"), new[] { RustConstants.AttributeCopyClone }));
			toPartialFileInfo.Add(TypeIds.RflagsBits, new PartialEnumFileInfo("Enum", Path.Combine(generatorOptions.RustJSDir, "rflags_bits.rs"), RustConstants.AttributeCopyClone));
			toPartialFileInfo.Add(TypeIds.RoundingControl, new PartialEnumFileInfo("Enum", Path.Combine(generatorOptions.RustJSDir, "rounding_control.rs"), RustConstants.AttributeCopyClone));
			toPartialFileInfo.Add(TypeIds.TupleType, new PartialEnumFileInfo("Enum", Path.Combine(generatorOptions.RustJSDir, "tuple_type.rs"), new[] { RustConstants.AttributeCopyClone, RustConstants.AttributeAllowNonCamelCaseTypes }));
		}

		public override void Generate(EnumType enumType) {
			if (toPartialFileInfo.TryGetValue(enumType.TypeId, out var partialInfo)) {
				if (!(partialInfo is null))
					new FileUpdater(TargetLanguage.RustJS, partialInfo.Id, partialInfo.Filename).Generate(writer => WriteEnum(writer, partialInfo, enumType));
			}
		}

		void WriteEnum(FileWriter writer, PartialEnumFileInfo info, EnumType enumType) =>
			WriteEnumCore(writer, info, enumType);

		void WriteEnumCore(FileWriter writer, PartialEnumFileInfo info, EnumType enumType) {
			// Don't add the Code comments since they're generated (less useful comments) and will generate bigger ts/js files
			bool writeComments = enumType.TypeId != TypeIds.Code;
			docWriter.WriteSummary(writer, enumType.Documentation, enumType.RawName);
			var enumTypeName = enumType.Name(idConverter);
			if (enumType.IsPublic)
				writer.WriteLine(RustConstants.AttributeWasmBindgen);
			foreach (var attr in info.Attributes)
				writer.WriteLine(attr);
			if (!writeComments || (enumType.IsPublic && enumType.IsMissingDocs))
				writer.WriteLine(RustConstants.AttributeAllowMissingDocs);
			if (!enumType.IsPublic)
				writer.WriteLine(RustConstants.AttributeAllowDeadCode);
			var pub = enumType.IsPublic ? "pub " : "pub(crate) ";
			writer.WriteLine($"{pub}enum {enumTypeName} {{");
			using (writer.Indent()) {
				uint expectedValue = 0;
				foreach (var value in enumType.Values) {
					if (writeComments)
						docWriter.WriteSummary(writer, value.Documentation, enumType.RawName);
					if (enumType.IsFlags)
						writer.WriteLine($"{value.Name(idConverter)} = {NumberFormatter.FormatHexUInt32WithSep(value.Value)},");
					else if (expectedValue != value.Value || enumType.IsPublic)
						writer.WriteLine($"{value.Name(idConverter)} = {value.Value},");
					else
						writer.WriteLine($"{value.Name(idConverter)},");
					expectedValue = value.Value + 1;
				}
			}
			writer.WriteLine("}");
		}
	}
}
