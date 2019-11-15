using System.Collections.Generic;
using BurningKnight.entity.component;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;
using Lens.util.math;

namespace BurningKnight.entity.item.use.parent {
	public abstract class DoWithTagUse : ItemUse {
		private bool self;
		private bool sameRoom;
		private bool all;
		private int tag;

		public override void Use(Entity entity, Item item) {
			var list = new List<Entity>();

			if (self) {
				list.Add(entity);
			}

			var tags = (sameRoom ? entity.GetComponent<RoomComponent>().Room.Tagged : entity.Area.Tagged);

			for (var i = 0; i < BitTag.Total; i++) {
				if ((tag & 1 << i) != 0) {
					var l = tags[i];
					
					if (all) {
						list.AddRange(l);
					} else if (l.Count > 0) {
						list.Add(l[Random.Int(l.Count)]);
					}
				}
			}
			
			DoAction(entity, item, list);
		}

		protected abstract void DoAction(Entity entity, Item item, List<Entity> entities);

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
				
			self = settings["self"].Bool(false);
			sameRoom = settings["same_room"].Bool(true);
			all = settings["all"].Bool(true);
			tag = settings["tag"].Int(1);
		}
		
		public static void RenderDebug(JsonValue root) {
			var val = root["self"].Bool(false);

			if (ImGui.Checkbox("Self", ref val)) {
				root["self"] = val;
			}
			
			ImGui.Separator();
			
			val = root["same_room"].Bool(false);

			if (ImGui.Checkbox("Same room?", ref val)) {
				root["same_room"] = val;
			}
			
			val = root["all"].Bool(false);

			if (ImGui.Checkbox("All?", ref val)) {
				root["all"] = val;
			}
			
			var tag = root["tag"].Int(0);
			ImGui.Text($"Tags: {tag}");
			
			for (var i = 0; i < BitTag.Total; i++) {
				var t = BitTag.Tags[i];
				var on = (tag & 1 << i) != 0;
				
				if (ImGui.Checkbox(t.Name, ref on)) {
					if (on) {
						tag |= 1 << i;
					} else {
						tag &= ~(1 << i);
					}

					root["tag"] = tag;
				}
			}
		}
	}
}