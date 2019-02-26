using BurningKnight.entity.creature.player;
using Lens.util.file;

namespace BurningKnight.entity.level.save {
	public class PlayerSave {
		public static List<SaveableEntity> All = new List<>();

		public static void Remove(SaveableEntity Entity) {
			All.Remove(Entity);
		}

		public static void Add(SaveableEntity Entity) {
			All.Add(Entity);
		}

		public static void Save(FileWriter Writer) {
			try {
				Writer.WriteInt32(All.Size());

				foreach (SaveableEntity Entity in All) {
					Writer.WriteString(Entity.GetClass().GetName().Replace("org.rexcellentgames.burningknight.", ""));
					Entity.Save(Writer);
				}
			}
			catch (Exception) {
				E.PrintStackTrace();
			}
		}

		public static void Load(FileReader Reader) {
			try {
				All.Clear();
				var Count = Reader.ReadInt32();

				for (var I = 0; I < Count; I++) {
					var T = Reader.ReadString();
					Class Clazz = Class.ForName("org.rexcellentgames.burningknight." + T);
					var Entity = (SaveableEntity) Clazz.NewInstance();
					Dungeon.Area.Add(Entity);
					All.Add(Entity);
					Entity.Load(Reader);
				}
			}
			catch (IllegalAccessException) {
				E.PrintStackTrace();
			}
			catch (InstantiationException) {
				E.PrintStackTrace();
			}
			catch (ClassNotFoundException) {
				E.PrintStackTrace();
			}
		}

		public static void Generate() {
			var Player = new Player();
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