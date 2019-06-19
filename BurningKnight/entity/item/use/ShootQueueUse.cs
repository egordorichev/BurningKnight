using ImGuiNET;
using Lens.entity;
using Lens.lightJson;
using Lens.util.camera;
using Lens.util.timer;

namespace BurningKnight.entity.item.use {
	public class ShootQueueUse : SimpleShootUse {
		private int amount;
		private float delay;
		
		public override void Use(Entity entity, Item item) {
			for (var i = 0; i < amount; i++) {
				if (i == 0) {
					SpawnProjectile(entity, item);
				} else {
					Timer.Add(() => SpawnProjectile(entity, item), i * delay);
				}
			}

			Camera.Instance.ShakeMax(1.5f);
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);

			amount = settings["amn"].Int(3);
			delay = settings["dl"].Number(0.1f);
		}

		public static void RenderDebug(JsonValue root) {
			SimpleShootUse.RenderDebug(root);
			
			var amount = root["amn"].Int(3);

			if (ImGui.InputInt("Projectile Count", ref amount)) {
				root["amn"] = amount;
			}
			
			var delay = root["dl"].Number(0.1f);
			
			if (ImGui.InputFloat("Projectile Delay", ref delay)) {
				root["dl"] = delay;
			}
		}
	}
}