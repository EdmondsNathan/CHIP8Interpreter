using CHIP8Interpreter.Emulator;
using System;
using System.Diagnostics;
using System.Threading;

class Program
{
	private static Chip8 _chip8;
	private static Interpreter _interpreter;

	static void Main(string[] args)
	{
		if (args.Length < 1)
		{
			Console.WriteLine("Usage: CHIP8Interpreter <path-to-rom> [legacy|modern]");
			return;
		}

		CompatibilityMode mode = CompatibilityMode.Legacy;
		if (args.Length >= 2)
		{
			switch (args[1].ToLowerInvariant())
			{
				case "legacy":
					mode = CompatibilityMode.Legacy;
					break;
				case "modern":
					mode = CompatibilityMode.Modern;
					break;
				default:
					Console.WriteLine($"Invalid mode '{args[1]}'. Expected 'legacy' or 'modern'.");
					return;
			}
		}

		_chip8 = new Chip8(args[0]);
		_interpreter = new Interpreter(_chip8, mode, 1000, KeypadLayout.Cosmac);

		Thread gameThread = new Thread(() => StartGame());
		gameThread.Start();

		Thread chip8Thread = new Thread(() => StartChip8(gameThread));
		chip8Thread.Start();

		Thread timerThread = new Thread(() => StartTimers(gameThread));
		timerThread.Start();
	}

	private static void StartGame()
	{
		using var game = new CHIP8Interpreter.Game1(_interpreter, _chip8);
		game.Run();
	}

	private static void StartChip8(Thread gameThread)
	{
		Stopwatch stopwatch = new();
		TimeSpan timeSpan = new();

		while (gameThread.IsAlive)
		{
			Stopwatch deltaTime = new Stopwatch();
			deltaTime.Restart();
			stopwatch.Restart();
			_interpreter.Execute(_interpreter.Fetch());

			stopwatch.Stop();
			timeSpan = stopwatch.Elapsed;

			Thread.Sleep(TimeSpan.FromMilliseconds(1000f / (_interpreter.ClockSpeedHz - timeSpan.Milliseconds)));
			deltaTime.Stop();
		}
	}

	private static void StartTimers(Thread gameThread)
	{
		while (gameThread.IsAlive)
		{
			if (_chip8.DelayTimer > 0)
			{
				_chip8.DelayTimer--;
			}

			if (_chip8.SoundTimer > 0)
			{
				_chip8.SoundTimer--;
			}

			Thread.Sleep(TimeSpan.FromMilliseconds(1000f / 60f));
		}
	}
}
