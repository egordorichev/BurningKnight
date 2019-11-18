using System.Linq;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;
using Lens.util.math;

namespace BurningKnight.entity.item.use {
	public class KillMobUse : ItemUse {
		private bool all;
		private int count;

		public override void Use(Entity entity, Item item) {
			base.Use(entity, item);

			var room = entity.GetComponent<RoomComponent>().Room;

			if (room == null) {
				return;
			}

			var mobs = room.Tagged[Tags.Mob].ToList();

			if (all) {
				foreach (var m in mobs) {
					((Mob) m).Kill(entity);
				}

				return;
			}

			var i = 0;

			do {
				var index = Rnd.Int(mobs.Count);
				var mob = mobs[index];
				mobs.RemoveAt(index);

				((Mob) mob).Kill(entity);
				
				i++;
			} while (mobs.Count > 0 && i < count);
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			
			all = settings["all"].Bool(false);
			count = settings["count"].Int(1);
		}

		public static void RenderDebug(JsonValue root) {
			var all = root["all"].Bool(false);

			if (ImGui.Checkbox("All?", ref all)) {
				root["all"] = all;
			}

			if (all) {
				return;
			}

			var count = root["count"].Int(1);

			if (ImGui.InputInt("Count", ref count)) {
				root["count"] = count;
			}
		}
	}
}