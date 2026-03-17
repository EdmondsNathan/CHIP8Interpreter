# CHIP-8 Interpreter

A CHIP-8 interpreter written in C# using the MonoGame framework. This project implements a fully functional emulation of the CHIP-8 virtual machine, capable of running classic CHIP-8 programs with accurate instruction execution, display rendering, input handling, and sound.

## Overview

CHIP-8 is an interpreted programming language developed in the mid-1970s for the COSMAC VIP and Telmac 1800 microcomputers. It was designed to simplify game development on early 8-bit systems and has since become a popular starting point for learning about emulator and interpreter development.

This interpreter faithfully emulates the CHIP-8 architecture, including its 4 KB memory space, 16 general-purpose registers, 64x32 monochrome display, and 16-key hexadecimal keypad. It supports both legacy and modern CHIP-8 instruction sets, allowing compatibility with a wide range of ROMs.

## Features

- **Full instruction set support** -- all 35 standard CHIP-8 opcodes implemented
- **Dual compatibility modes** -- legacy (COSMAC VIP) and modern instruction behavior for shift, jump, and memory operations
- **64x32 monochrome display** -- rendered at 10x scale (640x320) via MonoGame
- **16-key input mapping** -- configurable keypad layouts (COSMAC and ordered)
- **Delay and sound timers** -- decremented at 60 Hz on a dedicated thread
- **Sound playback** -- timer-driven audio using MonoGame's sound system
- **Multi-threaded architecture** -- separate threads for CPU execution, rendering, and timer management
- **Configurable clock speed** -- default 1000 Hz, adjustable at initialization

## Technologies Used

| Technology | Purpose |
|---|---|
| **C#** | Primary language |
| **.NET 8.0** | Runtime and build target |
| **MonoGame 3.8 (DesktopGL)** | Cross-platform rendering, input, and audio |

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later

### Build and Run

Clone the repository and build the project:

```bash
git clone https://github.com/EdmondsNathan/CHIP8Interpreter.git
cd CHIP8Interpreter
dotnet build
```

Run the interpreter with a ROM file:

```bash
dotnet run --project CHIP8Interpreter -- <path-to-rom> [legacy|modern]
```

The second argument selects the compatibility mode. If omitted, it defaults to `legacy`.

- **legacy** -- COSMAC VIP behavior for shift, jump, and memory operations
- **modern** -- modern interpreter behavior for these instructions

### Examples

```bash
# Run in legacy mode (default)
dotnet run --project CHIP8Interpreter -- CHIP8Interpreter/ROMs/Pong\ \(1\ player\).ch8

# Run in modern mode
dotnet run --project CHIP8Interpreter -- CHIP8Interpreter/ROMs/Pong\ \(1\ player\).ch8 modern
```

Several test and game ROMs are included in the `CHIP8Interpreter/ROMs/` directory.

## Usage

### Controls

The CHIP-8 hexadecimal keypad is mapped to the following keyboard layout:

```
CHIP-8 Keypad        Keyboard Mapping
+---+---+---+---+    +---+---+---+---+
| 1 | 2 | 3 | C |    | 1 | 2 | 3 | 4 |
+---+---+---+---+    +---+---+---+---+
| 4 | 5 | 6 | D |    | Q | W | E | R |
+---+---+---+---+    +---+---+---+---+
| 7 | 8 | 9 | E |    | A | S | D | F |
+---+---+---+---+    +---+---+---+---+
| A | 0 | B | F |    | Z | X | C | V |
+---+---+---+---+    +---+---+---+---+
```

### Included ROMs

| ROM | Description |
|---|---|
| `1-chip8-logo.ch8` through `7-beep.ch8` | Test suite ROMs for verifying interpreter correctness |
| `Airplane.ch8` | Classic CHIP-8 game |
| `Lunar Lander (Udo Pernisz, 1979).ch8` | Lunar landing simulation |
| `Pong (1 player).ch8` | Single-player Pong |

## Project Structure

```
CHIP8Interpreter/
├── Program.cs                 # Entry point; initializes threads for CPU, rendering, and timers
├── Game1.cs                   # MonoGame integration for display, input, and sound
├── Emulator/
│   ├── Chip8.cs               # CPU state: memory, registers, display buffer, timers
│   └── Interpreter.cs         # Fetch-decode-execute cycle and opcode implementation
├── Content/
│   └── C Note.wav             # Sound effect for the CHIP-8 beep timer
└── ROMs/                      # Included ROM files for testing and gameplay
```

## Implementation Details

### Memory Model

The interpreter emulates 4 KB (4096 bytes) of addressable memory:

- **0x000 - 0x04F** -- Reserved
- **0x050 - 0x09F** -- Built-in font data (5 bytes per hexadecimal character, 0-F)
- **0x200 - 0xFFF** -- Program space (ROMs are loaded starting at 0x200)

### CPU Registers

- **V0-VF** -- 16 general-purpose 8-bit registers (VF doubles as a flag register)
- **I** -- 16-bit index register for memory addressing
- **PC** -- 16-bit program counter, initialized to 0x200
- **Stack** -- 16-level subroutine call stack

### Execution Cycle

Each CPU cycle follows the standard fetch-decode-execute pattern:

1. **Fetch** -- Read a 2-byte instruction from memory at the current program counter
2. **Decode** -- Parse the instruction into its component fields (opcode category, register indices, immediate values)
3. **Execute** -- Dispatch to the appropriate handler based on the opcode

The CPU runs at a configurable clock speed (default: 1000 Hz) on a dedicated thread, independent of the 60 Hz timer thread and the MonoGame rendering loop.

### Display

The 64x32 display is stored as an array of 32 `UInt64` values, where each bit represents a single pixel. Sprites are drawn using XOR logic, with the VF register set on pixel collision. MonoGame renders the display at 10x scale.

### Input

Keyboard state is polled each frame via MonoGame. Each of the 16 keys maps to a bit in a 16-bit input register. The interpreter supports both key-press detection and blocking key-release detection (opcode FX0A).

## Future Improvements

- Step-through debugger with register and memory inspection
- Configurable display colors and scaling factor
- On-screen ROM selection menu
- SUPER-CHIP (SCHIP) and XO-CHIP extension support
- Configurable key bindings via external configuration
- Performance profiling and cycle-accurate timing

## Acknowledgments

- [Tobias V. Langhoff's CHIP-8 guide](https://tobiasvl.github.io/blog/write-a-chip-8-emulator) -- primary reference for implementation
- [Timendus's CHIP-8 test suite](https://github.com/Timendus/chip8-test-suite) -- test ROMs for verification
- Author's [blog post](https://edmondsnathan.github.io/2025/08/16/What-I-Learned-CHIP-8-Interpreter.html) detailing the implementation process

## License

This project is dedicated to the public domain under the [CC0 license](https://creativecommons.org/publicdomain/zero/1.0/). ROM files included in the `ROMs/` directory have their own licensing terms, detailed in [Licenses/LICENSE.txt](Licenses/LICENSE.txt).
