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
using System.Linq;
using Generator.Enums;

namespace Generator.Tables {
	[TypeGen(TypeGenOrders.PreCreateInstructions)]
	sealed class InstructionDefs : ICreatedInstructions {
		public InstructionDef[] Defs {
			get {
				if (!filtered)
					throw new InvalidOperationException();
				return defs;
			}
		}

		InstructionDef[] defs;
		bool filtered;

		InstructionDefs(GenTypes genTypes) {
			defs = InstructionDefsData.CreateTable(genTypes);
			genTypes.AddObject(TypeIds.InstructionDefs, this);
		}

		public InstructionDef[] GetDefsPreFiltered() {
			if (filtered)
				throw new InvalidOperationException();
			return defs;
		}

		void ICreatedInstructions.OnCreatedInstructions(GenTypes genTypes, HashSet<EnumValue> filteredCodeValues) {
			defs = defs.Where(a => filteredCodeValues.Contains(a.OpCodeInfo.Code)).ToArray();
			filtered = true;
		}
	}
}
