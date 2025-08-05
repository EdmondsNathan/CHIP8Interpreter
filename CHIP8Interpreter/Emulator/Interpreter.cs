using System;
using System.Diagnostics;

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

			switch (instruction.HighNibble)
			{
				case 0x0:
					switch (instruction.FullInstruction)
					{
						case 0x00E0:    //00E0 Clear Screen
							for (int row = 0; row < _chip8.Display.Length; row++)
							{
								_chip8.Display[row] = 0;
							}
							break;
						case 0x00EE:    //00EE Return from subroutine, Set PC to sub stack and pop
							_chip8.ProgramCounter = _chip8.SubStack.Pop();
							break;
						default:
							Debug.WriteLine("Instruction not found");
							break;
					}
					break;
				case 0x1:   //1NNN Jump Program Counter to NNN
					_chip8.ProgramCounter = (UInt16)(instruction.NNN);
					break;
				case 0x2:   //2NNN Subroutine, push PC to sub stack, jump PC to NNN
					_chip8.SubStack.Push(_chip8.ProgramCounter);
					_chip8.ProgramCounter = instruction.NNN;
					break;
				case 0x3:   //3XNN Skip if VX == NN
					if (_chip8.VariableRegisters[instruction.X] == instruction.NN)
					{
						_chip8.ProgramCounter += 2;
					}
					break;
				case 0x4:   //4XNN Skip if VX != NN
					if (_chip8.VariableRegisters[instruction.X] != instruction.NN)
					{
						_chip8.ProgramCounter += 2;
					}
					break;
				case 0x5:   //5XY0 Skip if VX == VY
					if (_chip8.VariableRegisters[instruction.X] == _chip8.VariableRegisters[instruction.Y])
					{
						_chip8.ProgramCounter += 2;
					}
					break;
				case 0x6:   //6XNN Set register X to NN
					_chip8.VariableRegisters[instruction.X] = instruction.NN;
					break;
				case 0x7:   //7XNN Add NN to VX
					_chip8.VariableRegisters[instruction.X] += instruction.NN;
					break;
				case 0x8:
					switch (instruction.N)
					{
						case 0: //8XY0 Set VX = VY
							_chip8.VariableRegisters[instruction.X] = _chip8.VariableRegisters[instruction.Y];
							break;
						case 1: //8XY1 Set VX to VX | VY
							_chip8.VariableRegisters[instruction.X] = (Byte)(_chip8.VariableRegisters[instruction.X] | _chip8.VariableRegisters[instruction.Y]);
							break;
						case 2: //8XY2 Set VX to VX & VY
							_chip8.VariableRegisters[instruction.X] = (Byte)(_chip8.VariableRegisters[instruction.X] & _chip8.VariableRegisters[instruction.Y]);
							break;
						case 3: //8XY3 Set VX to VX ^ VY
							_chip8.VariableRegisters[instruction.X] = (Byte)(_chip8.VariableRegisters[instruction.X] ^ _chip8.VariableRegisters[instruction.Y]);
							break;
						case 4: //8XY4 Set VX to VX + VY
							int result = _chip8.VariableRegisters[instruction.X] + _chip8.VariableRegisters[instruction.Y];
							_chip8.VariableRegisters[instruction.X] = (Byte)result;
							_chip8.VariableRegisters[0xF] = (Byte)(result > 255 ? 1 : 0);
							break;
						case 5: //8XY5 Set VX to VX - VY
							_chip8.VariableRegisters[0xF] = (Byte)(_chip8.VariableRegisters[instruction.X] > _chip8.VariableRegisters[instruction.Y] ? 1 : 0);
							_chip8.VariableRegisters[instruction.X] -= _chip8.VariableRegisters[instruction.Y];
							break;
						case 6: //8XY6 Shift, Legacy(Set VX to VY), Modern(nothing) and bit shift right 1, set VF to shifted out bit
							if (_compatibilityMode == CompatibilityMode.Legacy)
							{
								_chip8.VariableRegisters[instruction.X] = _chip8.VariableRegisters[instruction.Y];
							}

							_chip8.VariableRegisters[0xF] = (Byte)(_chip8.VariableRegisters[instruction.X] & 1);

							_chip8.VariableRegisters[instruction.X] = (Byte)(_chip8.VariableRegisters[instruction.X] >> 1);
							break;
						case 7: //8XY7 Set VX to VY - VX
							_chip8.VariableRegisters[0xF] = (Byte)(_chip8.VariableRegisters[instruction.Y] > _chip8.VariableRegisters[instruction.X] ? 1 : 0);
							_chip8.VariableRegisters[instruction.X] = (Byte)(_chip8.VariableRegisters[instruction.Y] - _chip8.VariableRegisters[instruction.X]);
							break;
						case 0xE:   //8XYE Shift, Legacy(Set VX to VY), Modern(nothing) and bit shift left 1, set VF to shifted out bit
							if (_compatibilityMode == CompatibilityMode.Legacy)
							{
								_chip8.VariableRegisters[instruction.X] = _chip8.VariableRegisters[instruction.Y];
							}

							_chip8.VariableRegisters[0xF] = (Byte)(_chip8.VariableRegisters[instruction.X] & 0x80);

							_chip8.VariableRegisters[instruction.X] = (Byte)(_chip8.VariableRegisters[instruction.X] << 1);
							break;
						default:
							Debug.WriteLine("Instruction not found");
							break;
					}
					break;
				case 0x9:   //9XY0 Skip if VX != VY
					if (_chip8.VariableRegisters[instruction.X] != _chip8.VariableRegisters[instruction.Y])
					{
						_chip8.ProgramCounter += 2;
					}
					break;
				case 0xA:   //ANNN Set Index I to NNN
					_chip8.IndexRegister = (UInt16)(instruction.NNN);
					break;
				case 0xB:

					break;
				case 0xC:

					break;
				case 0xD:   //DXYN Draw N pixels tall sprite from Index Register I's memory location at vX and vY
					Byte x = (Byte)(_chip8.VariableRegisters[instruction.X] % Chip8.DisplayWidth);
					Byte y = (Byte)(_chip8.VariableRegisters[instruction.Y] % Chip8.DisplayHeight);

					_chip8.VariableRegisters[0xF] = 0;

					for (int i = 0; i < instruction.N; i++)
					{
						if (y + i >= Chip8.DisplayHeight - 1)       //clip sprite
						{
							break;
						}

						Byte row = _chip8.RAM[_chip8.IndexRegister + i];

						for (int e = 0; e < 8; e++)
						{
							if (x + e > Chip8.DisplayWidth - 1)    //clip sprite
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
								_chip8.Display[y + i] += (UInt64)(1) << (Chip8.DisplayWidth - x - e);
							}
							else
							{
								_chip8.Display[y + i] -= (UInt64)(1) << (Chip8.DisplayWidth - x - e);

								_chip8.VariableRegisters[0xF] = 1;
							}
						}
					}
					break;
				case 0xE:

					break;
				case 0xF:
					switch (instruction.NN)
					{
						case 0x07:  //FX07 Set VX to Delay Timer
							_chip8.VariableRegisters[instruction.X] = _chip8.DelayTimer;
							break;
						case 0x15:  //FX15 Set Delay Timer to VX
							_chip8.DelayTimer = _chip8.VariableRegisters[instruction.X];
							break;
						default:
							Debug.WriteLine("Instruction not found");
							break;
					}

					break;
				default:
					Debug.WriteLine("Instruction not found");
					break;
			}
		}
	}
}
