using CHIP8Interpreter.Emulator;
using System;
using System.Threading;

class Program
{
	public static Chip8 MyChip8 = new Chip8("ROMs/4-flags.ch8");
	public static Interpreter MyInterpreter = new Interpreter(MyChip8);

	static void Main(string[] args)
	{
		Thread gameThread = new Thread(() => StartGame());
		gameThread.Start();

		Thread chip8Thread = new Thread(() => StartChip8(gameThread));
		chip8Thread.Start();
	}

	private static void StartGame()
	{
		using var game = new CHIP8Interpreter.Game1();
		game.Run();
	}

	private static void StartChip8(Thread gameThread)
	{
		while (gameThread.IsAlive)
		{
			MyInterpreter.Execute(MyInterpreter.Fetch());
		}
	}
}
