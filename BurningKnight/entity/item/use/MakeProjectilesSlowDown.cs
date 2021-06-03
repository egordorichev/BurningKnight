using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using BurningKnight.entity.projectile.controller;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class MakeProjectilesSlowDown : ItemUse {
		private float amount;
		private float time;

		public override bool HandleEvent(Event e) {
			if (e is ProjectileCreatedEvent pce) {
				ProjectileCallbacks.AttachUpdateCallback(pce.Projectile,  SlowdownProjectileController.Make(amount, time));
			}
			
			return base.HandleEvent(e);
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			amount = settings["amount"].Number(1);
			time = settings["time"].Number(1);
		}
		
		public static void RenderDebug(JsonValue root) {
			var val = root["amount"].Number(1);

			if (ImGui.InputFloat("Speed", ref val)) {
				root["amount"] = val;
			}
			
			val = root["time"].Number(1);

			if (ImGui.InputFloat("Time", ref val)) {
				root["time"] = val;
			}
		}
	}
}