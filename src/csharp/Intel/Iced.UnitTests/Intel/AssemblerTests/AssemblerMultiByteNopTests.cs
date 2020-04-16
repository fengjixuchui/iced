#if ENCODER && BLOCK_ENCODER
using System;
using Xunit;

namespace Iced.UnitTests.Intel.AssemblerTests {
	public class AssemblerMultiByteNopTests16 : AssemblerTestsBase {
		public AssemblerMultiByteNopTests16() : base(16) { }

		[Fact]
		public void Test16Bit() {
			TestAssemblerDeclareData(c => c.nop(0), new byte[] { });
			TestAssemblerDeclareData(c => c.nop(1), new byte[] { 0x90 }); //NOP
			TestAssemblerDeclareData(c => c.nop(2), new byte[] { 0x66, 0x90 }); //66 NOP
			TestAssemblerDeclareData(c => c.nop(3), new byte[] { 0x0F, 0x1F, 0x00 }); //NOP word ptr [bx+si]
			TestAssemblerDeclareData(c => c.nop(4), new byte[] { 0x0F, 0x1F, 0x40, 0x00 }); //NOP word ptr [bx+si]
			TestAssemblerDeclareData(c => c.nop(5), new byte[] { 0x0F, 0x1F, 0x80, 0x00, 0x00 }); //NOP word ptr [bx+si]
			TestAssemblerDeclareData(c => c.nop(6), new byte[] { 0x66, 0x0F, 0x1F, 0x80, 0x00, 0x00 }); //NOP word ptr[bx + si]
			TestAssemblerDeclareData(c => c.nop(7), new byte[] { 0x67, 0x66, 0x0F, 0x1F, 0x44, 0x00, 0x00 }); //NOP dword ptr [eax+eax]
			TestAssemblerDeclareData(c => c.nop(8), new byte[] { 0x67, 0x0F, 0x1F, 0x80, 0x00, 0x00, 0x00, 0x00 }); //NOP word ptr [eax]
			TestAssemblerDeclareData(c => c.nop(9), new byte[] { 0x67, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00 }); //NOP word ptr [eax+eax]

			TestAssemblerDeclareData(c => c.nop(10), new byte[] {
				0x67, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00,	//NOP word ptr [eax+eax] 	
				0x90													//NOP
			});
			TestAssemblerDeclareData(c => c.nop(11), new byte[] {
				0x67, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00,	//NOP word ptr [eax+eax] 	
				0x66, 0x90												//66 NOP
			});
			TestAssemblerDeclareData(c => c.nop(12), new byte[] {
				0x67, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00,	//NOP word ptr [eax+eax] 	
				0x0F, 0x1F, 0x00										//NOP word ptr [bx+si]
			});
			TestAssemblerDeclareData(c => c.nop(13), new byte[] {
				0x67, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00,	//NOP word ptr [eax+eax] 	
				0x0F, 0x1F, 0x40, 0x00									//NOP word ptr [bx+si]
			});
			TestAssemblerDeclareData(c => c.nop(14), new byte[] {
				0x67, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00,	//NOP word ptr [eax+eax] 	
				0x0F, 0x1F, 0x80, 0x00, 0x00							//NOP word ptr [bx+si]
			});
			TestAssemblerDeclareData(c => c.nop(15), new byte[] {
				0x67, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00,	//NOP word ptr [eax+eax] 	
				0x66, 0x0F, 0x1F, 0x80, 0x00, 0x00						//NOP word ptr[bx + si]
			});
			TestAssemblerDeclareData(c => c.nop(16), new byte[] {
				0x67, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00,	//NOP word ptr [eax+eax] 	
				0x67, 0x66, 0x0F, 0x1F, 0x44, 0x00, 0x00				//NOP dword ptr [bx+si]
			});
			TestAssemblerDeclareData(c => c.nop(17), new byte[] {
				0x67, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00,	//NOP word ptr [eax+eax] 	
				0x67, 0x0F, 0x1F, 0x80, 0x00, 0x00, 0x00, 0x00			//NOP dword ptr [eax+eax]
			});
			TestAssemblerDeclareData(c => c.nop(18), new byte[] {
				0x67, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00,	//NOP word ptr [eax+eax] 	
				0x67, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00 	//NOP word ptr [eax]	
			});
			TestAssemblerDeclareData(c => c.nop(19), new byte[] {
				0x67, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00,	//NOP word ptr [eax+eax] 	
				0x67, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00,	//NOP word ptr [eax+eax] 	
				0x90													//NOP
			});

			Assert.Throws<ArgumentOutOfRangeException>(() => TestAssemblerDeclareData(c => c.nop(-1), new byte[] { 0x90 }));
		}
	}

	public class AssemblerMultiByteNopTests32 : AssemblerTestsBase {
		public AssemblerMultiByteNopTests32() : base(32) { }

		[Fact]
		public void Test32Bit() {
			TestAssemblerDeclareData(c => c.nop(0), new byte[] { });
			TestAssemblerDeclareData(c => c.nop(1), new byte[] { 0x90 }); //NOP
			TestAssemblerDeclareData(c => c.nop(2), new byte[] { 0x66, 0x90 }); //66 NOP
			TestAssemblerDeclareData(c => c.nop(3), new byte[] { 0x0F, 0x1F, 0x00 }); //NOP dword ptr [eax]
			TestAssemblerDeclareData(c => c.nop(4), new byte[] { 0x0F, 0x1F, 0x40, 0x00 }); //NOP dword ptr [eax + 00]
			TestAssemblerDeclareData(c => c.nop(5), new byte[] { 0x0F, 0x1F, 0x44, 0x00, 0x00 }); //NOP dword ptr [eax + eax*1 + 00]
			TestAssemblerDeclareData(c => c.nop(6), new byte[] { 0x66, 0x0F, 0x1F, 0x44, 0x00, 0x00 }); //66 NOP dword ptr [eax + eax*1 + 00]
			TestAssemblerDeclareData(c => c.nop(7), new byte[] { 0x0F, 0x1F, 0x80, 0x00, 0x00, 0x00, 0x00 }); //NOP dword ptr [eax + 00000000]
			TestAssemblerDeclareData(c => c.nop(8), new byte[] { 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00 }); //NOP dword ptr [eax + eax*1 + 00000000]
			TestAssemblerDeclareData(c => c.nop(9), new byte[] { 0x66, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00 }); //66 NOP dword ptr [eax + eax*1 + 00000000] 	

			TestAssemblerDeclareData(c => c.nop(10), new byte[] {
				0x66, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00,	//66 NOP dword ptr [eax + eax*1 + 00000000] 	
				0x90													//NOP
			});
			TestAssemblerDeclareData(c => c.nop(11), new byte[] {
				0x66, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00,	//66 NOP dword ptr [eax + eax*1 + 00000000] 	
				0x66, 0x90												//66 NOP
			});
			TestAssemblerDeclareData(c => c.nop(12), new byte[] {
				0x66, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00,	//66 NOP dword ptr [eax + eax*1 + 00000000] 	
				0x0F, 0x1F, 0x00										//NOP dword ptr [eax]
			});
			TestAssemblerDeclareData(c => c.nop(13), new byte[] {
				0x66, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00,	//66 NOP dword ptr [eax + eax*1 + 00000000] 	
				0x0F, 0x1F, 0x40, 0x00									//NOP dword ptr [eax + 00]
			});
			TestAssemblerDeclareData(c => c.nop(14), new byte[] {
				0x66, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00,	//66 NOP dword ptr [eax + eax*1 + 00000000] 	
				0x0F, 0x1F, 0x44, 0x00, 0x00							//NOP dword ptr [eax + eax*1 + 00]
			});
			TestAssemblerDeclareData(c => c.nop(15), new byte[] {
				0x66, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00,	//66 NOP dword ptr [eax + eax*1 + 00000000] 	
				0x66, 0x0F, 0x1F, 0x44, 0x00, 0x00						//66 NOP dword ptr [eax + eax*1 + 00]
			});
			TestAssemblerDeclareData(c => c.nop(16), new byte[] {
				0x66, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00,	//66 NOP dword ptr [eax + eax*1 + 00000000] 	
				0x0F, 0x1F, 0x80, 0x00, 0x00, 0x00, 0x00				//NOP dword ptr [eax + 00000000]
			});
			TestAssemblerDeclareData(c => c.nop(17), new byte[] {
				0x66, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00,	//66 NOP dword ptr [eax + eax*1 + 00000000] 	
				0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00			//NOP dword ptr [eax + eax*1 + 00000000]
			});
			TestAssemblerDeclareData(c => c.nop(18), new byte[] {
				0x66, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00,	//66 NOP dword ptr [eax + eax*1 + 00000000] 	
				0x66, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00 	//66 NOP dword ptr [eax + eax*1 + 00000000] 	
			});
			TestAssemblerDeclareData(c => c.nop(19), new byte[] {
				0x66, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00,	//66 NOP dword ptr [eax + eax*1 + 00000000] 	
				0x66, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00,	//66 NOP dword ptr [eax + eax*1 + 00000000] 	
				0x90													//NOP
			});

			Assert.Throws<ArgumentOutOfRangeException>(() => TestAssemblerDeclareData(c => c.nop(-1), new byte[] { 0x90 }));
		}
	}
	public class AssemblerMultiByteNopTests64 : AssemblerTestsBase {
		public AssemblerMultiByteNopTests64() : base(64) { }

		[Fact]
		public void Test64Bit() {
			TestAssemblerDeclareData(c => c.nop(0), new byte[] { });
			TestAssemblerDeclareData(c => c.nop(1), new byte[] { 0x90 }); //NOP
			TestAssemblerDeclareData(c => c.nop(2), new byte[] { 0x66, 0x90 }); //66 NOP
			TestAssemblerDeclareData(c => c.nop(3), new byte[] { 0x0F, 0x1F, 0x00 }); //NOP dword ptr [eax]
			TestAssemblerDeclareData(c => c.nop(4), new byte[] { 0x0F, 0x1F, 0x40, 0x00 }); //NOP dword ptr [eax + 00]
			TestAssemblerDeclareData(c => c.nop(5), new byte[] { 0x0F, 0x1F, 0x44, 0x00, 0x00 }); //NOP dword ptr [eax + eax*1 + 00]
			TestAssemblerDeclareData(c => c.nop(6), new byte[] { 0x66, 0x0F, 0x1F, 0x44, 0x00, 0x00 }); //66 NOP dword ptr [eax + eax*1 + 00]
			TestAssemblerDeclareData(c => c.nop(7), new byte[] { 0x0F, 0x1F, 0x80, 0x00, 0x00, 0x00, 0x00 }); //NOP dword ptr [eax + 00000000]
			TestAssemblerDeclareData(c => c.nop(8), new byte[] { 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00 }); //NOP dword ptr [eax + eax*1 + 00000000]
			TestAssemblerDeclareData(c => c.nop(9), new byte[] { 0x66, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00 }); //66 NOP dword ptr [eax + eax*1 + 00000000] 	

			TestAssemblerDeclareData(c => c.nop(10), new byte[] {
				0x66, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00,	//66 NOP dword ptr [eax + eax*1 + 00000000] 	
				0x90													//NOP
			});
			TestAssemblerDeclareData(c => c.nop(11), new byte[] {
				0x66, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00,	//66 NOP dword ptr [eax + eax*1 + 00000000] 	
				0x66, 0x90												//66 NOP
			});
			TestAssemblerDeclareData(c => c.nop(12), new byte[] {
				0x66, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00,	//66 NOP dword ptr [eax + eax*1 + 00000000] 	
				0x0F, 0x1F, 0x00										//NOP dword ptr [eax]
			});
			TestAssemblerDeclareData(c => c.nop(13), new byte[] {
				0x66, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00,	//66 NOP dword ptr [eax + eax*1 + 00000000] 	
				0x0F, 0x1F, 0x40, 0x00									//NOP dword ptr [eax + 00]
			});
			TestAssemblerDeclareData(c => c.nop(14), new byte[] {
				0x66, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00,	//66 NOP dword ptr [eax + eax*1 + 00000000] 	
				0x0F, 0x1F, 0x44, 0x00, 0x00							//NOP dword ptr [eax + eax*1 + 00]
			});
			TestAssemblerDeclareData(c => c.nop(15), new byte[] {
				0x66, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00,	//66 NOP dword ptr [eax + eax*1 + 00000000] 	
				0x66, 0x0F, 0x1F, 0x44, 0x00, 0x00						//66 NOP dword ptr [eax + eax*1 + 00]
			});
			TestAssemblerDeclareData(c => c.nop(16), new byte[] {
				0x66, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00,	//66 NOP dword ptr [eax + eax*1 + 00000000] 	
				0x0F, 0x1F, 0x80, 0x00, 0x00, 0x00, 0x00				//NOP dword ptr [eax + 00000000]
			});
			TestAssemblerDeclareData(c => c.nop(17), new byte[] {
				0x66, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00,	//66 NOP dword ptr [eax + eax*1 + 00000000] 	
				0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00			//NOP dword ptr [eax + eax*1 + 00000000]
			});
			TestAssemblerDeclareData(c => c.nop(18), new byte[] {
				0x66, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00,	//66 NOP dword ptr [eax + eax*1 + 00000000] 	
				0x66, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00 	//66 NOP dword ptr [eax + eax*1 + 00000000] 	
			});
			TestAssemblerDeclareData(c => c.nop(19), new byte[] {
				0x66, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00,	//66 NOP dword ptr [eax + eax*1 + 00000000] 	
				0x66, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00,	//66 NOP dword ptr [eax + eax*1 + 00000000] 	
				0x90													//NOP
			});

			Assert.Throws<ArgumentOutOfRangeException>(() => TestAssemblerDeclareData(c => c.nop(-1), new byte[] { 0x90 }));
		}
	}
}
#endif
