# CHIP-8 Interpreter 
This CHIP-8 interpreter was implemented using C# and the MonoGame framework.

Special thanks to [Tobias's excellent guide](https://tobiasvl.github.io/blog/write-a-chip-8-emulator) as well as [Tim's test ROMs](https://github.com/Timendus/chip8-test-suite?tab=readme-ov-file).

This project is released under the CC0 license. Feel free to do whatever you want with this code! No attribution is necessary, but I would love to hear if you make use of it :)

## ❗Overview
- This project functions with 3 main components:
  - The Chip8 class holds all the data associated with the interpreter, such as the RAM and registers.
  - The Interpreter class fetches, decodes, and executes the instructions from RAM.
  - The Game class uses MonoGame to show the display and handle input.
- The project has support for both legacy and modern CHIP-8 instruction sets

## 📃To-Do
- [ ] Implement input
- [x] Run the interpreter at an appropriate clock rate
- [ ] Implement timers
- [ ] Implement sound
- [x] ~Implement all instructions~
- [x] ~Show the display on screen~ 
