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
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			_spriteBatch.Begin();
			_spriteBatch.Draw(_pixel, new Rectangle(0, 0, Chip8.DisplayWidth * 10, Chip8.DisplayHeight * 10), Color.Black);
			DrawDisplay();
			_spriteBatch.End();

			base.Draw(gameTime);
		}

		private void DrawDisplay()
		{
			for (int y = 0; y < Chip8.DisplayHeight; y++)
			{
				UInt64 row = Program.MyChip8.Display[y];

				for (int x = 0; x < Chip8.DisplayWidth; x++)
				{
					if (((row >> Chip8.DisplayWidth - x) & 1) == 1)
					{
						_spriteBatch.Draw(_pixel, new Rectangle(x * 10, y * 10, 10, 10), Color.Red);
					}
				}
			}
		}
	}
}
