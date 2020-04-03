using System;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.util;
using ImGuiNET;
using Lens.assets;
using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class ModifyMaxHpUse : ItemUse {
		public int Amount;
		public bool Set;
		public bool GiveHp;
		public bool Bomb;

		public override void Use(Entity entity, Item item) {
			if (Bomb) {
				var component = entity.GetComponent<HeartsComponent>();

				if (Set) {
					component.BombsMax = Amount;
				} else {
					component.BombsMax += Amount;
				}

				if (GiveHp && Amount > 0) {
					component.ModifyBombs(Amount, entity);
					TextParticle.Add(entity, "HP", Amount, true);
				}
			} else {
				if (Amount > 0) {
					if (entity.GetComponent<LampComponent>().Item?.Id == "bk:shielded_lamp") {
						entity.GetComponent<HeartsComponent>().ModifyShields(Amount, entity);

						return;
					} else if (entity.GetComponent<LampComponent>().Item?.Id == "bk:explosive_lamp") {
						var c = entity.GetComponent<HeartsComponent>();
						Amount = (int) Math.Ceiling(Amount / 2f);

						c.BombsMax += Amount;

						if (GiveHp) {
							c.ModifyBombs(Amount, entity);
							TextParticle.Add(entity, "HP", Amount, true);
						}

						return;	
					}
				}

				var component = entity.GetComponent<HealthComponent>();

				if (Set) {
					component.MaxHealth = Amount;
				} else {
					component.MaxHealth += Amount;
				}

				if (GiveHp && Amount > 0) {
					component.ModifyHealth(Amount, entity);
					TextParticle.Add(entity, "HP", Amount, true);
				}
			}

			TextParticle.Add(entity, Locale.Get("max_hp"), Math.Abs(Amount), true, Amount < 0);
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			
			Amount = settings["amount"].Int(1);
			GiveHp = settings["give_hp"].Bool(true);
			Set = settings["set"].Bool(false);
			Bomb = settings["bomb"].Bool(false);
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

			root.Checkbox("Set", "set", false);
			root.Checkbox("Bomb", "bomb", false);
		}
	}
}