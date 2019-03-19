using System.Collections.Generic;
using BurningKnight;
using Desktop.integration;
using Desktop.integration.discord;
using Lens.game;
using Microsoft.Xna.Framework;

namespace Desktop {
	public class DesktopApp : BK {
		private List<Integration> integrations = new List<Integration>();
		
		public DesktopApp(GameState state, string title, int width, int height, bool fullscreen) : base(state, title, width, height, fullscreen) {}

		protected override void LoadContent() {
			base.LoadContent();
			
			integrations.Add(new DiscordIntegration());

			foreach (var i in integrations) {
				i.Init();
			}
		}

		protected override void UnloadContent() {
			base.UnloadContent();
			
			foreach (var i in integrations) {
				i.Destroy();
			}
			
			integrations.Clear();
		}

		protected override void Update(GameTime gameTime) {
			base.Update(gameTime);
			float dt = gameTime.ElapsedGameTime.Seconds;
			
			foreach (var i in integrations) {
				i.Update(dt);
			}
		}
	}
}