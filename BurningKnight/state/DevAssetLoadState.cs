using System;
using System.Threading;
using BurningKnight.assets;
using BurningKnight.assets.achievements;
using BurningKnight.assets.items;
using BurningKnight.assets.lighting;
using BurningKnight.assets.loot;
using BurningKnight.assets.mod;
using BurningKnight.assets.prefabs;
using BurningKnight.level.tile;
using BurningKnight.physics;
using BurningKnight.save;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.game;
using Lens.graphics;
using Lens.util;
using Lens.util.math;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BurningKnight.state {
	public class DevAssetLoadState : GameState {
		private const bool LoadEditor = false;
		private const bool LoadCutscene = true;
		
		private int progress;
		private bool ready;
		private bool checkFullscreen;
		private Area gameArea;
		private float t;
		
		public override void Init() {
			base.Init();
			
			progress = 0;

			var thread = new Thread(Load);

			thread.Priority = ThreadPriority.Highest;
			thread.Start();
		}
		
		private void Load() {
			Log.Info("Starting asset loading thread");

			SaveManager.Load(gameArea, SaveType.Global);
			checkFullscreen = true;
			progress++;
			
			var t = DateTime.Now.Millisecond;
			var c = DateTime.Now.Millisecond;

			Assets.Load(ref progress);
			Log.Info($"Assets took {(DateTime.Now.Millisecond - c) / 1000f} seconds");
			c = DateTime.Now.Millisecond;

			Dialogs.Load();
			progress++;
			CommonAse.Load();
			progress++;
			ImGuiHelper.BindTextures();
			progress++;
			Shaders.Load();
			progress++;
			Prefabs.Load();
			progress++;
			Items.Load();
			progress++;
			LootTables.Load();
			progress++;
			Mods.Load();
			progress++; // Should be 13 here
			Log.Info($"Custom assets took {(DateTime.Now.Millisecond - c) / 1000f} seconds");
				
			Log.Info("Done loading assets! Loading level now.");
			
			Lights.Init();
			Physics.Init();
			gameArea = new Area();

			Run.Level = null;
			Tilesets.Load();
			progress++;
				
			Achievements.Load();
			c = DateTime.Now.Millisecond;

			if (!LoadEditor) {
				SaveManager.Load(gameArea, SaveType.Game);
				progress++;
				Log.Info($"Game took {(DateTime.Now.Millisecond - c) / 1000f} seconds");
				c = DateTime.Now.Millisecond;

				Rnd.Seed = $"{Run.Seed}_{Run.Depth}";

				SaveManager.Load(gameArea, SaveType.Level);
				progress++;
				Log.Info($"Level took {(DateTime.Now.Millisecond - c) / 1000f} seconds");
				c = DateTime.Now.Millisecond;

				if (Run.Depth > 0) {
					SaveManager.Load(gameArea, SaveType.Player);
				} else {
					SaveManager.Generate(gameArea, SaveType.Player);
				}

				Log.Info($"Player took {(DateTime.Now.Millisecond - c) / 1000f}");
			}

			progress++; // Should be 18 here
			Log.Info($"Done loading level! ({(DateTime.Now.Millisecond - t) / 1000f} seconds) Going to menu.");
				
			ready = true;
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (checkFullscreen) {
				checkFullscreen = false;
				
				if (Settings.Fullscreen) {
					Engine.Instance.SetFullscreen();
				} else {
					Engine.Instance.SetWindowed(Display.Width * 3, Display.Height * 3);
				}
			}

			t += dt;
			
			if (ready) {
				if (Settings.Fullscreen) {
					Engine.Instance.SetFullscreen();
				} else {
					Engine.Instance.SetWindowed(Display.Width * 3, Display.Height * 3);
				}

				Engine.Instance.StateRenderer.UiEffect = Shaders.Ui;
				Engine.Instance.SetState(LoadEditor ? (GameState) new EditorState() : (LoadCutscene ? (GameState) new LoadState() : new InGameState(gameArea, false)));
			}
		}

		public override void RenderUi() {
			base.RenderUi();
			Graphics.Print($"Loading assets {progress / 18f * 100}%", Font.Small, new Vector2(10, 10));
			
			var n = t % 2f;
			var s = $"{(n > 0.5f ? "." : "")}{(n > 1f ? "." : "")}{(n > 1.5f ? "." : "")}";
			
			var x = Font.Small.MeasureString(s).Width * -0.5f;
			Graphics.Print(s, Font.Small, new Vector2(Display.UiWidth / 2f + x, Display.UiHeight / 2f + 32));
		}
	}
}