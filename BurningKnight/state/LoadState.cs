using System;
using System.Threading;
using BurningKnight.assets;
using BurningKnight.assets.lighting;
using BurningKnight.level.tile;
using BurningKnight.physics;
using BurningKnight.save;
using Lens;
using Lens.entity;
using Lens.game;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.state {
	public class LoadState : GameState {
		public string Path;
		private Area gameArea;
		private int ready;
		private bool down;
		private float alpha;
		
		public override void Init() {
			base.Init();
			
			Lights.Init();
			Physics.Init();
			gameArea = new Area();

			Run.Level = null;
			
			Tilesets.Load();
			
			if (Run.Id == -1) {
				ready = -1;
				SaveManager.ThreadLoad(ReadyCallback, gameArea, SaveType.Game, Path);
			}

			SaveManager.ThreadLoad(ReadyCallback, gameArea, SaveType.Level, Path);
			SaveManager.ThreadLoad(ReadyCallback, gameArea, SaveType.Player, Path);
		}

		private void ReadyCallback() {
			ready++;
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (down) {
				if (ready >= 2 && ((Engine.Version.Debug) || Time > 3f)) {
					alpha -= dt * 5;
				}
			} else {
				alpha = Math.Min(1, alpha + dt * 5);

				if (alpha >= 0.95f) {
					alpha = 1;
					down = true;
				}
			}

			if (ready >= 2 && ((down && alpha < 0.05f) || (Engine.Version.Debug))) {
				Engine.Instance.SetState(new InGameState(gameArea));
			}
		}

		public override void RenderUi() {
			base.RenderUi();
			
			Graphics.Color = new Color(1f, 1f, 1f, alpha);
			Graphics.Print($"Coming soon tm {Math.Min(102, Math.Floor(Time / 3f * 100f))}%", Font.Medium, new Vector2(4, 4));
			Graphics.Color = ColorUtils.WhiteColor;
		}
	}
}