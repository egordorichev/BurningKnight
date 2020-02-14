using System;
using System.Reflection;
using Lens.util;
using Lens.util.file;

namespace BurningKnight.assets.mod {
	public abstract class Mod {
		public string Prefix { get; protected set; } = "null";

		public abstract void Init();
		public abstract void Destroy();
		public abstract void Update(float dt);
		public abstract void Render();
		
		public static Mod Load(FileHandle file) {
			var dll = Assembly.LoadFile(file.FullPath);

			foreach (var type in dll.ExportedTypes) {
				if (typeof(Mod).IsAssignableFrom(type)) {
					try {
						var mod = (Mod) Activator.CreateInstance(type);
						
						return mod;
					} catch (Exception e) {
						Log.Error(e);
						return null;
					}
				}
			}
			
			return null;
		}
	}
}