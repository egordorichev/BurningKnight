using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.item.stand;
using BurningKnight.util;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class SaleItemsUse : ItemUse {
		private float percent;
		private bool onlyOnceUsed;
		private bool used;

		public override void Use(Entity entity, Item item) {
			used = true;
			
			foreach (var i in entity.GetComponent<RoomComponent>().Room.Tagged[Tags.Item]) {
				if (i is ShopStand s) {
					s.Recalculate();
				}
			}

			used = false;
		}

		public override bool HandleEvent(Event e) {
			if (e is ItemPriceCalculationEvent ipce) {
				if (!onlyOnceUsed || used) {
					ipce.Percent += percent;
				}
			}
			
			return base.HandleEvent(e);
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			percent = settings["prc"].Number(50f);
			onlyOnceUsed = settings["wu"].Bool(false);
		}
		
		public static void RenderDebug(JsonValue root) {
			var v = root["prc"].Number(50f);

			if (ImGui.InputFloat("% sale", ref v)) {
				root["prc"] = v;
			}

			root.Checkbox("Only when used", "wu", false);
		}
	}
}