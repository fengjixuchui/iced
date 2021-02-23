// SPDX-License-Identifier: MIT
// Copyright (C) 2018-present iced project and contributors

// ⚠️This file was generated by GENERATOR!🦹‍♂️

#nullable enable

#if DECODER
using System;

namespace Iced.Intel.DecoderInternal {
	[Flags]
	enum LegacyHandlerFlags : uint {
		HandlerReg = 0x00000001,
		HandlerMem = 0x00000002,
		Handler66Reg = 0x00000004,
		Handler66Mem = 0x00000008,
		HandlerF3Reg = 0x00000010,
		HandlerF3Mem = 0x00000020,
		HandlerF2Reg = 0x00000040,
		HandlerF2Mem = 0x00000080,
	}
}
#endif
