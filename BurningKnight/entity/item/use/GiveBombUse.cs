using System;
using BurningKnight.entity.creature.player;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class GiveBombUse: ItemUse {
		public int Amount;

		public override void Use(Entity entity, Item item) {
			var h = entity.GetComponent<HeartsComponent>();
			var a = Amount;

			if (h.Bombs < h.BombsMax) {
				var t = Math.Min(Amount, h.BombsMax - h.Bombs);
				h.ModifyBombs(t, null);
				a -= t;
			}

			if (a > 0) {
				entity.GetComponent<ConsumablesComponent>().Bombs += a;
			}
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			Amount = settings["amount"].Int(1);
		}
		
		public static void RenderDebug(JsonValue root) {
			var val = root["amount"].Int(1);

			if (ImGui.InputInt("Amount", ref val)) {
				root["amount"] = val;
			}
		}
	}
}