using System.Collections.Generic;
using System.IO;
using Lens.graphics;
using Lens.util;
using Microsoft.Xna.Framework.Content;

namespace Lens.assets {
	public static class Assets {
		public static ContentManager Content;
		// If true, Assets.Content wont be used, the original files will be loaded
		public static bool LoadOriginalFiles = true;
		public static string Root => LoadOriginalFiles ? Path.Combine(Directory.GetCurrentDirectory(), "../../Content/") : $"{Directory.GetCurrentDirectory()}/Content/bin/";
		
		private static string[] folders;
		private static List<FileSystemEventArgs> changed = new List<FileSystemEventArgs>();
		private static float lastUpdate;
		
		public static void Load() {
			Content.RootDirectory = "Content/";
			
			Graphics.Init();
			LoadAssets();

			if (LoadOriginalFiles) {				
				folders = new[] {
					"Textures/",
					"Animations/",
					"Sfx/"
				};

				for (int i = 0; i < folders.Length; i++) {
					var watcher = new FileSystemWatcher();
					
					watcher.Path = Path.GetFullPath(Root + folders[i]);
					watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
					                                                | NotifyFilters.FileName | NotifyFilters.DirectoryName;

					watcher.Changed += OnChanged;
					watcher.Created += OnChanged;
					watcher.Deleted += OnChanged;

					watcher.EnableRaisingEvents = true;
				}
			}
		}

		private static void OnChanged(object source, FileSystemEventArgs e) {
			changed.Add(e);
		}

		private static void LoadAssets() {
			// Locale.Load("en");
			Textures.Load();
			Animations.Load();
			// Audio.Load();
		}

		public static void Destroy() {
			Graphics.Destroy();
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
			if (changed.Count == 0) {
				return;
			}
			
			var reloadedTextures = false;
			var reloadedSfx = false;
			var reloadedAnimations = false;

			var changedClone = changed.ToArray();
			
			foreach (var e in changedClone) {
				switch (new DirectoryInfo(Path.GetDirectoryName(e.FullPath)).Name) {
					case "Textures": {
						if (!reloadedTextures) {
							Log.Debug("Reloading textures...");
							reloadedTextures = true;
						}

						break;
					}

					case "Sfx": {
						if (!reloadedSfx) {
							Log.Debug("Reloading sfx...");

							Audio.Destroy();
							Audio.Load();
							
							reloadedSfx = true;
						}

						break;
					}

					case "Animations": {
						if (!reloadedAnimations) {
							Log.Debug("Reloading animations...");

							Animations.Destroy();
							Animations.Load();
							Animations.Reload = true;

							reloadedAnimations = true;
						}

						break;
					}
				}
			}
			
			changed.Clear();
		}
	}
}