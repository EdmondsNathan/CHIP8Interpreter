using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHIP8Interpreter.Emulator
{
	public struct Instruction
	{
		public UInt16 FullInstruction;
		public Byte HighNibble;
		public Byte X;
		public Byte Y;
		public Byte N;
		public Byte NN;
		public UInt16 NNN;

		public Instruction(UInt16 fullInstruction)
		{
			DecodeInstruction(fullInstruction);
		}

		private void DecodeInstruction(UInt16 fullInstruction)
		{
			FullInstruction = fullInstruction;
			HighNibble = (Byte)(fullInstruction >> 12);
			X = (Byte)((fullInstruction >> 8) & 0x000F);
			Y = (Byte)((fullInstruction >> 4) & 0x000F);
			N = (Byte)(fullInstruction >> 0 & 0x000F);
			NN = (Byte)(fullInstruction & 0x00FF);
			NNN = (UInt16)(fullInstruction & 0x0FFF);
		}
	}

	public enum CompatibilityMode
	{
		Legacy,
		Modern
	}

	public class Interpreter
	{
		private Chip8 _chip8;
		private CompatibilityMode _compatibilityMode;

		public Interpreter(Chip8 chip8, CompatibilityMode compatibilityMode = CompatibilityMode.Modern)
		{
			this._chip8 = chip8;
			this._compatibilityMode = compatibilityMode;
		}

		public Instruction Fetch()
		{
			var currentInstruction = Peek();

			_chip8.ProgramCounter += 2;

			return currentInstruction;
		}

		public Instruction Peek()
		{
			return new((UInt16)((_chip8.RAM[_chip8.ProgramCounter] << 8) + (_chip8.RAM[_chip8.ProgramCounter + 1])));
		}

		public void Execute(Instruction instruction)
		{
			if (instruction.FullInstruction == 0)
			{
				return;
			}

			Debug.WriteLine(Convert.ToString(instruction.FullInstruction, 16));

			switch (instruction.HighNibble)
			{
				case 0x0:
					switch (instruction.FullInstruction)
					{
						case 0x00E0:    //Clear Screen
							for (int row = 0; row < _chip8.Display.Length; row++)
							{
								_chip8.Display[row] = 0;
							}
							break;
						case 0x00EE:    //Return from subroutine

							break;
					}
					break;
				case 0x1:   //Jump Program Counter to NNN
					_chip8.ProgramCounter = (UInt16)(instruction.NNN);
					break;
				case 0x2:

					break;
				case 0x3:

					break;
				case 0x4:

					break;
				case 0x5:

					break;
				case 0x6:   //Set register X to NN
					_chip8.VariableRegisters[instruction.X] = instruction.NN;
					break;
				case 0x7:
					_chip8.VariableRegisters[instruction.X] += instruction.NN;
					break;
				case 0x8:

					break;
				case 0x9:

					break;
				case 0xA:   //Set Index I to NNN
					_chip8.IndexRegister = (UInt16)(instruction.NNN);
					break;
				case 0xB:

					break;
				case 0xC:

					break;
				case 0xD:   //Draw N pixels tall sprite from Index Register I's memory location at vX and vY
					Byte x = (Byte)(_chip8.VariableRegisters[instruction.X] % Chip8.DisplayWidth);
					Byte y = (Byte)(_chip8.VariableRegisters[instruction.Y] % Chip8.DisplayHeight);

					_chip8.VariableRegisters[0xF] = 0;

					for (int i = 0; i < instruction.N; i++)
					{
						if (y + i >= Chip8.DisplayHeight - 1)       //clip sprite
						{
							break;
						}

						Byte row = _chip8.RAM[i + _chip8.IndexRegister];

						for (int e = 0; e < 8; e++)
						{
							if (x + e >= Chip8.DisplayWidth - 1)    //clip sprite
							{
								break;
							}

							if (((row >> (7 - e)) & 1) == 0)    //is sprite pixel 0?
							{
								continue;
							}
							//sprite pixel is 1

							if (((_chip8.Display[y + i] >> (Chip8.DisplayWidth - (x + e))) & 0x1) == 0)    //is screen pixel 0?
							{
								_chip8.Display[y + i] += (UInt64)Math.Pow(2, Chip8.DisplayWidth - (x + e));
							}
							else
							{
								_chip8.Display[y + i] -= (UInt64)Math.Pow(2, Chip8.DisplayWidth - (x + e));

								_chip8.VariableRegisters[0xF] = 1;
							}
						}
					}
					break;
				case 0xE:

					break;
				case 0xF:

					break;
				default:
					Debug.WriteLine("Instruction not found");
					break;
			}
		}
	}
}
