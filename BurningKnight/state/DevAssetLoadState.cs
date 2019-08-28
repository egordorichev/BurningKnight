using System.Threading;
using BurningKnight.assets;
using BurningKnight.assets.items;
using BurningKnight.assets.lighting;
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

namespace BurningKnight.state {
	public class DevAssetLoadState : GameState {
		private int progress;
		private bool ready;
		private Area gameArea;
		
		public override void Init() {
			base.Init();
			
			progress = 0;

			var thread = new Thread(Load);

			thread.Priority = ThreadPriority.Highest;
			thread.Start();
		}
		
		private void Load() {
			Log.Info("Starting asset loading thread");
				
			Assets.Load(ref progress);
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
			Mods.Load();
			progress++; // Should be 12 here
				
			Log.Info("Done loading assets! Loading level now.");
			
			Lights.Init();
			Physics.Init();
			gameArea = new Area();

			Run.Level = null;
			Tilesets.Load();
			progress++;
				
			SaveManager.Load(gameArea, SaveType.Game);
			progress++;
				
			Random.Seed = $"{Run.Seed}_{Run.Depth}"; 
				
			SaveManager.Load(gameArea, SaveType.Level);
			progress++;

			if (Run.Depth > 0) {
				SaveManager.Load(gameArea, SaveType.Player);
			} else {
				SaveManager.Generate(gameArea, SaveType.Player);
			}

			progress++; // Should be 16 here
			Log.Info("Done loading level! Going to menu.");
				
			ready = true;
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (ready) {
				Engine.Instance.SetState(new InGameState(gameArea, false));
			}
		}

		public override void RenderUi() {
			base.RenderUi();
			Graphics.Print($"Loading assets {progress / 16f * 100}%", Font.Medium, new Vector2(10, 10));
		}
	}
}