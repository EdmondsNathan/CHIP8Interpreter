using CHIP8Interpreter.Emulator;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
		private KeyboardState _keyboardState;
		private Keys[] _keypad =
		{
			Keys.D1, Keys.D2, Keys.D3, Keys.D4,
			Keys.Q, Keys.W, Keys.E, Keys.R,
			Keys.A, Keys.S, Keys.D, Keys.F,
			Keys.Z, Keys.X, Keys.C, Keys.V
		};
		private int[] _keyLayout;
		private SoundEffectInstance _soundEffectInstance;
		private SoundEffect _soundEffect;

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

			SetKeypadLayout();

			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			_pixel = new Texture2D(GraphicsDevice, 1, 1);
			_pixel.SetData(new Color[] { Color.White });

			_soundEffect = Content.Load<SoundEffect>("C Note");
			_soundEffectInstance = _soundEffect.CreateInstance();
			_soundEffectInstance.IsLooped = true;
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			UpdateInput(_keyboardState);

			PlaySound();

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

		private void SetKeypadLayout()
		{
			if (Program.MyInterpreter.KeypadLayout == KeypadLayout.Ordered)
			{
				_keyLayout = new[]
				{
					0, 1, 2, 3,
					4, 5, 6, 7,
					8, 9, 0xA, 0xB,
					0xC, 0xD, 0xE, 0xF
				};
			}
			else if (Program.MyInterpreter.KeypadLayout == KeypadLayout.Cosmac)
			{
				_keyLayout = new[]
				{
					1, 2, 3, 0xC,
					4, 5, 6, 0xD,
					7, 8, 9, 0xE,
					0xA, 0, 0xB, 0xF
				};
			}
		}

		private void UpdateInput(KeyboardState keyboardState)
		{
			_keyboardState = Keyboard.GetState();
			UInt16 keyboardByte = 0;

			for (int i = 0; i < 16; i++)
			{
				int result = _keyboardState.IsKeyDown(_keypad[i]) ? 1 : 0;
				keyboardByte += (UInt16)(result << _keyLayout[i]);
			}

			Program.MyChip8.InputRegister = keyboardByte;
		}
		private void PlaySound()
		{
			if (Program.MyChip8.SoundTimer > 0)
			{
				_soundEffectInstance.Play();
			}
			else
			{
				_soundEffectInstance.Stop();
			}
		}
	}

}
