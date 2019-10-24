using BurningKnight.assets.items;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class SpawnBombUse : ConsumeUse {
		public float Timer;
		
		public override void Use(Entity entity, Item item) {
			Items.Unlock("bk:grenade_launcher");

			var bomb = new Bomb(entity, Timer);
			entity.Area.Add(bomb);
			bomb.Center = entity.Center;
			bomb.MoveToMouse();
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			Timer = settings["timer"].Number(3);
		}

		public static void RenderDebug(JsonValue root) {
			var val = (float) root["timer"].Int(3);

			if (ImGui.InputFloat("Timer", ref val)) {
				root["timer"] = val;
			}
		}
	}
}