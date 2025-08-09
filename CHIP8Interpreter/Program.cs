class Program
{
	static void Main(string[] args)
	{
		StartGame();
	}

	private static void StartGame()
	{
		using var game = new CHIP8Interpreter.Game1();
		game.Run();
	}
}
