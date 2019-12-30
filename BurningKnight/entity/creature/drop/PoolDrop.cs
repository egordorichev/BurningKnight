using System.Collections.Generic;
using BurningKnight.assets.items;
using BurningKnight.debug;
using BurningKnight.entity.item;
using BurningKnight.ui.imgui;
using BurningKnight.util;
using ImGuiNET;
using Lens.lightJson;
using Lens.util.math;

namespace BurningKnight.entity.creature.drop {
	public class PoolDrop : Drop {
		public ItemPool Pool;
		public int Min;
		public int Max;

		public PoolDrop() {
			
		}
		
		public PoolDrop(ItemPool pool, float chance = 1f, int min = 1, int max = 1) {
			Pool = pool;
			Chance = chance;
			Min = min;
			Max = max;
		}

		public override List<string> GetItems() {
			var list = new List<string>();

			for (var i = 0; i < Rnd.Int(Min, Max + 1); i++) {
				list.Add(Items.Generate(Pool));
			}

			return list;
		}

		public override string GetId() {
			return "pool";
		}

		public override void Load(JsonValue root) {
			base.Load(root);

			Min = root["min"].Int(1);
			Max = root["max"].Int(1);
			Pool = ItemPool.ById[root["pool"].Int(0)];
		}

		public override void Save(JsonValue root) {
			base.Save(root);

			root["min"] = Min;
			root["max"] = Max;
			root["pool"] = Pool.Id;
		}

		public static void RenderDebug(JsonValue root) {
			root.InputFloat("Chance", "chance");

			root.InputInt("Min Count", "min");
			root.InputInt("Max Count", "max");
			
			var pool = root["pool"].Int(0);

			if (ImGui.Combo("Pool##p", ref pool, ItemPool.Names, ItemPool.Count)) {
				root["pool"] = pool;
			}

			if (ImGui.Button("View pool")) {
				WindowManager.PoolEditor = true;
				PoolEditor.Pool = pool;
			}
		}
	}
}