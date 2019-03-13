using BurningKnight.assets;
using BurningKnight.entity.creature.player;
using BurningKnight.integration.discord;
using Lens;
using Lens.assets;
using Lens.game;
using Microsoft.Xna.Framework;

namespace BurningKnight {
	public class BK : Engine {
		public BK(GameState state, string title, int width, int height, bool fullscreen) : base(state, title, width, height, fullscreen) {
			
		}

		protected override void Initialize() {
			base.Initialize();
			
			Discord.Init();
			Controls.Bind();
			
			CommonAse.Load();
			Shaders.Load();
			Font.Load();
			Mods.Load();
		}

		protected override void UnloadContent() {
			Discord.Destroy();
			Mods.Destroy();
			
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