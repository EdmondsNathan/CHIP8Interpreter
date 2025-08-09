using CHIP8Interpreter.Emulator;
using System;
using System.Diagnostics;
using System.Threading;

class Program
{
	public static Chip8 MyChip8 = new Chip8("ROMs/6-keypad.ch8");
	public static Interpreter MyInterpreter = new Interpreter(MyChip8, CompatibilityMode.Modern, 2000, KeypadLayout.Cosmac);

	static void Main(string[] args)
	{
		Thread gameThread = new Thread(() => StartGame());
		gameThread.Start();

		Thread chip8Thread = new Thread(() => StartChip8(gameThread));
		chip8Thread.Start();

		Thread timerThread = new Thread(() => StartTimers(gameThread));
		timerThread.Start();
	}

	private static void StartGame()
	{
		using var game = new CHIP8Interpreter.Game1();
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
			MyInterpreter.Execute(MyInterpreter.Fetch());

			stopwatch.Stop();
			timeSpan = stopwatch.Elapsed;

			Thread.Sleep(TimeSpan.FromMilliseconds(1000f / (MyInterpreter.ClockSpeedHz - timeSpan.Milliseconds)));
			deltaTime.Stop();
		}
	}

	private static void StartTimers(Thread gameThread)
	{
		while (gameThread.IsAlive)
		{
			if (MyChip8.DelayTimer > 0)
			{
				MyChip8.DelayTimer--;
			}

			if (MyChip8.SoundTimer > 0)
			{
				MyChip8.SoundTimer--;
			}

			Thread.Sleep(TimeSpan.FromMilliseconds(1000f / 60f));
		}
	}
}
