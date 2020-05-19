using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGamePico8.backend;
using Pico8Emulator;
using System;
using System.Diagnostics;

namespace MonoGamePico8 {
	public class Pico8 : Game {
		private const float UpdateTime30 = 1 / 30f;
		private const float UpdateTime60 = 1 / 60f;
		
		private Emulator _emulator;
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _batch;
		private FrameCounter _counter;
		private MonoGameGraphicsBackend _graphicsBackend;
		private float _deltaUpdate30, _deltaUpdate60, _deltaDraw;
		
		public Pico8() {
			_graphics = new GraphicsDeviceManager(this);
			_counter = new FrameCounter();

			IsFixedTimeStep = false;
		}

		protected override void Initialize() {
			base.Initialize();
			
			_batch = new SpriteBatch(GraphicsDevice);
			
			_graphics.PreferredBackBufferWidth = 512;
			_graphics.PreferredBackBufferHeight = 512;
			_graphics.ApplyChanges();
		}

		protected override void LoadContent() {
			base.LoadContent();
			
			_graphicsBackend = new MonoGameGraphicsBackend(GraphicsDevice);
			_emulator = new Emulator(_graphicsBackend, new MonoGameAudioBackend(), new MonoGameInputBackend());

			if (!_emulator.CartridgeLoader.Load("testcarts/jelpi.p8")) {
				Exit();
			}
		}

		protected override void Update(GameTime gameTime) {
			base.Update(gameTime);
			var dt = (float) gameTime.ElapsedGameTime.TotalSeconds;

			_deltaUpdate30 += dt;

			while (_deltaUpdate30 >= UpdateTime30) {
				_deltaUpdate30 -= UpdateTime30;
				_emulator.Update30();
			}
			
			_deltaUpdate60 += dt;

			while (_deltaUpdate60 >= UpdateTime60) {
				_deltaUpdate60 -= UpdateTime60;
				_emulator.Update60();
			}
			
			_counter.Update(dt);
			Window.Title = $"{_counter.AverageFramesPerSecond} fps {_emulator.Graphics.drawCalls} calls";

			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
					Keyboard.GetState().IsKeyDown(Keys.Escape)) {
				Exit();
			}
		}

		protected override void Draw(GameTime gameTime) {
			base.Draw(gameTime);

			var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
			var u = _emulator.CartridgeLoader.HighFps ? UpdateTime60 : UpdateTime30;
			
			_deltaDraw += dt;

			while (_deltaDraw >= u) {
				_deltaDraw -= u;
				_emulator.Graphics.drawCalls = 0;
				_emulator.Draw();
			}

			_emulator.Graphics.Flip();

			GraphicsDevice.Clear(Color.Black);
			
			_batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
			_batch.Draw(_graphicsBackend.Surface, new Rectangle(0, 0, 512, 512), Color.White);
			_batch.End();
		}
	}
}