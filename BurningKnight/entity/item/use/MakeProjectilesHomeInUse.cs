using BurningKnight.entity.events;
using BurningKnight.entity.projectile.controller;
using BurningKnight.util;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class MakeProjectilesHomeInUse : ItemUse {
		private float speed;
		private bool better;
		
		public override bool HandleEvent(Event e) {
			if (e is ProjectileCreatedEvent pce) {
				if (better) {
					pce.Projectile.Controller += TargetProjectileController.MakeBetter(speed);
				} else {
					pce.Projectile.Controller += TargetProjectileController.Make(null, speed);
				}
			}
			
			return base.HandleEvent(e);
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			speed = settings["speed"].Number(1);
			better = settings["better"].Bool(false);
		}

		public static void RenderDebug(JsonValue root) {
			var speed = root["speed"].Number(1);

			if (ImGui.InputFloat("Speed", ref speed)) {
				root["speed"] = speed;
			}

			root.Checkbox("Better?", "better", false);
		}
	}
}