using System;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.item.use {
	public class ModifyProjectilesUse : ItemUse {
		public float Scale;
		public float Damage;
		public float Chance;
		public bool EventCreated = true;

		public override void Use(Entity entity, Item item) {
			
		}

		public override bool HandleEvent(Event e) {
			if (EventCreated && e is ProjectileCreatedEvent pce) {
				ModifyProjectile(pce.Projectile);
			}
			
			return base.HandleEvent(e);
		}

		public void ModifyProjectile(Projectile projectile) {
			if (Random.Float() > Chance) {
				return;
			}
			
			projectile.Scale *= Scale;
			projectile.Damage = (int) Math.Round(projectile.Damage * Damage);
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			
			Chance = settings["chance"].Number(1);
			Scale = settings["amount"].Number(1);
			Damage = settings["damage"].Number(1);
		}
		
		public static void RenderDebug(JsonValue root) {
			var val = root["chance"].Number(1);

			if (ImGui.InputFloat("Chance", ref val)) {
				root["chance"] = val;
			}

			val = root["amount"].Number(1);

			if (ImGui.InputFloat("Scale Modifier", ref val)) {
				root["amount"] = val;
			}
			
			val = root["damage"].Number(1);

			if (ImGui.InputFloat("Damage Modifier", ref val)) {
				root["damage"] = val;
			}
		}
	}
}