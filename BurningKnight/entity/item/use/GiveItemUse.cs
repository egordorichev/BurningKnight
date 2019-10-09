using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.item.stand;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;
using Lens.util;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.item.use {
	public class GiveItemUse : ItemUse {
		public int Amount;
		public string Item;
		public bool OnStand;
		public bool Random;

		public override void Use(Entity entity, Item item) {
			var id = Random ? Items.Generate() : Item;
			
			if (OnStand) {
				var i = Items.CreateAndAdd(id, entity.Area);

				if (i == null) {
					Log.Error($"Invalid item {id}");
					return;
				}

				var stand = new ItemStand();
				entity.Area.Add(stand);
				stand.Center = entity.Center - new Vector2(0, 16);
				stand.SetItem(i, null);
				
				return;
			}
			
			for (var j = 0; j < Amount; j++) {
				var i = Items.CreateAndAdd(id, entity.Area);

				if (i == null) {
					Log.Error($"Invalid item {id}");
					return;
				}

				entity.GetComponent<InventoryComponent>().Pickup(i);
			}
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			
			Amount = settings["amount"].Int(1);
			Item = settings["item"].AsString ?? "";
			OnStand = settings["on_stand"].Bool(false);
			Random = settings["random"].Bool(false);
		}
		
		public static void RenderDebug(JsonValue root) {
			var stand = root["on_stand"].Bool(false);
			var random = root["random"].Bool(false);

			if (ImGui.Checkbox("Spawn on stand?", ref stand)) {
				root["on_stand"] = stand;
			}
			
			if (ImGui.Checkbox("Random item?", ref random)) {
				root["random"] = random;
			}
			
			if (stand) {
				return;
			}
			
			var val = root["amount"].Int(1);

			if (!random) {
				var item = root["item"].AsString ?? "";

				if (ImGui.InputText("Item", ref item, 128)) {
					root["item"] = item;
				}

				if (!Items.Datas.ContainsKey(item)) {
					ImGui.BulletText("Unknown item!");
				}
			}
			
			if (ImGui.InputInt("Amount", ref val)) {
				root["amount"] = val;
			}
		}
	}
}