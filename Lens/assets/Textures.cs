using System.Collections.Generic;
using System.IO;
using Aseprite;
using Lens.graphics;
using Lens.util;
using Lens.util.file;
using Microsoft.Xna.Framework.Graphics;

namespace Lens.assets {
	public class Textures {
		private static Dictionary<string, TextureRegion> textures = new Dictionary<string, TextureRegion>();
		
		internal static void Load() {
			var textureDir = FileHandle.FromRoot("Textures/");
			
			if (textureDir.Exists()) {
				// fixme: subdirs
				foreach (var id in textureDir.ListFiles()) {
					LoadTexture(id);
				} 	
			}

			AsepriteReader.GraphicsDevice = Engine.GraphicsDevice;
		}

		private static void LoadTexture(string id) {
			var region = new TextureRegion();

			if (Assets.LoadOriginalFiles) {
				var fileStream = new FileStream($"../../Content/Textures/{Path.GetFileName(id)}", FileMode.Open);
				Log.Error($"../../Content/Textures/{Path.GetFileName(id)}");
				region.Texture = Texture2D.FromStream(Engine.GraphicsDevice, fileStream);
				fileStream.Dispose();
				
				id = Path.GetFileNameWithoutExtension(id);
			} else {
				id = Path.GetFileNameWithoutExtension(id);
				region.Texture = Assets.Content.Load<Texture2D>($"bin/Textures/{id}");				
			}
			
			region.Source = region.Texture.Bounds;
			
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
			return null; // TODO: missing texture placeholder
		}
	}
}