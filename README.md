# CHIP-8 Interpreter 🖥️
This CHIP-8 interpreter was implemented using C# and the MonoGame framework.

This project is dedicated to the public domain under the CC0 license. The files found in the ROMs directory have their own licensing, which is clarified in [LICENSE.txt](https://github.com/EdmondsNathan/CHIP8Interpreter/blob/main/Licenses/LICENSE.txt).

## More Info 📝
I have written a [blog post](https://edmondsnathan.github.io/2025/08/16/What-I-Learned-CHIP-8-Interpreter.html) which goes into detail of how I implemented the interpreter.

## Overview ❗
- This project functions with 3 main components:
  - The [Chip8 class](https://github.com/EdmondsNathan/CHIP8Interpreter/blob/main/CHIP8Interpreter/Emulator/Chip8.cs) holds all the data associated with the interpreter, such as the RAM and registers.
  - The [Interpreter class](https://github.com/EdmondsNathan/CHIP8Interpreter/blob/main/CHIP8Interpreter/Emulator/Interpreter.cs) fetches, decodes, and executes the instructions from RAM.
  - The [Game class](https://github.com/EdmondsNathan/CHIP8Interpreter/blob/main/CHIP8Interpreter/Game1.cs) uses MonoGame to show the display, play sounds, and handle input.
- The project has support for both legacy and modern CHIP-8 instruction sets

# Special Thanks 🙏
A very big thanks to [Tobias's excellent guide](https://tobiasvl.github.io/blog/write-a-chip-8-emulator) as well as [Tim's test ROMs](https://github.com/Timendus/chip8-test-suite?tab=readme-ov-file). These resources made this project possible.
