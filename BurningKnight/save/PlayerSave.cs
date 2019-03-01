using System;
using BurningKnight.entity;
using BurningKnight.entity.creature.player;
using Lens.entity;
using Lens.util.file;

namespace BurningKnight.save {
	public class PlayerSave {
		public static void Save(Area area, FileWriter Writer) {
			var all = area.Tags[Tags.PlayerSave];
			Writer.WriteInt32(all.Count);

			foreach (SaveableEntity Entity in all) {
				Writer.WriteString(Entity.GetType().FullName.Replace("BurningKnight.", ""));
				Entity.Save(Writer);
			}
		}

		public static void Load(Area area, FileReader Reader) {
			var Count = Reader.ReadInt32();

			for (var I = 0; I < Count; I++) {
				var entity = (SaveableEntity) Activator.CreateInstance(Type.ReflectionOnlyGetType($"BurningKnight.{Reader.ReadString()}", true, false));

				area.Add(entity, false);
				entity.Load(Reader);
				entity.PostInit();
			}
		}

		public static void Generate(Area area) {
			area.Add(new LocalPlayer());
		}
	}
}