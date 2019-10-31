using System.Collections.Generic;
using BurningKnight;
using Desktop.integration;
using Desktop.integration.crash;
using Desktop.integration.discord;
using Desktop.integration.rgb;
using Desktop.integration.steam;
using Lens;
using Microsoft.Xna.Framework;

namespace Desktop {
	public class DesktopApp : BK {
		private List<Integration> integrations = new List<Integration>();

		public DesktopApp() : base(Display.Width * 3, Display.Height * 3, false) {
			CrashReporter.Bind();
		}

		protected override void Initialize() {
			base.Initialize();

			if (!BK.Version.Dev) {
				integrations.Add(new DiscordIntegration());
				integrations.Add(new SteamIntegration());

				// integrations.Add(new RgbIntegration());
			}

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
			
			foreach (var i in integrations) {
				i.Update(Delta);
			}
		}
	}
}