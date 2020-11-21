using System;
using System.Diagnostics;
using Aseprite;
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
using Lens.util.file;
using Microsoft.Xna.Framework;
using Version = Lens.Version;

namespace BurningKnight {
	public class BK : Engine {
		public const bool StandMode = false;
		public const bool Demo = false;
		
		// Name removed cuz release bois
		public static Version Version = new Version("Xmas out-of-time update", 48, 1, 2, 0, 0, Debug);
		
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
			
			/*AsepriteReader.GraphicsDevice = Engine.GraphicsDevice;
			
			var dir = FileHandle.FromRoot("Animations");
			var outDir = new FileHandle("out");

			if (!outDir.Exists()) {
				outDir.MakeDirectory();
			}

			var outDirPath = outDir.FullPath;

			foreach (var f in dir.ListFileHandles()) {
				var fullPath = f.FullPath;
				var file = new AsepriteFile(fullPath);

				if (file.Slices.Count > 0) {
					Console.WriteLine($"{f.NameWithoutExtension} (sliced)");
					RunBash($"aseprite -b {fullPath} --save-as {outDirPath}/{f.NameWithoutExtension}_{{slice}}.png");
				} else {
					Console.WriteLine($"{f.NameWithoutExtension} (framed)");
					RunBash($"aseprite -b {fullPath} --list-tags --save-as {outDirPath}/{f.NameWithoutExtension}_{{tag}}_{{tagframe00}}.png");
				}
			}

			Environment.Exit(0);*/

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
