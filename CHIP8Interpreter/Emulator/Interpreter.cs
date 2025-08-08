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
		private Random rnd = new();

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

			int result;
			byte flag;

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
							result = _chip8.VariableRegisters[instruction.X] + _chip8.VariableRegisters[instruction.Y];
							_chip8.VariableRegisters[instruction.X] = (Byte)result;
							_chip8.VariableRegisters[0xF] = (Byte)(result > 255 ? 1 : 0);
							break;
						case 5: //8XY5 Set VX to VX - VY
							result = _chip8.VariableRegisters[instruction.X] - _chip8.VariableRegisters[instruction.Y];
							_chip8.VariableRegisters[instruction.X] = (byte)result;
							_chip8.VariableRegisters[0xF] = (Byte)(result >= 0 ? 1 : 0);
							break;
						case 6: //8XY6 Shift, Legacy(Set VX to VY), Modern(nothing) and bit shift right 1, set VF to shifted out bit
							if (_compatibilityMode == CompatibilityMode.Legacy)
							{
								_chip8.VariableRegisters[instruction.X] = _chip8.VariableRegisters[instruction.Y];
							}

							result = (byte)(_chip8.VariableRegisters[instruction.X] >> 1);
							flag = (byte)((_chip8.VariableRegisters[instruction.X] & 1) == 1 ? 1 : 0);

							_chip8.VariableRegisters[instruction.X] = (byte)(result);

							_chip8.VariableRegisters[0xF] = flag;
							break;
						case 7: //8XY7 Set VX to VY - VX
							result = _chip8.VariableRegisters[instruction.Y] - _chip8.VariableRegisters[instruction.X];
							_chip8.VariableRegisters[instruction.X] = (byte)result;
							_chip8.VariableRegisters[0xF] = (Byte)(result >= 0 ? 1 : 0);
							break;
						case 0xE:   //8XYE Shift, Legacy(Set VX to VY), Modern(nothing) and bit shift left 1, set VF to shifted out bit
							if (_compatibilityMode == CompatibilityMode.Legacy)
							{
								_chip8.VariableRegisters[instruction.X] = _chip8.VariableRegisters[instruction.Y];
							}

							result = (byte)(_chip8.VariableRegisters[instruction.X] << 1);
							flag = (byte)((_chip8.VariableRegisters[instruction.X] & 0x80) == 0x80 ? 1 : 0);

							_chip8.VariableRegisters[instruction.X] = (byte)(result);

							_chip8.VariableRegisters[0xF] = flag;
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
				case 0xB:   //BNNN/BXNN Legacy(Jump to instruction NNN + V0) Modern(Jump to instruction XNN + VX)
					if (_compatibilityMode == CompatibilityMode.Legacy)
					{
						_chip8.ProgramCounter = (byte)(instruction.NNN + _chip8.VariableRegisters[0]);
					}
					else
					{
						_chip8.ProgramCounter = (byte)(instruction.NNN + _chip8.VariableRegisters[instruction.X]);
					}
					break;
				case 0xC:   //CXNN Generate a random number, AND with NN, and store in VX
					_chip8.VariableRegisters[instruction.X] = (byte)(rnd.Next(0, 0x100) & instruction.NN);
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
					switch (instruction.NN)
					{
						case 0x9E:  //EX9E Skip if key is down
							if (((_chip8.InputRegister >> (_chip8.VariableRegisters[instruction.X])) & 1) == 1)
							{
								_chip8.ProgramCounter += 2;
							}
							break;
						case 0xA1:  //EXA1 Skip if key is NOT down
							if (((_chip8.InputRegister >> (_chip8.VariableRegisters[instruction.X])) & 1) == 0)
							{
								_chip8.ProgramCounter += 2;
							}
							break;
						default:
							Debug.WriteLine("Instruction not found");
							break;
					}

					break;
				case 0xF:
					switch (instruction.NN)
					{
						case 0x07:  //FX07 Set VX to Delay Timer
							_chip8.VariableRegisters[instruction.X] = _chip8.DelayTimer;
							break;
						case 0x0A:  //Blocks until key X is pressed
							if (((_chip8.InputRegister >> instruction.X) & 1) == 0)
							{
								_chip8.ProgramCounter -= 2;
							}
							break;
						case 0x15:  //FX15 Set Delay Timer to VX
							_chip8.DelayTimer = _chip8.VariableRegisters[instruction.X];
							break;
						case 0x18:  //FX18 Set Sound Timer to VX
							_chip8.SoundTimer = _chip8.VariableRegisters[instruction.X];
							break;
						case 0x1E:  //FX1E Add VX to RAM I
							result = (int)(_chip8.IndexRegister) + (int)(_chip8.VariableRegisters[instruction.X]);
							flag = (Byte)(result >= 0x1000 ? 1 : 0);

							_chip8.IndexRegister = (UInt16)result;
							_chip8.VariableRegisters[0xF] = flag;
							break;
						case 0x33:  //FX33 Convert VX to decimal and stores in RAM I to I+2
							for (int i = 0; i < 3; i++)
							{
								_chip8.RAM[_chip8.IndexRegister + i] = (byte)(Math.Floor(_chip8.VariableRegisters[instruction.X] / Math.Pow(10, 2 - i)) % 10);
							}
							break;
						case 0x55:  //FX55 Save V0 to VX in RAM I to I+X, legacy(increment I)
							for (int i = 0; i <= instruction.X; i++)
							{
								_chip8.RAM[_chip8.IndexRegister + i] = _chip8.VariableRegisters[i];

								if (_compatibilityMode == CompatibilityMode.Legacy)
								{
									_chip8.IndexRegister++;
								}
							}
							break;
						case 0x65:  //FX65 Load RAM I to I+x in V0 to Vx, legacy(increment I)
							for (int i = 0; i <= instruction.X; i++)
							{
								_chip8.VariableRegisters[i] = _chip8.RAM[_chip8.IndexRegister + i];

								if (_compatibilityMode == CompatibilityMode.Legacy)
								{
									_chip8.IndexRegister++;
								}
							}
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
