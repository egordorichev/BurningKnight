using BurningKnight.core.entity.creature.player;
using BurningKnight.core.util.file;

namespace BurningKnight.core.entity.level.save {
	public class PlayerSave {
		public static List<SaveableEntity> All = new List<>();

		public static Void Remove(SaveableEntity Entity) {
			All.Remove(Entity);
		}

		public static Void Add(SaveableEntity Entity) {
			All.Add(Entity);
		}

		public static Void Save(FileWriter Writer) {
			try {
				Writer.WriteInt32(All.Size());

				foreach (SaveableEntity Entity in All) {
					Writer.WriteString(Entity.GetClass().GetName().Replace("org.rexcellentgames.burningknight.", ""));
					Entity.Save(Writer);
				}
			} catch (Exception) {
				E.PrintStackTrace();
			}
		}

		public static Void Load(FileReader Reader) {
			try {
				All.Clear();
				int Count = Reader.ReadInt32();

				for (int I = 0; I < Count; I++) {
					string T = Reader.ReadString();
					Class Clazz = Class.ForName("org.rexcellentgames.burningknight." + T);
					SaveableEntity Entity = (SaveableEntity) Clazz.NewInstance();
					Dungeon.Area.Add(Entity);
					All.Add(Entity);
					Entity.Load(Reader);
				}
			} catch (IllegalAccessException) {
				E.PrintStackTrace();
			} catch (InstantiationException) {
				E.PrintStackTrace();
			} catch (ClassNotFoundException) {
				E.PrintStackTrace();
			}
		}

		public static Void Generate() {
			Player Player = new Player();
			Dungeon.Area.Add(Player);
			All.Add(Player);
			Player.Generate();

			if (Dungeon.Quick) {
				Dungeon.Quick = false;
				GlobalSave.Put("last_class", Player.GetTypeId(Player.GetType()));
			} 
		}
	}
}
