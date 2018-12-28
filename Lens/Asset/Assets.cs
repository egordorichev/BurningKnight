using Microsoft.Xna.Framework.Content;

namespace Lens.Asset {
	public static class Assets {
		public static ContentManager Content;
		public static string Root = "./";
		
	
		public static void Load() {
			Renderer.Init();
		}

		public static void Destroy() {
			Renderer.Destroy();
		}
	}
}