using BurningKnight.entity.events;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class MakeProjectilesBounceUse : ItemUse {
		private int count;

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			count = settings["count"].Int(1);
		}

		public override bool HandleEvent(Event e) {
			if (e is ProjectileCreatedEvent pce) {
				pce.Projectile.Bounce += count;
			}
			
			return base.HandleEvent(e);
		}

		public static void RenderDebug(JsonValue root) {
			var count = root["count"].Int(1);

			if (ImGui.InputInt("Count", ref count)) {
				root["count"] = count;
			}
		}
	}
}