using System.Collections.Generic;
using BurningKnight;
using Desktop.integration;
using Desktop.integration.discord;
using Lens;
using Lens.game;
using Microsoft.Xna.Framework;

namespace Desktop {
	public class DesktopApp : BK {
		private List<Integration> integrations = new List<Integration>();

		public DesktopApp() : base(Display.Width * 3, Display.Height * 3, false) {
			
		}
		
		protected override void LoadContent() {
			base.LoadContent();
			
			// integrations.Add(new DiscordIntegration());

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