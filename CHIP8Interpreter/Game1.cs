using CHIP8Interpreter.Emulator;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace CHIP8Interpreter
{
	public class Game1 : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		Texture2D _pixel;
		protected override void Initialize()
		{
			// TODO: Add your initialization logic here

			TestCodeInit();

			_graphics.IsFullScreen = false;
			_graphics.PreferredBackBufferWidth = Chip8.DisplayWidth * 10;
			_graphics.PreferredBackBufferHeight = Chip8.DisplayHeight * 10;
			_graphics.ApplyChanges();

			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			_pixel = new Texture2D(GraphicsDevice, 1, 1);
			_pixel.SetData(new Color[] { Color.White });

			// TODO: use this.Content to load your game content here
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			// TODO: Add your update logic here

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			// TODO: Add your drawing code here
			Debug.WriteLine("Cycle at PC " + _chip8.ProgramCounter);
			_interpreter.Execute(_interpreter.Fetch());

			_spriteBatch.Begin();
			DrawDisplay();
			//_spriteBatch.Draw(_pixel, new Rectangle(0, 0, 10, 10), Color.White);
			_spriteBatch.End();

			base.Draw(gameTime);
		}

		private void DrawDisplay()
		{
			for (int y = 0; y < Chip8.DisplayHeight; y++)
			{
				UInt64 row = _chip8.Display[y];

				//String output = "";

				for (int x = 0; x < Chip8.DisplayWidth; x++)
				{
					if (((row >> Chip8.DisplayWidth - x) & 1) == 1)
					{
						_spriteBatch.Draw(_pixel, new Rectangle(x * 10, y * 10, 10, 10), Color.White);

						//output += "1 ";
					}
					/*else
					{
						output += "0 ";
					}*/
				}
				//Debug.WriteLine(output);
			}
		}

		private Chip8 _chip8;
		private Interpreter _interpreter;
		private void TestCodeInit()
		{
			_chip8 = new Chip8("ROMs/1-chip8-logo.ch8");
			_interpreter = new Interpreter(_chip8);

			/*for (int i = Chip8.RomStartingAddress; i < _chip8.RAM.Length; i++)
			{
				Debug.WriteLine("Address " + i + $": {_chip8.RAM[i]:X}");
			}*/

			/*Instruction instruction = new Instruction(0x1234);
			Debug.WriteLine(instruction.HighNibble);
			Debug.WriteLine(instruction.X);
			Debug.WriteLine(instruction.Y);
			Debug.WriteLine(instruction.N);
			Debug.WriteLine(instruction.NN);
			Debug.WriteLine(instruction.NNN);*/
		}
	}
}
