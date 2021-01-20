﻿using System;
using Lens.assets;
using Lens.game;
using Lens.graphics;
using Lens.graphics.gamerenderer;
using Lens.input;
using Lens.util;
using Lens.util.file;
using Lens.util.timer;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Runtime.InteropServices;

namespace Lens {
	public class Engine : Game {
#if DEBUG
		public const bool Debug = true;
#else
		public const bool Debug = false;
#endif

		public static Action AssetsLoaded;
		
		public static bool PixelPerfect;
		public static bool EditingLevel;
		public static Version Version;
		public static Engine Instance;
		public static GraphicsDeviceManager Graphics;
		public new static GraphicsDevice GraphicsDevice;
		public static Matrix ScreenMatrix;
		public static Matrix UiMatrix;
		public static float Time;
		public static float Delta;
		public static GameTime GameTime;
		public static bool Quiting;
		public static int UpdateTime;
		public static int RenderTime;
		public static bool Flashes = true;
		
		public FrameCounter Counter;
		public GameRenderer StateRenderer;
		public GameState State { get; private set; }
		public static Vector2 Viewport;
		public float Upscale;
		public float UiUpscale;

		private static Core.Core core;
		private const float FixedUpdateTime = 0.015f;
		private float time;
		public float Speed = 1;
		public float Flash;
		public float Freeze;
		public static float FlashModifier = 1;
		public static float FreezeModifier = 1;
		
		public bool Focused;
		public Color FlashColor = ColorUtils.WhiteColor;
		public float Split;
		// Window.Title works only in Init, sadly
		private string tmpTitle;
		private GameState newState;
		
		public string Title {
			get => Window.Title;
			set {
				if (value != null && !RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
					try {
						Window.Title = value;
					} catch (Exception e) {
						Log.Error(e);
					}
				}
			}
		}

		public Engine(Version version, GameState state, string title, int width, int height, bool fullscreen) {
			Instance = this;
			tmpTitle = title;
			Version = version;
			IsFixedTimeStep = false;
			
			Graphics = new GraphicsDeviceManager(this);
			// Graphics.GraphicsProfile = GraphicsProfile.HiDef;
			
			Graphics.PreferMultiSampling = false;
			Graphics.GraphicsProfile = GraphicsProfile.Reach;
			
			Graphics.HardwareModeSwitch = false;

			core = Core.Core.SelectCore(Window, Graphics);
			core.Init(width, height, fullscreen);

			Content.RootDirectory = "Content/";
			Assets.Content = Content;
			newState = state;
			Counter = new FrameCounter();
		}

		protected override void OnActivated(object sender, EventArgs args) {
			base.OnActivated(sender, args);
			Focused = true;
			State?.OnActivated();
		}

		protected override void OnDeactivated(object sender, EventArgs args) {
			base.OnDeactivated(sender, args);
			Focused = false;
			State?.OnDeactivated();
		}

		protected override void LoadContent() {
			GraphicsDevice = base.GraphicsDevice;
			GraphicsDevice.BlendState = BlendState.NonPremultiplied;
			Lens.graphics.Graphics.Init();
		}
		
		protected override void UnloadContent() {
			Quiting = true;
			State?.Destroy();
			StateRenderer?.Destroy();

			Destroy();
			
			Assets.Destroy();
			Input.Destroy();

			Instance = null;
			Log.Info("Bye");
			Log.Close();
		}

		protected virtual void Destroy() {
			
		}
		
		protected override void Initialize() {
			base.Initialize();

			if (StateRenderer == null) {
				StateRenderer = new PixelPerfectGameRenderer();
			}
			
			UpdateView();

			Log.Open();
			Log.Info($"{tmpTitle} v{Version}");
			Log.Info($"Starting from {AppDomain.CurrentDomain.BaseDirectory}");
			Log.Info(tmpTitle);
			Log.Info(DateTime.Now.ToString("dd.MM.yyyy h:mm tt"));

			Title = tmpTitle;

			Input.Init();
		}

		protected override void Update(GameTime gameTime) {
			var t = DateTime.Now.Millisecond;
			
			base.Update(gameTime);
			Audio.UpdateAudio();

			GameTime = gameTime;
			var dt = (float) gameTime.ElapsedGameTime.TotalSeconds * Speed;

			Delta = dt;
			Time += dt;

			Audio.Update(dt);

			Split = Math.Max(0, Split - dt);

			if (Flash > 0) {
				Flash -= dt * 240f;
			}
			
			// Freeze = Math.Max(0, Math.Min(1, Freeze) - dt * 60f * (1.1f - FreezeModifier));

			// if (FreezeModifier <= 0.01f || Freeze < 0.01f) {
				time += dt;
			// }

			MouseData.HadClick = false;

			// Make sure we don't have to process more than ~15 frames in 1 frame
			time = Math.Min(time, 0.25f);

			while (time >= FixedUpdateTime) {
				time -= FixedUpdateTime;
	
				if (newState != null) {
					Log.Info("Setting state to " + newState.GetType().Name);

					Speed = 1;
					
					State?.Destroy();
					State = newState;
					Input.EnableImGuiFocus = true;
					State?.Init();

					newState = null;
					UpdateView();
				}

				Assets.Update(FixedUpdateTime);
				Input.Update(FixedUpdateTime);
				Timer.Update(FixedUpdateTime);
				Tween.Update(FixedUpdateTime);

				State?.Update(FixedUpdateTime);
			}
			
			Counter.Update(dt);
			UpdateTime = DateTime.Now.Millisecond - t;
		}

		public virtual void RenderUi() {
			
		}

		public void SetState(GameState state) {
			if (State == null) {
				State = state;
				State.Init();
			} else {
				newState = state;				
			}
		}
		
		public void Quit() {
			Exit();
		}
		
		protected override void Draw(GameTime gameTime) {
			var t = DateTime.Now.Millisecond;
			StateRenderer.Render();
			base.Draw(gameTime);
			RenderTime = DateTime.Now.Millisecond - t;
		}

		public void SetWindowed(int width, int height) {
			core.SetWindowed(width, height);
			UpdateView();
		}

		public void SetFullscreen() {
			
			core.SetFullscreen();
			UpdateView();
		}

		public float GetScreenWidth() {
			if (Graphics.IsFullScreen) {
				return GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
			}

			return Math.Max(Display.Width, base.GraphicsDevice.PresentationParameters.BackBufferWidth);
		}

		public float GetScreenHeight() {
			if (Graphics.IsFullScreen) {
				return GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
			}

			return Math.Max(Display.Height, base.GraphicsDevice.PresentationParameters.BackBufferHeight);
		}
		
		public void UpdateView() {
			var screenWidth = GetScreenWidth();
			var screenHeight = GetScreenHeight();

			Graphics.PreferredBackBufferWidth = (int) screenWidth;
			Graphics.PreferredBackBufferHeight = (int) screenHeight;
			Graphics.ApplyChanges();

			if (State?.NativeRender() ?? false) {
				Upscale = 1;
				UiUpscale = 1;
				Viewport.X = 0;
				Viewport.Y = 0;
				
				ScreenMatrix = Matrix.Identity;
				UiMatrix = Matrix.Identity;
			} else {
				Upscale = Math.Min(screenWidth / Display.Width, screenHeight / Display.Height);

				if (PixelPerfect) {
					Upscale = (int) Math.Floor(Upscale);
				}
				
				Viewport.X = (screenWidth - Upscale * Display.Width) / 2;
				Viewport.Y = (screenHeight - Upscale * Display.Height) / 2;	
				
				if (PixelPerfect) {
					screenWidth = (int) Math.Floor(screenWidth / Display.Width) * Display.Width;
					screenHeight = (int) Math.Floor(screenHeight / Display.Height) * Display.Height;
				}
				
				UiUpscale = Math.Min(screenWidth / Display.UiWidth, screenHeight / Display.UiHeight);

				float viewWidth;
			
				if (screenWidth / Display.Width > screenHeight / Display.Height) {
					viewWidth = screenHeight / Display.Height * Display.Width;
				} else {
					viewWidth = screenWidth;
				}
			
				ScreenMatrix = Matrix.CreateScale(Upscale) * Matrix.CreateTranslation(Viewport.X, Viewport.Y, 0);
				UiMatrix = Matrix.CreateScale(viewWidth / Display.UiWidth) * Matrix.CreateTranslation(Viewport.X, Viewport.Y, 0);
			}
			
			StateRenderer?.Resize((int) screenWidth, (int) screenHeight);
		}
	}
}