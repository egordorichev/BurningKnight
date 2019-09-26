using System;
using BurningKnight.entity.creature.player;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;
using Lens.util;

namespace BurningKnight.entity.item.use {
	public class ModifyActiveChargeUse : ItemUse {
		private bool percent;
		private float amount;

		public override void Use(Entity entity, Item item) {
			var i = entity.GetComponent<ActiveItemComponent>().Item;

			if (i == null) {
				return;
			}
			
			if (percent) {
				i.Delay = Math.Max(0, i.Delay - i.UseTime * amount * 0.01f);
			} else {
				i.Delay = Math.Max(0, i.Delay - amount);
			}

			if (i.Delay <= 0.001f) {
				i.Delay = 0;
			}
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);

			percent = settings["percent"].Bool(false);
			amount = settings["amount"].Number(1f);
		}

		public static void RenderDebug(JsonValue root) {
			var percent = root["percent"].Bool(false);

			if (ImGui.Checkbox("Percent?", ref percent)) {
				root["percent"] = percent;
			}
			
			var amount = root["amount"].Number(1f);

			if (ImGui.InputFloat(percent ? "Amount (%)" : "Amount (charge)", ref amount)) {
				root["amount"] = amount;
			}
		}
	}
}