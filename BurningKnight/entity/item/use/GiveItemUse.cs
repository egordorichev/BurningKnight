using BurningKnight.assets.items;
using BurningKnight.entity.component;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;
using Lens.util;

namespace BurningKnight.entity.item.use {
	public class GiveItemUse : ItemUse {
		public int Amount;
		public string Item;

		public override void Use(Entity entity, Item item) {
			for (int j = 0; j < Amount; j++) {
				var i = Items.CreateAndAdd(Item, entity.Area);

				if (i == null) {
					Log.Error($"Invalid item {item}");
					return;
				}

				entity.GetComponent<InventoryComponent>().Pickup(i);
			}
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			
			Amount = settings["amount"].Int(1);
			Item = settings["item"].AsString ?? "";
		}
		
		public static void RenderDebug(JsonValue root) {
			var val = root["amount"].Int(1);
			var item = root["item"].AsString ?? "";

			if (ImGui.InputText("Item", ref item, 128)) {
				root["item"] = item;
			}

			if (!Items.Datas.ContainsKey(item)) {
				ImGui.BulletText("Unknown item!");
			}
			
			if (ImGui.InputInt("Amount", ref val)) {
				root["amount"] = val;
			}
		}
	}
}