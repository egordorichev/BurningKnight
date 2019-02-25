using BurningKnight.core.game.state;
using BurningKnight.core.util.file;

namespace BurningKnight.core.entity.level.save {
	public class LevelSave {
		public static List<SaveableEntity> All = new List<>();
		private static int I;

		public static Void Remove(SaveableEntity Entity) {
			All.Remove(Entity);
		}

		public static Void Add(SaveableEntity Entity) {
			All.Add(Entity);
		}

		public static Void Save(FileWriter Writer) {
			try {
				Dungeon.Level.Save(Writer);
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
			All.Clear();
			Dungeon.Level = Level.ForDepth(Dungeon.Depth);
			Dungeon.Area.Add(Dungeon.Level);
			Dungeon.Level.Load(Reader);
			int Count = Reader.ReadInt32();

			for (int I = 0; I < Count; I++) {
				string T = Reader.ReadString();
				Class Clazz = null;

				try {
					Clazz = Class.ForName("org.rexcellentgames.burningknight." + T);
				} catch (ClassNotFoundException) {
					E.PrintStackTrace();
				}

				SaveableEntity Entity;

				try {
					Entity = (SaveableEntity) Clazz.NewInstance();
				} catch (InstantiationException) {
					E.PrintStackTrace();

					continue;
				} catch (IllegalAccessException) {
					E.PrintStackTrace();

					continue;
				}

				Dungeon.Area.Add(Entity);
				All.Add(Entity);
				Entity.Load(Reader);
			}
		}

		public static Void Generate() {
			try {
				LoadState.Generating = true;
				Dungeon.Level = Level.ForDepth(Dungeon.Depth);
				Dungeon.Area.Add(Dungeon.Level);
				Dungeon.Level.Generate(I);
				I = 0;
			} catch (RuntimeException) {
				E.PrintStackTrace();
				Generate();
				I++;
			}
		}
	}
}
