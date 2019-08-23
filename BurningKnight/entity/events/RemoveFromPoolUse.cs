using System.Collections.Generic;
using BurningKnight.assets.items;
using BurningKnight.entity.item;
using BurningKnight.entity.item.use;
using BurningKnight.save;
using BurningKnight.state;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.events {
	public class RemoveFromPoolUse : ItemUse {
		private List<string> items = new List<string>();

		public override void Use(Entity entity, Item item) {
			if (item.Used) {
				return;
			}

			foreach (var i in items) {
				Run.Statistics.Banned.Add(i);
			}
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			items.Clear();

			foreach (var i in settings["items"].AsJsonArray) {
				items.Add(i.String(""));
			}
		}

		public static void RenderDebug(JsonValue root) {
			if (!root["items"].IsJsonArray) {
				root["items"] = new JsonArray();
			}
			
			var items = root["items"].AsJsonArray;
			var toRemove = -1;
			
			for (var i = 0; i < items.Count; i++) {
				var item = items[i].AsString;
				
				if (ImGui.InputText($"##item{i}", ref item, 128)) {
					items[i] = item;
				}
				
				ImGui.SameLine();

				if (ImGui.Button("-")) {
					toRemove = i;
				}

				if (!Items.Has(item)) {
					ImGui.BulletText("Unknown item!");
				}	
			}

			if (toRemove > -1) {
				items.Remove(toRemove);
			}

			if (ImGui.Button("+")) {
				items.Add("");
			}
		}
	}
}