using System;
using System.Diagnostics;
using BurningKnight.assets;
using BurningKnight.assets.input;
using BurningKnight.assets.items;
using BurningKnight.assets.lighting;
using BurningKnight.assets.mod;
using BurningKnight.assets.prefabs;
using BurningKnight.level;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.util;
using Lens;
using Lens.util;
using Microsoft.Xna.Framework;
using Version = Lens.Version;

namespace BurningKnight {
	public class BK : Engine {
		public const bool StandMode = false;
		public const bool Demo = false;
		
		public static Version Version = new Version("Bad rock update", 50, 1, 3, 1, 2, Debug);
		
		public BK(int width, int height, bool fullscreen) : base(Version, 
			#if DEBUG
				new DevAssetLoadState(),
			#else
				new AssetLoadState(),
			#endif
			 $"Burning Knight{(Demo ? " Demo" : "")}: {Titles.Generate()}", width, height, fullscreen) {
		}

		protected override void Initialize() {
			base.Initialize();

			SaveManager.Init();
			Controls.Load();
			Font.Load();

			try {
				ImGuiHelper.Init();
			} catch (Exception e) {
				Log.Error(e);
			}
			
			Weather.Init();
		}

		private static void RunBash(string args) {
			var process = new Process {
				StartInfo = new ProcessStartInfo {
					FileName = "/bin/bash",
					Arguments = $"-c \"{args}\"",
					RedirectStandardOutput = true,
					UseShellExecute = false,
					CreateNoWindow = true,
				}
			};
			
			process.Start();
			var result = process.StandardOutput.ReadToEnd();
			process.WaitForExit();

			Console.WriteLine(result);
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
