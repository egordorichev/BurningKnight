using System.IO;
using Lens.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Lens.Asset {
	public static class Assets {
		public static ContentManager Content;
		public static string Root = $"{Directory.GetCurrentDirectory()}/Content/bin/";
	
		public static void Load() {
			Content.RootDirectory = "Content/";
			
			Renderer.Init();
			Textures.Load();
			Audio.Load();
		}

		public static void Destroy() {
			Renderer.Destroy();
			Textures.Destroy();
			Audio.Destroy();
		}
	}
}