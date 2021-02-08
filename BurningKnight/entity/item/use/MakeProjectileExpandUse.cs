using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using BurningKnight.entity.projectile.controller;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class MakeProjectileExpandUse : ItemUse {
		private float speed;
		
		public override bool HandleEvent(Event e) {
			if (e is ProjectileCreatedEvent pce) {
				ProjectileCallbacks.AttachUpdateCallback(pce.Projectile, ExpandProjectileController.Make(speed));
			}
			
			return base.HandleEvent(e);
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			speed = settings["speed"].Number(1);
		}

		public static void RenderDebug(JsonValue root) {
			var speed = root["speed"].Number(1);

			if (ImGui.InputFloat("Speed", ref speed)) {
				root["speed"] = speed;
			}
		}
	}
}