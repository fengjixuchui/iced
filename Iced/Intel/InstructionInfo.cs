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

#if !NO_INSTR_INFO
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Iced.Intel.InstructionInfoInternal;

namespace Iced.Intel {
	/// <summary>
	/// Contains information about an instruction, eg. read/written registers, read/written RFLAGS bits, CPUID feature bit, etc
	/// </summary>
	public struct InstructionInfo {
		internal UsedRegister[] usedRegisters;
		internal UsedMemory[] usedMemoryLocations;
		internal ushort usedRegistersLength;
		internal ushort usedMemoryLocationsLength;
		internal ushort opMaskFlags;
		internal byte cpuidFeature;
		internal byte flowControl;
		internal byte encoding;
		internal byte rflagsInfo;
		internal byte flags;

		[Flags]
		internal enum OpMaskFlags : ushort {
			OpAccessMask			= 7,
			Op1AccessShift			= 3,
			Op2AccessShift			= 6,
			Op3AccessShift			= 9,
			Op4AccessShift			= 12,
		}

		[Flags]
		internal enum Flags : byte {
			SaveRestore				= 0x01,
			StackInstruction		= 0x02,
			ProtectedMode			= 0x04,
			Privileged				= 0x08,
		}

		/// <summary>
		/// Gets a struct iterator that returns all read and written registers. There are some exceptions, this method doesn't return all used registers:
		/// 
		/// 1) If <see cref="SaveRestoreInstruction"/> is true, or
		/// 
		/// 2) If it's a <see cref="FlowControl.Call"/> or <see cref="FlowControl.Interrupt"/> instruction (call, sysenter, int n etc), it can read and write any register (including RFLAGS).
		/// </summary>
		/// <returns></returns>
		public readonly UsedRegisterIterator GetUsedRegisters() => new UsedRegisterIterator(usedRegisters, usedRegistersLength);

		/// <summary>
		/// Gets a struct iterator that returns all read and written memory locations
		/// </summary>
		/// <returns></returns>
		public readonly UsedMemoryIterator GetUsedMemory() => new UsedMemoryIterator(usedMemoryLocations, usedMemoryLocationsLength);

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
		public struct UsedRegisterIterator : IEnumerable<UsedRegister>, IEnumerator<UsedRegister> {
			readonly UsedRegister[] usedRegisters;
			readonly uint length;
			int index;

			internal UsedRegisterIterator(UsedRegister[] usedRegisters, uint length) {
				this.usedRegisters = usedRegisters;
				this.length = length;
				index = -1;
			}

			public UsedRegisterIterator GetEnumerator() => this;
			public UsedRegister Current => usedRegisters[index];

			public bool MoveNext() {
				index++;
				return (uint)index < length;
			}

			IEnumerator<UsedRegister> IEnumerable<UsedRegister>.GetEnumerator() => GetEnumerator();
			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
			UsedRegister IEnumerator<UsedRegister>.Current => Current;
			object IEnumerator.Current => Current;
			bool IEnumerator.MoveNext() => MoveNext();
			void IEnumerator.Reset() => throw new NotSupportedException();
			public void Dispose() { }
		}

		public struct UsedMemoryIterator : IEnumerable<UsedMemory>, IEnumerator<UsedMemory> {
			readonly UsedMemory[] usedMemoryLocations;
			readonly uint length;
			int index;

			internal UsedMemoryIterator(UsedMemory[] usedMemoryLocations, uint length) {
				this.usedMemoryLocations = usedMemoryLocations;
				this.length = length;
				index = -1;
			}

			public UsedMemoryIterator GetEnumerator() => this;
			public UsedMemory Current => usedMemoryLocations[index];

			public bool MoveNext() {
				index++;
				return (uint)index < length;
			}

			IEnumerator<UsedMemory> IEnumerable<UsedMemory>.GetEnumerator() => GetEnumerator();
			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
			UsedMemory IEnumerator<UsedMemory>.Current => Current;
			object IEnumerator.Current => Current;
			bool IEnumerator.MoveNext() => MoveNext();
			void IEnumerator.Reset() => throw new NotSupportedException();
			public void Dispose() { }
		}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

		/// <summary>
		/// true if the instruction isn't available in real mode or virtual 8086 mode
		/// </summary>
		[Obsolete("Use " + nameof(IsProtectedMode) + " instead", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public readonly bool ProtectedMode => IsProtectedMode;

		/// <summary>
		/// true if the instruction isn't available in real mode or virtual 8086 mode
		/// </summary>
		public readonly bool IsProtectedMode => (flags & (uint)Flags.ProtectedMode) != 0;

		/// <summary>
		/// true if this is a privileged instruction
		/// </summary>
		[Obsolete("Use " + nameof(IsPrivileged) + " instead", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public readonly bool Privileged => IsPrivileged;

		/// <summary>
		/// true if this is a privileged instruction
		/// </summary>
		public readonly bool IsPrivileged => (flags & (uint)Flags.Privileged) != 0;

		/// <summary>
		/// true if this is an instruction that implicitly uses the stack pointer (SP/ESP/RSP), eg. call, push, pop, ret, etc.
		/// See also <see cref="Instruction.StackPointerIncrement"/>
		/// </summary>
		[Obsolete("Use " + nameof(IsStackInstruction) + " instead", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public readonly bool StackInstruction => IsStackInstruction;

		/// <summary>
		/// true if this is an instruction that implicitly uses the stack pointer (SP/ESP/RSP), eg. call, push, pop, ret, etc.
		/// See also <see cref="Instruction.StackPointerIncrement"/>
		/// </summary>
		public readonly bool IsStackInstruction => (flags & (uint)Flags.StackInstruction) != 0;

		/// <summary>
		/// true if it's an instruction that saves or restores too many registers (eg. fxrstor, xsave, etc).
		/// <see cref="GetUsedRegisters"/> won't return all read/written registers.
		/// </summary>
		[Obsolete("Use " + nameof(IsSaveRestoreInstruction) + " instead", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public readonly bool SaveRestoreInstruction => IsSaveRestoreInstruction;

		/// <summary>
		/// true if it's an instruction that saves or restores too many registers (eg. fxrstor, xsave, etc).
		/// <see cref="GetUsedRegisters"/> won't return all read/written registers.
		/// </summary>
		public readonly bool IsSaveRestoreInstruction => (flags & (uint)Flags.SaveRestore) != 0;

		/// <summary>
		/// Instruction encoding, eg. legacy, VEX, EVEX, ...
		/// </summary>
		public readonly EncodingKind Encoding => (EncodingKind)encoding;

		/// <summary>
		/// CPU or CPUID feature flag
		/// </summary>
		[Obsolete("Use " + nameof(CpuidFeatures) + " instead", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public readonly CpuidFeature CpuidFeature => CpuidFeatures[0];

		/// <summary>
		/// Gets the CPU or CPUID feature flags
		/// </summary>
		public readonly CpuidFeature[] CpuidFeatures => CpuidFeatureInternalData.ToCpuidFeatures[cpuidFeature];

		/// <summary>
		/// Flow control info
		/// </summary>
		public readonly FlowControl FlowControl => (FlowControl)flowControl;

		/// <summary>
		/// Operand #0 access
		/// </summary>
		public readonly OpAccess Op0Access => (OpAccess)(opMaskFlags & (uint)OpMaskFlags.OpAccessMask);

		/// <summary>
		/// Operand #1 access
		/// </summary>
		public readonly OpAccess Op1Access => (OpAccess)(((uint)opMaskFlags >> (int)OpMaskFlags.Op1AccessShift) & (uint)OpMaskFlags.OpAccessMask);

		/// <summary>
		/// Operand #2 access
		/// </summary>
		public readonly OpAccess Op2Access => (OpAccess)(((uint)opMaskFlags >> (int)OpMaskFlags.Op2AccessShift) & (uint)OpMaskFlags.OpAccessMask);

		/// <summary>
		/// Operand #3 access
		/// </summary>
		public readonly OpAccess Op3Access => (OpAccess)(((uint)opMaskFlags >> (int)OpMaskFlags.Op3AccessShift) & (uint)OpMaskFlags.OpAccessMask);

		/// <summary>
		/// Operand #4 access
		/// </summary>
		public readonly OpAccess Op4Access => (OpAccess)(((uint)opMaskFlags >> (int)OpMaskFlags.Op4AccessShift) & (uint)OpMaskFlags.OpAccessMask);

		/// <summary>
		/// Gets operand access
		/// </summary>
		/// <param name="operand">Operand number, 0-4</param>
		/// <returns></returns>
		public readonly OpAccess GetOpAccess(int operand) {
			switch (operand) {
			case 0: return Op0Access;
			case 1: return Op1Access;
			case 2: return Op2Access;
			case 3: return Op3Access;
			case 4: return Op4Access;
			default:
				ThrowHelper.ThrowArgumentOutOfRangeException_operand();
				return 0;
			}
		}

		/// <summary>
		/// All flags that are read by the CPU when executing the instruction
		/// </summary>
		public readonly RflagsBits RflagsRead => (RflagsBits)RflagsInfoConstants.flagsRead[rflagsInfo];

		/// <summary>
		/// All flags that are written by the CPU, except those flags that are known to be undefined, always set or always cleared. See also <see cref="RflagsModified"/>
		/// </summary>
		public readonly RflagsBits RflagsWritten => (RflagsBits)RflagsInfoConstants.flagsWritten[rflagsInfo];

		/// <summary>
		/// All flags that are always cleared by the CPU
		/// </summary>
		public readonly RflagsBits RflagsCleared => (RflagsBits)RflagsInfoConstants.flagsCleared[rflagsInfo];

		/// <summary>
		/// All flags that are always set by the CPU
		/// </summary>
		public readonly RflagsBits RflagsSet => (RflagsBits)RflagsInfoConstants.flagsSet[rflagsInfo];

		/// <summary>
		/// All flags that are undefined after executing the instruction
		/// </summary>
		public readonly RflagsBits RflagsUndefined => (RflagsBits)RflagsInfoConstants.flagsUndefined[rflagsInfo];

		/// <summary>
		/// All flags that are modified by the CPU. This is <see cref="RflagsWritten"/> + <see cref="RflagsCleared"/> + <see cref="RflagsSet"/> + <see cref="RflagsUndefined"/>
		/// </summary>
		public readonly RflagsBits RflagsModified => (RflagsBits)RflagsInfoConstants.flagsModified[rflagsInfo];
	}
}
#endif
