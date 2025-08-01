using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHIP8Interpreter.Emulator
{
	public struct Opcode
	{
		public UInt16 FullOpcode;
		public Byte Instruction;
		public Byte X;
		public Byte Y;
		public Byte N;
		public Byte NN;
		public UInt16 NNN;

		public Opcode(UInt16 fullInstruction)
		{
			DecodeInstruction(fullInstruction);
		}

		private void DecodeInstruction(UInt16 fullInstruction)
		{
			FullOpcode = fullInstruction;
			Instruction = (Byte)(fullInstruction >> 12);
			X = (Byte)((fullInstruction >> 8) & 0xF);
			Y = (Byte)((fullInstruction >> 4) & 0xF);
			N = (Byte)(fullInstruction & 0xF);
			NN = (Byte)(fullInstruction & 0xFF);
			NNN = (UInt16)(fullInstruction & 0xFFF);
		}
	}
	public class Interpreter
	{
		private Chip8 _chip8;

		public Interpreter(Chip8 chip8)
		{
			this._chip8 = chip8;
		}

		public Opcode Fetch()
		{
			var currentInstruction = Peek();

			_chip8.ProgramCounter += 2;

			return currentInstruction;
		}

		public Opcode Peek()
		{
			return new((UInt16)(_chip8.RAM[_chip8.ProgramCounter] << 8));
		}
	}
}
