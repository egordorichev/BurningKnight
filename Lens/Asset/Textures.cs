using System.Collections.Generic;
using System.IO;
using Aseprite;
using Lens.Graphics;
using Lens.Util;
using Lens.Util.File;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lens.Asset {
	public class Textures {
		private static Dictionary<string, TextureRegion> textures = new Dictionary<string, TextureRegion>();
		
		internal static void Load() {
			var textureDir = FileHandle.FromRoot("Textures/");

			if (textureDir.Exists()) {
				foreach (var id in textureDir.ListFiles()) {
					LoadTexture(Path.GetFileNameWithoutExtension(id));
				} 	
			}

			AsepriteReader.GraphicsDevice = Engine.GraphicsDevice;
		}

		private static void LoadTexture(string id) {
			var region = new TextureRegion();
			
			// Can be loaded without content: Texture2D.FromStream(Engine.GraphicsDevice, fileStream);
			region.Texture = Assets.Content.Load<Texture2D>("bin/Textures/" + id);
			region.Source = new Rectangle(0, 0, region.Texture.Width, region.Texture.Height);
			
			textures[id] = region;
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
			
			Log.Error($"Texture {id} was not found!");
			return null; // TODO: missing texture placeholder
		}
	}
}