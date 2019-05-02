using System.Collections.Generic;
using System.IO;
using Aseprite;
using Lens.graphics;
using Lens.util;
using Lens.util.file;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lens.assets {
	public class Textures {
		private static Dictionary<string, TextureRegion> textures = new Dictionary<string, TextureRegion>();
		public static TextureRegion Missing;
		
		internal static void Load() {
			var textureDir = FileHandle.FromRoot("Textures/");
			
			if (textureDir.Exists()) {
				LoadTextures(textureDir);
			}
		}

		private static void LoadTextures(FileHandle handle) {
			foreach (var h in handle.ListFileHandles()) {
				LoadTexture(h);
			}

			foreach (var h in handle.ListDirectoryHandles()) {
				LoadTextures(h);
			}
		}

		private static void LoadTexture(FileHandle handle) {
			var region = new TextureRegion();
			string id = handle.NameWithoutExtension;
			
			var fileStream = new FileStream(handle.FullPath, FileMode.Open);
			region.Texture = Texture2D.FromStream(Engine.GraphicsDevice, fileStream);
			region.Source = region.Texture.Bounds;
			region.Center = new Vector2(region.Source.Width / 2f, region.Source.Height / 2f);
			fileStream.Dispose();
			
			textures[id] = region;
		}

		internal static void Destroy() {
			foreach (var region in textures.Values) {
				region.Texture.Dispose();
			}
			
			textures.Clear();
		}
		
		public static TextureRegion Get(string id) {
			if (textures.TryGetValue(id, out var region)) {
				return region;
			}
			
			Log.Error($"Texture {id} was not found!");
			return Missing;
		}
	}
}