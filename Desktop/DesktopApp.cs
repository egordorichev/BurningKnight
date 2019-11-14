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

		public DesktopApp() : base(Display.Width * 3, Display.Height * 3, !BK.Version.Dev) {
			CrashReporter.Bind();
		}

		protected override void Initialize() {
			base.Initialize();

			integrations.Add(new DiscordIntegration());
			// integrations.Add(new RgbIntegration());

			integrations.Add(new SteamIntegration());

			foreach (var i in integrations) {
				i.Init();
			}
		}

		protected override void Destroy() {
			foreach (var i in integrations) {
				i.Destroy();
			}
			
			integrations.Clear();
			
			base.Destroy();
		}

		protected override void Update(GameTime gameTime) {
			base.Update(gameTime);
			
			foreach (var i in integrations) {
				i.Update(Delta);
			}
		}
	}
}
