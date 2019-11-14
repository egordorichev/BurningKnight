using BurningKnight.assets;
using BurningKnight.assets.input;
using BurningKnight.assets.items;
using BurningKnight.assets.lighting;
using BurningKnight.assets.mod;
using BurningKnight.assets.prefabs;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.util;
using Lens;
using Microsoft.Xna.Framework;

namespace BurningKnight {
	public class BK : Engine {
		public static bool StandMode = false;
		public static Version Version = new Version("Boss update", 16, 0, 1, 8, 0, true, Debug);
		
		public BK(int width, int height, bool fullscreen) : base(Version, 
			#if DEBUG
				new DevAssetLoadState(),
			#else
				new AssetLoadState(),			
			#endif
			 $"Burning Knight: {Titles.Generate()}", width, height, fullscreen) {
		}

		protected override void Initialize() {
			base.Initialize();

			SaveManager.Init();
			Controls.Load();
			Font.Load();
			ImGuiHelper.Init();
		}

		protected override void UnloadContent() {
			Mods.Destroy();
			Items.Destroy();
			Prefabs.Destroy();
			Lights.DestroySurface();
			
			base.UnloadContent();
		}

		protected override void Update(GameTime gameTime) {
			base.Update(gameTime);
			Mods.Update((float) gameTime.ElapsedGameTime.TotalSeconds);
		}

		public override void RenderUi() {
			base.RenderUi();
			Mods.Render();
		}
	}
}
