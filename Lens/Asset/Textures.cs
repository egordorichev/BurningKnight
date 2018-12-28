using System.Collections.Generic;
using System.IO;
using Lens.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lens.Asset {
	public class Textures {
		private static Dictionary<string, TextureRegion> textures = new Dictionary<string, TextureRegion>();
		
		internal static void Load() {
			
		}

		internal static void Destroy() {
			foreach (var region in textures.Values) {
				region.Texture.Dispose();
			}
			
			textures.Clear();
		}

		public static TextureRegion Get(string id) {
			TextureRegion region;

			if (textures.TryGetValue(id, out region)) {
				return region;
			}
			
			var fileStream = new FileStream($"{Assets.Root}{id}.png", FileMode.Open);
			
			region = new TextureRegion();			
			region.Texture = Texture2D.FromStream(Engine.GraphicsDevice, fileStream);
			region.Source = new Rectangle(0, 0, region.Texture.Width, region.Texture.Height);
			
			textures[id] = region;
			fileStream.Dispose();		
			
			return region;
		}
	}
}