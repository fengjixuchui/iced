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

#if (!NO_GAS_FORMATTER || !NO_INTEL_FORMATTER || !NO_MASM_FORMATTER || !NO_NASM_FORMATTER) && !NO_FORMATTER
using System;

namespace Iced.Intel {
	/// <summary>
	/// Used by a <see cref="Formatter"/> to resolve symbols
	/// </summary>
	public interface ISymbolResolver {
		/// <summary>
		/// Tries to resolve a symbol. It returns true if <paramref name="symbol"/> was updated.
		/// </summary>
		/// <param name="instruction">Instruction</param>
		/// <param name="operand">Operand number, 0-based. This is a formatter operand and isn't necessarily the same as an instruction operand.</param>
		/// <param name="instructionOperand">Instruction operand number, 0-based, or -1 if it's an operand created by the formatter.</param>
		/// <param name="address">Address</param>
		/// <param name="addressSize">Size of <paramref name="address"/> in bytes</param>
		/// <param name="symbol">Updated with symbol information if this method returns true</param>
		/// <returns></returns>
		bool TryGetSymbol(in Instruction instruction, int operand, int instructionOperand, ulong address, int addressSize, out SymbolResult symbol);
	}

	/// <summary>
	/// Symbol flags
	/// </summary>
	[Flags]
	public enum SymbolFlags : uint {
		/// <summary>
		/// No bit is set
		/// </summary>
		None				= 0,

		/// <summary>
		/// It's a symbol relative to a register, eg. a struct offset `[ebx+some_struct.field1]`. If this is
		/// cleared, it's the address of a symbol.
		/// </summary>
		Relative			= 0x00000001,

		/// <summary>
		/// It's a signed symbol and it should be displayed as '-symbol' or 'reg-symbol' instead of 'symbol' or 'reg+symbol'
		/// </summary>
		Signed				= 0x00000002,

		/// <summary>
		/// Set if <see cref="SymbolResult.SymbolSize"/> is valid
		/// </summary>
		HasSymbolSize		= 0x00000004,
	}

	/// <summary>
	/// The result of resolving a symbol
	/// </summary>
	public readonly struct SymbolResult {
		/// <summary>
		/// The address of the symbol
		/// </summary>
		public readonly ulong Address;

		/// <summary>
		/// Contains the symbol
		/// </summary>
		public readonly TextInfo Text;

		/// <summary>
		/// Symbol flags
		/// </summary>
		public readonly SymbolFlags Flags;

		/// <summary>
		/// Checks whether <see cref="SymbolSize"/> is valid
		/// </summary>
		public bool HasSymbolSize => (Flags & SymbolFlags.HasSymbolSize) != 0;

		/// <summary>
		/// Symbol size if <see cref="HasSymbolSize"/> is true
		/// </summary>
		public readonly MemorySize SymbolSize;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="address">The address of the symbol</param>
		/// <param name="text">Symbol</param>
		public SymbolResult(ulong address, string text) {
			Address = address;
			Text = new TextInfo(text, FormatterOutputTextKind.Label);
			Flags = 0;
			SymbolSize = 0;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="address">The address of the symbol</param>
		/// <param name="text">Symbol</param>
		/// <param name="size">Symbol size</param>
		public SymbolResult(ulong address, string text, MemorySize size) {
			Address = address;
			Text = new TextInfo(text, FormatterOutputTextKind.Label);
			Flags = SymbolFlags.HasSymbolSize;
			SymbolSize = size;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="address">The address of the symbol</param>
		/// <param name="text">Symbol</param>
		/// <param name="color">Color</param>
		public SymbolResult(ulong address, string text, FormatterOutputTextKind color) {
			Address = address;
			Text = new TextInfo(text, color);
			Flags = 0;
			SymbolSize = 0;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="address">The address of the symbol</param>
		/// <param name="text">Symbol</param>
		/// <param name="color">Color</param>
		/// <param name="flags">Symbol flags</param>
		public SymbolResult(ulong address, string text, FormatterOutputTextKind color, SymbolFlags flags) {
			Address = address;
			Text = new TextInfo(text, color);
			Flags = flags & ~SymbolFlags.HasSymbolSize;
			SymbolSize = 0;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="address">The address of the symbol</param>
		/// <param name="text">Symbol</param>
		public SymbolResult(ulong address, TextInfo text) {
			Address = address;
			Text = text;
			Flags = 0;
			SymbolSize = 0;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="address">The address of the symbol</param>
		/// <param name="text">Symbol</param>
		/// <param name="size">Symbol size</param>
		public SymbolResult(ulong address, TextInfo text, MemorySize size) {
			Address = address;
			Text = text;
			Flags = SymbolFlags.HasSymbolSize;
			SymbolSize = size;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="address">The address of the symbol</param>
		/// <param name="text">Symbol</param>
		/// <param name="flags">Symbol flags</param>
		public SymbolResult(ulong address, TextInfo text, SymbolFlags flags) {
			Address = address;
			Text = text;
			Flags = flags & ~SymbolFlags.HasSymbolSize;
			SymbolSize = 0;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="address">The address of the symbol</param>
		/// <param name="text">Symbol</param>
		/// <param name="flags">Symbol flags</param>
		/// <param name="size">Symbol size</param>
		public SymbolResult(ulong address, TextInfo text, SymbolFlags flags, MemorySize size) {
			Address = address;
			Text = text;
			Flags = flags | SymbolFlags.HasSymbolSize;
			SymbolSize = size;
		}
	}

	/// <summary>
	/// Contains one or more <see cref="TextPart"/>s (text and color)
	/// </summary>
	public readonly struct TextInfo {
		/// <summary>
		/// true if this is the default instance
		/// </summary>
		public bool IsDefault => TextArray is null && Text.Text is null;

		/// <summary>
		/// The text and color unless <see cref="TextArray"/> is non-null
		/// </summary>
		public readonly TextPart Text;

		/// <summary>
		/// Text and color or null if <see cref="Text"/> should be used
		/// </summary>
		public readonly TextPart[]? TextArray;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="text">Text</param>
		/// <param name="color">Color</param>
		public TextInfo(string text, FormatterOutputTextKind color) {
			Text = new TextPart(text, color);
			TextArray = null;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="text">Text</param>
		public TextInfo(TextPart text) {
			Text = text;
			TextArray = null;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="text">All text parts</param>
		public TextInfo(TextPart[] text) {
			Text = default;
			TextArray = text;
		}
	}

	/// <summary>
	/// Contains text and colors
	/// </summary>
	public readonly struct TextPart {
		/// <summary>
		/// Text
		/// </summary>
		public readonly string Text;

		/// <summary>
		/// Color
		/// </summary>
		public readonly FormatterOutputTextKind Color;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="text">Text</param>
		/// <param name="color">Color</param>
		public TextPart(string text, FormatterOutputTextKind color) {
			Text = text;
			Color = color;
		}
	}
}
#endif
