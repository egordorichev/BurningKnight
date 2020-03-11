using System;
using System.IO;
using System.Threading;
using BurningKnight.assets;
using BurningKnight.assets.input;
using BurningKnight.assets.lighting;
using BurningKnight.level.biome;
using BurningKnight.level.tile;
using BurningKnight.physics;
using BurningKnight.save;
using BurningKnight.ui.imgui;
using BurningKnight.util;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.game;
using Lens.graphics;
using Lens.graphics.animation;
using Lens.util;
using Lens.util.math;
using Microsoft.Xna.Framework;
using Console = BurningKnight.debug.Console;

namespace BurningKnight.state {
	public class LoadState : GameState {
		public string Path;
		private Area gameArea;
		private bool ready;
		private bool down;
		private float alpha;
		private string title;
		private string prefix;
		private float titleX;
		private float prefixX;
		private float t;
		private int progress;
		private float timer;
		private bool loading;
		private bool nice;
		
		public bool Menu;

		private Animation animation;
		
		public override void Init() {
			base.Init();

			animation = Animations.Create("loading");
			animation.Paused = false;

			nice = Rnd.Chance(5);
			
			if (SaveManager.ExistsAndValid(SaveType.Game)
			    && SaveManager.ExistsAndValid(SaveType.Level)
			    && SaveManager.ExistsAndValid(SaveType.Player)) {

				loading = true;
			}

			prefix = Locale.Get(loading || Run.Depth < 1 ? "loading" : "generating");
			title = new Random().NextDouble() > 0.3 ? LoadScreenTitles.Generate() : BiomeTitles.Generate(BiomeRegistry.GenerateForDepth(Run.Depth).Id);
			
			Lights.Init();
			Physics.Init();
			gameArea = new Area();

			Run.Level = null;
			progress = 0;

			var thread = new Thread(() => {
				Tilesets.Load();
				
				SaveManager.Load(gameArea, SaveType.Game, Path);
				progress++;
				
				Rnd.Seed = $"{Run.Seed}_{Run.Depth}"; 
				
				SaveManager.Load(gameArea, SaveType.Level, Path);
				progress++;

				Run.Luck = 0;
				Run.ResetScourge();
				
				if (Run.Depth > 0) {
					SaveManager.Load(gameArea, SaveType.Player, Path);
				} else {
					SaveManager.Generate(gameArea, SaveType.Player);
				}

				GC.Collect();
				progress++;
				ready = true;
			});

			thread.Priority = ThreadPriority.Lowest;
			thread.Start();

			titleX = Font.Small.MeasureString(title).Width * -0.5f;
		}

		public override void Update(float dt) {
			base.Update(dt);

			animation.Update(dt);

			t += dt;
			
			timer += dt / 3;
			timer = Math.Min(timer, (progress + 1) * 0.345f);
			
			if (down) {
				if (ready && ((Engine.Version.Dev || loading) || timer >= 1f)) {
					timer = 1;
					alpha -= dt * 5;
				}
			} else {
				alpha = Math.Min(1, alpha + dt * 5);

				if (alpha >= 0.95f) {
					alpha = 1;
					down = true;
				}
			}

			if (ready && ((down && alpha < 0.05f) || (Engine.Version.Dev))) {
				Engine.Instance.SetState(new InGameState(gameArea, Menu));
				Menu = false;
			}
		}

		public override void RenderUi() {
			base.RenderUi();

			var value = (int) Math.Min(102, Math.Floor(timer * 100f));
			var s = $"{prefix} {(nice && value == 69 ? "Nice." : $"{value}")}%";
			
			prefixX = Font.Medium.MeasureString(s).Width * -0.5f;
			
			Graphics.Color = new Color(1f, 1f, 1f, alpha);
			Graphics.Print(s, Font.Medium, new Vector2(Display.UiWidth / 2f + prefixX, Display.UiHeight / 2f - 8));
			Graphics.Print(title, Font.Small, new Vector2(Display.UiWidth / 2f + titleX, Display.UiHeight / 2f + 8));

			animation.Render( new Vector2(Display.UiWidth / 2f - 5.5f, Display.UiHeight / 2f + 24));
			Graphics.Color = ColorUtils.WhiteColor;
		}

		public override void RenderNative() {
			ImGuiHelper.Begin();
		
			if (Console.Open) {
				DebugWindow.Render();
			}

			ImGuiHelper.End();
		}
	}
}