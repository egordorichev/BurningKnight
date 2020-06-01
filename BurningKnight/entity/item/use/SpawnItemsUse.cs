using System;
using System.Collections.Generic;
using BurningKnight.assets.items;
using BurningKnight.entity.component;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.item.use {
	public class SpawnItemsUse : ItemUse {
		public const int Distance = 24;
		public List<ItemPair> ToSpawn = new List<ItemPair>();
		
		public override void Use(Entity entity, Item item) {
			var count = 0;

			var center = entity.Center;
			var room = entity.GetComponent<RoomComponent>().Room;

			if (room != null) {
				center = room.Center;
			}
			
			foreach (var i in ToSpawn) {
				count += i.Count;
			}

			var j = 0f;
			
			for (var i = 0; i < ToSpawn.Count; i++) {
				var it = ToSpawn[i];

				for (var m = 0; m < it.Count; m++) {
					var itm = Items.CreateAndAdd(it.Id, entity.Area);
					var angle = j / count * Math.PI * 2;
					
					itm.Center = center + new Vector2((float) Math.Cos(angle) * Distance, (float) Math.Sin(angle) * Distance);
					
					j++;
				}
			}
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);

			ToSpawn.Clear();
			var v = settings["items"];

			if (!v.IsJsonArray) {
				return;
			}

			foreach (var i in v.AsJsonArray) {
				ToSpawn.Add(new ItemPair {
					Count = i[0],
					Id = i[1]
				});
			}
		}

		public static void RenderDebug(JsonValue root) {
			var toRemove = -1;
			
			if (!root["items"].IsJsonArray) {
				root["items"] = new JsonArray();
			}
			
			var toSpawn = root["items"].AsJsonArray;

			for (var i = 0; i < toSpawn.Count; i++) {
				var item = toSpawn[i];
				var v = item[0].Int(1);
				var n = item[1].String("");

				if (ImGui.InputText($"##ss{i}", ref n, 128)) {
					item[1] = n;
				}
				
				ImGui.SameLine();

				if (ImGui.InputInt($"##sl{i}", ref v)) {
					item[0] = v;
				}
				
				ImGui.SameLine();
				
				if (ImGui.Button("-")) {
					toRemove = i;
				}
			}

			if (ImGui.Button("+")) {
				toSpawn.Add(new JsonArray {
					1, "bk:copper_coin"
				});
			}

			if (toRemove > -1) {
				toSpawn.Remove(toRemove);
			}
		}
	}
}