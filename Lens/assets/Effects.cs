using System.Collections.Generic;
using Lens.util.file;
using Microsoft.Xna.Framework.Graphics;

namespace Lens.assets {
	public static class Effects {
		public static Dictionary<string, Effect> All = new Dictionary<string, Effect>();
		
		public static void Load() {
			var textureDir = FileHandle.FromRoot("Effects/");
			
			if (textureDir.Exists()) {
				foreach (var h in textureDir.ListFileHandles()) {
					LoadEffect(h);
				}
			}
		}

		private static void LoadEffect(FileHandle handle) {
			All[handle.NameWithoutExtension] = Assets.Content.Load<Effect>(handle.FullPath);
		}
		
		public static void Destroy() {
			foreach (var e in All) {
				e.Value.Dispose();
			}	
			
			All.Clear();
		}

		public static Effect Get(string id) {
			return All.TryGetValue(id, out var o) ? o : null;
		}
	}
}