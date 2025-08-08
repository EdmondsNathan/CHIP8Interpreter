using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHIP8Interpreter.Emulator
{
	public class Chip8
	{
		public Byte[] RAM = new Byte[4096];
		public UInt64[] Display = new UInt64[32];
		public UInt16 ProgramCounter = 512;
		public UInt16 IndexRegister;
		public Stack<UInt16> SubStack = new(16);
		public Byte DelayTimer;
		public Byte SoundTimer;
		public Byte[] VariableRegisters = new Byte[16];
		public UInt16 InputRegister;

		public const int DisplayWidth = 64;
		public const int DisplayHeight = 32;
		public const UInt16 RomStartingAddress = 0x200;
		public const int FontStartingAddress = 0x050;
		private static readonly Byte[] _font =
		{
			0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
			0x20, 0x60, 0x20, 0x20, 0x70, // 1
			0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
			0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
			0x90, 0x90, 0xF0, 0x10, 0x10, // 4
			0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
			0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
			0xF0, 0x10, 0x20, 0x40, 0x40, // 7
			0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
			0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
			0xF0, 0x90, 0xF0, 0x90, 0x90, // A
			0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
			0xF0, 0x80, 0x80, 0x80, 0xF0, // C
			0xE0, 0x90, 0x90, 0x90, 0xE0, // D
			0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
			0xF0, 0x80, 0xF0, 0x80, 0x80  // F
		};

		public Chip8()
		{
			PutFontIntoMemory();
		}

		public Chip8(string filePath) : this()
		{
			Byte[] rom = File.ReadAllBytes(filePath);

			for (int i = 0; i < rom.Length; i++)
			{
				this.RAM[RomStartingAddress + i] = rom[i];
			}
		}

		private void PutFontIntoMemory()
		{
			for (int i = 0; i < _font.Length; i++)
			{
				RAM[FontStartingAddress + i] = _font[i];
			}
		}
	}
}
