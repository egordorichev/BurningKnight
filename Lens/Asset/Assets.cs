using System.IO;
using Lens.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Lens.Asset {
	public static class Assets {
		public static ContentManager Content;
		// If true, Assets.Content wont be used, the original files will be loaded
		public static bool LoadOriginalFiles = true;
		public static string Root => LoadOriginalFiles ? $"{Directory.GetCurrentDirectory()}/Content/" : $"{Directory.GetCurrentDirectory()}/Content/bin/";
		
		public static void Load() {
			Content.RootDirectory = "Content/";
			
			Gr.Init();
			Textures.Load();
			Animations.Load();
			Audio.Load();
		}

		public static void Destroy() {
			Gr.Destroy();
			Textures.Destroy();
			Animations.Destroy();
			Audio.Destroy();
		}
	}
}