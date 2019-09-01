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
using System.Diagnostics;

namespace Iced.Intel {
	/// <summary>
	/// <see cref="Register"/> extension methods
	/// </summary>
	public static class RegisterExtensions {
		internal static readonly RegisterInfo[] RegisterInfos = GetRegisterInfos();
		static RegisterInfo[] GetRegisterInfos() {
			var regInfos = new RegisterInfo[DecoderConstants.NumberOfRegisters];

			regInfos[(int)Register.EIP] = new RegisterInfo(Register.EIP, Register.EIP, Register.RIP, 4);
			regInfos[(int)Register.RIP] = new RegisterInfo(Register.RIP, Register.EIP, Register.RIP, 8);

			var data = new byte[] {
				(byte)Register.AL, (byte)Register.R15L, (byte)Register.RAX, 1,
				(byte)Register.AX, (byte)Register.R15W, (byte)Register.RAX, 2,
				(byte)Register.EAX, (byte)Register.R15D, (byte)Register.RAX, 4,
				(byte)Register.RAX, (byte)Register.R15, (byte)Register.RAX, 8,
				(byte)Register.ES, (byte)Register.GS, (byte)Register.ES, 2,
				(byte)Register.XMM0, (byte)Register.XMM31, (byte)Register.ZMM0, 16,
				(byte)Register.YMM0, (byte)Register.YMM31, (byte)Register.ZMM0, 32,
				(byte)Register.ZMM0, (byte)Register.ZMM31, (byte)Register.ZMM0, 64,
				(byte)Register.K0, (byte)Register.K7, (byte)Register.K0, 8,
				(byte)Register.BND0, (byte)Register.BND3, (byte)Register.BND0, 16,
				(byte)Register.CR0, (byte)Register.CR15, (byte)Register.CR0, 8,
				(byte)Register.DR0, (byte)Register.DR15, (byte)Register.DR0, 8,
				(byte)Register.ST0, (byte)Register.ST7, (byte)Register.ST0, 10,
				(byte)Register.MM0, (byte)Register.MM7, (byte)Register.MM0, 8,
				(byte)Register.TR0, (byte)Register.TR7, (byte)Register.TR0, 4,
			};

			int i;
			for (i = 0; i < data.Length; i += 4) {
				var baseReg = (Register)data[i];
				var reg = baseReg;
				var regEnd = (Register)data[i + 1];
				var fullReg = (Register)data[i + 2];
				int size = data[i + 3];
				while (reg <= regEnd) {
					regInfos[(int)reg] = new RegisterInfo(reg, baseReg, fullReg, size);
					reg++;
					fullReg++;
					if (reg == Register.AH)
						fullReg -= 4;
				}
			}
			if (i != data.Length)
				throw new InvalidOperationException();

			return regInfos;
		}

		/// <summary>
		/// Gets register info
		/// </summary>
		/// <param name="register">Register</param>
		/// <returns></returns>
		public static RegisterInfo GetInfo(this Register register) {
			var infos = RegisterInfos;
			if ((uint)register >= (uint)infos.Length)
				ThrowHelper.ThrowArgumentOutOfRangeException_register();
			return infos[(int)register];
		}

		/// <summary>
		/// Gets the base register, eg. AL, AX, EAX, RAX, MM0, XMM0, YMM0, ZMM0, ES
		/// </summary>
		/// <param name="register">Register</param>
		/// <returns></returns>
		public static Register GetBaseRegister(this Register register) => register.GetInfo().Base;

		/// <summary>
		/// The register number (index) relative to <see cref="GetBaseRegister(Register)"/>, eg. 0-15, or 0-31, or if 8-bit GPR, 0-19
		/// </summary>
		/// <param name="register">Register</param>
		/// <returns></returns>
		public static int GetNumber(this Register register) => register.GetInfo().Number;

		/// <summary>
		/// Gets the full register that this one is a part of, eg. CL/CH/CX/ECX/RCX -> RCX, XMM11/YMM11/ZMM11 -> ZMM11
		/// </summary>
		/// <param name="register">Register</param>
		/// <returns></returns>
		public static Register GetFullRegister(this Register register) => register.GetInfo().FullRegister;

		/// <summary>
		/// Gets the full register that this one is a part of, except if it's a GPR in which case the 32-bit register is returned,
		/// eg. CL/CH/CX/ECX/RCX -> ECX, XMM11/YMM11/ZMM11 -> ZMM11
		/// </summary>
		/// <param name="register">Register</param>
		/// <returns></returns>
		public static Register GetFullRegister32(this Register register) => register.GetInfo().FullRegister32;

		/// <summary>
		/// Gets the size of the register in bytes
		/// </summary>
		/// <param name="register">Register</param>
		/// <returns></returns>
		public static int GetSize(this Register register) => register.GetInfo().Size;

		/// <summary>
		/// Checks if it's a segment register (ES, CS, SS, DS, FS, GS)
		/// </summary>
		/// <param name="register">Register</param>
		/// <returns></returns>
		public static bool IsSegmentRegister(this Register register) => Register.ES <= register && register <= Register.GS;

		/// <summary>
		/// Checks if it's a general purpose register (AL-R15L, AX-R15W, EAX-R15D, RAX-R15)
		/// </summary>
		/// <param name="register">Register</param>
		/// <returns></returns>
		public static bool IsGPR(this Register register) => Register.AL <= register && register <= Register.R15;

		/// <summary>
		/// Checks if it's an 8-bit general purpose register (AL-R15L)
		/// </summary>
		/// <param name="register">Register</param>
		/// <returns></returns>
		public static bool IsGPR8(this Register register) => Register.AL <= register && register <= Register.R15L;

		/// <summary>
		/// Checks if it's a 16-bit general purpose register (AX-R15W)
		/// </summary>
		/// <param name="register">Register</param>
		/// <returns></returns>
		public static bool IsGPR16(this Register register) => Register.AX <= register && register <= Register.R15W;

		/// <summary>
		/// Checks if it's a 32-bit general purpose register (EAX-R15D)
		/// </summary>
		/// <param name="register">Register</param>
		/// <returns></returns>
		public static bool IsGPR32(this Register register) => Register.EAX <= register && register <= Register.R15D;

		/// <summary>
		/// Checks if it's a 64-bit general purpose register (RAX-R15)
		/// </summary>
		/// <param name="register">Register</param>
		/// <returns></returns>
		public static bool IsGPR64(this Register register) => Register.RAX <= register && register <= Register.R15;

		/// <summary>
		/// Checks if it's a 128-bit vector register (XMM0-XMM31)
		/// </summary>
		/// <param name="register">Register</param>
		/// <returns></returns>
		public static bool IsXMM(this Register register) => Register.XMM0 <= register && register <= Register.XMM0 + InstructionInfoConstants.VMM_count - 1;

		/// <summary>
		/// Checks if it's a 256-bit vector register (YMM0-YMM31)
		/// </summary>
		/// <param name="register">Register</param>
		/// <returns></returns>
		public static bool IsYMM(this Register register) => Register.YMM0 <= register && register <= Register.YMM0 + InstructionInfoConstants.VMM_count - 1;

		/// <summary>
		/// Checks if it's a 512-bit vector register (ZMM0-ZMM31)
		/// </summary>
		/// <param name="register">Register</param>
		/// <returns></returns>
		public static bool IsZMM(this Register register) => Register.ZMM0 <= register && register <= Register.ZMM0 + InstructionInfoConstants.VMM_count - 1;

		/// <summary>
		/// Checks if it's an XMM, YMM or ZMM register
		/// </summary>
		/// <param name="register">Register</param>
		/// <returns></returns>
		public static bool IsVectorRegister(this Register register) => Register.XMM0 <= register && register <= Register.ZMM0 + InstructionInfoConstants.VMM_count - 1;
	}

	/// <summary>
	/// Register info
	/// </summary>
	public readonly struct RegisterInfo {
		readonly byte baseRegister;
		readonly byte fullRegister;
		readonly byte size;
		readonly byte register;

		/// <summary>
		/// Gets the register
		/// </summary>
		public Register Register => (Register)register;

		/// <summary>
		/// Gets the base register, eg. AL, AX, EAX, RAX, MM0, XMM0, YMM0, ZMM0, ES
		/// </summary>
		public Register Base => (Register)baseRegister;

		/// <summary>
		/// The register number (index) relative to <see cref="Base"/>, eg. 0-15, or 0-31, or if 8-bit GPR, 0-19
		/// </summary>
		public int Number => register - baseRegister;

		/// <summary>
		/// The full register that this one is a part of, eg. CL/CH/CX/ECX/RCX -> RCX, XMM11/YMM11/ZMM11 -> ZMM11
		/// </summary>
		public Register FullRegister => (Register)fullRegister;

		/// <summary>
		/// Gets the full register that this one is a part of, except if it's a GPR in which case the 32-bit register is returned,
		/// eg. CL/CH/CX/ECX/RCX -> ECX, XMM11/YMM11/ZMM11 -> ZMM11
		/// </summary>
		public Register FullRegister32 {
			get {
				var fullRegister = (Register)this.fullRegister;
				if (fullRegister.IsGPR()) {
					Debug.Assert(Register.RAX <= fullRegister && fullRegister <= Register.R15);
					return fullRegister - Register.RAX + Register.EAX;
				}
				return fullRegister;
			}
		}

		/// <summary>
		/// Size of the register in bytes
		/// </summary>
		public int Size => size;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="register">Register</param>
		/// <param name="baseRegister">Base register, eg. AL, AX, EAX, RAX, XMM0, YMM0, ZMM0, ES</param>
		/// <param name="fullRegister">Full register, eg. RAX, ZMM0, ES</param>
		/// <param name="size">Size of register in bytes</param>
		public RegisterInfo(Register register, Register baseRegister, Register fullRegister, int size) {
			Debug.Assert(baseRegister <= register);
			Debug.Assert((uint)register <= byte.MaxValue);
			this.register = (byte)register;
			Debug.Assert((uint)baseRegister <= byte.MaxValue);
			this.baseRegister = (byte)baseRegister;
			Debug.Assert((uint)fullRegister <= byte.MaxValue);
			this.fullRegister = (byte)fullRegister;
			Debug.Assert((uint)size <= byte.MaxValue);
			this.size = (byte)size;
		}
	}
}
#endif
