using System;
using Lens;
using Lens.entity;
using Lens.util.file;

namespace BurningKnight.save {
	public class SecretSave : Saver {
		public static bool Loaded;

		public static void Load(Area area) {
			if (Loaded) {
				return;
			}

			Loaded = true;
			SaveManager.Load(area, SaveType.Secret);
		}
		
		public override string GetPath(string path, bool old = false) {
			return $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/.nothing.sv";
		}

		public static bool WasATester;

		public override void Load(Area area, FileReader reader) {
			WasATester = reader.ReadBoolean();
		}

		public override void Save(Area area, FileWriter writer) {
			writer.WriteBoolean(WasATester);
		}

		public override void Generate(Area area) {
			WasATester = Engine.Version.Debug;
		}
	}
}