# CHIP-8 Interpreter 
This CHIP-8 interpreter was implemented using C# and the MonoGame framework.

This project is dedicated to the public domain under the CC0 license. The files found in the ROMs directory have their own licensing, which is clarified in Licenses.txt.

## ❗Overview
- This project functions with 3 main components:
  - The Chip8 class holds all the data associated with the interpreter, such as the RAM and registers.
  - The Interpreter class fetches, decodes, and executes the instructions from RAM.
  - The Game class uses MonoGame to show the display, play sounds, and handle input.
- The project has support for both legacy and modern CHIP-8 instruction sets

# Special thanks
[Tobias's excellent guide](https://tobiasvl.github.io/blog/write-a-chip-8-emulator) as well as [Tim's test ROMs](https://github.com/Timendus/chip8-test-suite?tab=readme-ov-file).
