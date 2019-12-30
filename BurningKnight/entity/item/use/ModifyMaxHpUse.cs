using System;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity.component;
using ImGuiNET;
using Lens.assets;
using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class ModifyMaxHpUse : ItemUse {
		public int Amount;
		public bool GiveHp;

		public override void Use(Entity entity, Item item) {
			var component = entity.GetComponent<HealthComponent>();
			component.MaxHealth += Amount;
			
			if (GiveHp && Amount > 0) {
				component.ModifyHealth(Amount, entity);
				TextParticle.Add(entity, "HP", Amount, true);
			}
			
			TextParticle.Add(entity, Locale.Get("max_hp"), Math.Abs(Amount), true, Amount < 0);
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			
			Amount = settings["amount"].Int(1);
			GiveHp = settings["give_hp"].Bool(true);
		}
		
		public static void RenderDebug(JsonValue root) {
			var val = root["amount"].Int(1);

			if (ImGui.InputInt("Amount", ref val)) {
				root["amount"] = val;
			}

			var giveHp = root["give_hp"].Bool(true);

			if (ImGui.Checkbox("Give health", ref giveHp)) {
				root["give_hp"] = giveHp;
			}
		}
	}
}