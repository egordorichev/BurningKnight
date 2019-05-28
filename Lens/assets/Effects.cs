using System.Collections.Generic;
using Lens.util.file;
using Microsoft.Xna.Framework.Graphics;

namespace Lens.assets {
	public static class Effects {
		public static Dictionary<string, Effect> All = new Dictionary<string, Effect>();
		
		public static void Load() {
			var shaderDir = FileHandle.FromRoot("bin/Shaders/");
			
			if (shaderDir.Exists()) {
				foreach (var h in shaderDir.ListFileHandles()) {
					if (h.Extension == ".xnb") {
						LoadEffect(h);
					}
				}
			}
		}

		private static void LoadEffect(FileHandle handle) {
			All[handle.NameWithoutExtension] = Assets.Content.Load<Effect>($"bin/Shaders/{handle.NameWithoutExtension}");
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