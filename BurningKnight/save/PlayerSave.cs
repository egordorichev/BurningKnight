using System;
using BurningKnight.assets.items;
using BurningKnight.entity;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using Lens;
using Lens.entity;
using Lens.util.file;

namespace BurningKnight.save {
	public class PlayerSave : Saver {
		public override void Save(Area area, FileWriter writer) {
			var all = area.Tags[Tags.PlayerSave];
			writer.WriteInt32(all.Count);

			foreach (var entity in all) {
				var e = (SaveableEntity) entity;
				writer.WriteString(e.GetType().FullName.Replace("BurningKnight.", ""));
				e.Save(writer);
			}
		}

		public override string GetPath(string path, bool old = false) {
			return $"{path}player.sv";
		}

		public override void Load(Area area, FileReader reader) {
			var count = reader.ReadInt32();

			for (var I = 0; I < count; I++) {
				var entity = (SaveableEntity) Activator.CreateInstance(Type.GetType($"BurningKnight.{reader.ReadString()}", true, false));

				area.Add(entity, false);
				entity.Load(reader);
				entity.PostInit();
			}
		}

		public override void Generate(Area area) {
			var player = new LocalPlayer();
			area.Add(player);

			if (Engine.Version.Dev) {
				// Simple inventory simulation
				var inventory = player.GetComponent<InventoryComponent>();
				inventory.Pickup(Items.CreateAndAdd("bk:sword", area));
			}
		}
	}
}