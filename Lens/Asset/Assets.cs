using System.IO;
using Lens.Graphics;
using Lens.Util;
using Microsoft.Xna.Framework.Content;

namespace Lens.Asset {
	public static class Assets {
		public static ContentManager Content;
		// If true, Assets.Content wont be used, the original files will be loaded
		public static bool LoadOriginalFiles = true;
		public static string Root => LoadOriginalFiles ? $"{Directory.GetCurrentDirectory()}/Content/" : $"{Directory.GetCurrentDirectory()}/Content/bin/";
		
		private static long[] lastChanged;
		private static string[] folders;
		private static float lastUpdate;
		
		public static void Load() {
			Content.RootDirectory = "Content/";
			
			Gr.Init();
			LoadAssets();

			if (LoadOriginalFiles) {				
				folders = new[] {
					"Textures/",
					"Animations/",
					"Sfx/"
				};
				
				lastChanged = new long[folders.Length];

				for (int i = 0; i < folders.Length; i++) {
					lastChanged[i] = File.GetLastWriteTime(folders[i]).ToFileTime();
				}
			}
		}

		private static void LoadAssets() {
			Textures.Load();
			Animations.Load();
			Audio.Load();
		}

		public static void Destroy() {
			Gr.Destroy();
			DestroyAssets();
		}

		private static void DestroyAssets() {
			Textures.Destroy();
			Animations.Destroy();
			Audio.Destroy();
		}

		public static void Update(float dt) {
			Animations.Reload = false;
			lastUpdate += dt;

			if (lastUpdate >= 0.3f) {
				lastUpdate = 0;
				CheckForUpdates();
			}
		}

		private static void CheckForUpdates() {
			for (int i = 0; i < folders.Length; i++) {
				long changed = File.GetLastWriteTime(Path.GetFullPath(Root + folders[i])).ToFileTime();

				if (changed != lastChanged[i]) {
					Log.Debug("Asset directory was changed " + folders[i]);
					lastChanged[i] = changed;
					
					switch (folders[i]) {
						case "Textures/": {
							Log.Debug("Reloading textures...");
							
							break;
						}

						case "Sfx/": {
							Log.Debug("Reloading sfx...");
							
							Audio.Destroy();
							Audio.Load();
							
							break;
						}

						case "Animations/": {
							Log.Debug("Reloading animations...");
							
							Animations.Destroy();
							Animations.Load();
							Animations.Reload = true;
							
							break;
						}
					}
				}
			}
		}
	}
}