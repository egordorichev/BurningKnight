using System;
using Lens;
using Lens.entity;
using Lens.util;
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

		public static bool HadSaveBefore = true;
		public static bool DeletedSave;
		public static bool WasATester;
		
		public override void Load(Area area, FileReader reader) {
			 DeletedSave = reader.ReadBoolean();
			 WasATester = reader.ReadBoolean();

			if (!HadSaveBefore) {
				DeletedSave = true;
				Log.Info("You think ya smart?");
			}

			if (WasATester) {
				Log.Info("Thanks for being a tester!");
			}
		}

		public override void Save(Area area, FileWriter writer) {
			WasATester = Engine.Version.Debug;
			
			writer.WriteBoolean(DeletedSave);
			writer.WriteBoolean(WasATester);
		}

		public override void Generate(Area area) {
			DeletedSave = false;
			WasATester = Engine.Version.Debug;
		}

		public SecretSave() : base(SaveType.Secret) {
		
		}
	}
}