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

namespace Generator.Documentation {
	abstract class DocCommentWriter {
		protected enum TokenKind {
			NewParagraph,
			String,
			Code,
			PrimitiveType,
			Type,
			EnumFieldReference,
			FieldReference,
			Property,
			Method,
		}

		protected static IEnumerable<(TokenKind kind, string value, string value2)> GetTokens(string defaultTypeName, string comment) {
			int index = 0;
			while (index < comment.Length) {
				const string pattern = "#(";
				const string patternEnd = ")#";
				const char newParagraph = 'p';
				const char codeChar = 'c';
				const char primitiveTypeChar = 't';
				const char typeChar = 'r';
				const char enumFieldReferenceChar = 'e';
				const char fieldReferenceChar = 'f';
				const char propertyChar = 'P';
				const char methodChar = 'M';
				// char (eg. 'c') + ':'
				const int kindLen = 1 + 1;

				int nextIndex = comment.IndexOf(pattern, index);
				if (nextIndex < 0)
					nextIndex = comment.Length;
				if (nextIndex != index) {
					yield return (TokenKind.String, comment.Substring(index, nextIndex - index), string.Empty);
					index = nextIndex;
				}
				if (index == comment.Length)
					break;
				index += pattern.Length;
				if (index + kindLen > comment.Length)
					throw new InvalidOperationException($"Invalid comment: {comment}");
				var type = comment[index];
				if (comment[index + 1] != ':')
					throw new InvalidOperationException($"Invalid comment: {comment}");
				nextIndex = comment.IndexOf(patternEnd, index + kindLen);
				if (nextIndex < 0)
					throw new InvalidOperationException($"Invalid comment: {comment}");

				string typeName, memberName;
				var data = comment.Substring(index + kindLen, nextIndex - (index + kindLen));
				switch (type) {
				case newParagraph:
					if (!string.IsNullOrEmpty(data))
						throw new InvalidOperationException($"Invalid comment: {comment}");
					yield return (TokenKind.NewParagraph, data, string.Empty);
					break;

				case codeChar:
					yield return (TokenKind.Code, data, string.Empty);
					break;

				case primitiveTypeChar:
					yield return (TokenKind.PrimitiveType, data, string.Empty);
					break;

				case typeChar:
					yield return (TokenKind.Type, data, string.Empty);
					break;

				case enumFieldReferenceChar:
					(typeName, memberName) = SplitMember(data, defaultTypeName);
					yield return (TokenKind.EnumFieldReference, typeName, memberName);
					break;

				case fieldReferenceChar:
					(typeName, memberName) = SplitMember(data, defaultTypeName);
					yield return (TokenKind.FieldReference, typeName, memberName);
					break;

				case propertyChar:
					(typeName, memberName) = SplitMember(data, defaultTypeName);
					yield return (TokenKind.Property, typeName, memberName);
					break;

				case methodChar:
					(typeName, memberName) = SplitMember(data, defaultTypeName);
					yield return (TokenKind.Method, typeName, memberName);
					break;

				default:
					throw new InvalidOperationException($"Invalid char '{type}', comment: {comment}");
				}

				index = nextIndex + patternEnd.Length;
			}
		}

		static (string type, string name) SplitMember(string s, string defaultTypeName) {
			string typeName, memberName;
			int typeIndex = s.IndexOf('.');
			if (typeIndex < 0) {
				typeName = defaultTypeName;
				memberName = s;
			}
			else {
				typeName = s.Substring(0, typeIndex);
				memberName = s.Substring(typeIndex + 1);
			}
			return (typeName, memberName);
		}

		protected static string RemoveNamespace(string type) {
			int i = type.LastIndexOf('.');
			if (i < 0)
				return type;
			return type.Substring(i + 1);
		}
	}
}
