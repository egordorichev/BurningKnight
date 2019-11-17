using BurningKnight.assets.items;
using BurningKnight.entity.component;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.item.use {
	public class SpawnBombUse : ConsumeUse {
		public float Timer;
		public int Amount;
		public bool Randomly;
		
		public override void Use(Entity entity, Item item) {
			Items.Unlock("bk:grenade_launcher");

			var room = entity.GetComponent<RoomComponent>().Room;

			for (var i = 0; i < Amount; i++) {
				if (Randomly) {
					Lens.util.timer.Timer.Add(() => {
						if (entity?.Area == null) {
							return;
						}
							
						var bomb = new Bomb(entity, Timer);
						entity.Area.Add(bomb);
						bomb.Center = room == null ? entity.Center + Rnd.Vector(-4, 4) : room.GetRandomFreeTile() * 16 + new Vector2(8);
					}, i * 0.1f);
				} else {
					if (entity?.Area == null) {
						return;
					}
					
					var bomb = new Bomb(entity, Timer);
					entity.Area.Add(bomb);
					
					bomb.Center = entity.Center;
					bomb.MoveToMouse();
				}
			}
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			
			Timer = settings["timer"].Number(3);
			Amount = settings["amount"].Int(1);
			Randomly = settings["randomly"].Bool(false);
		}

		public static void RenderDebug(JsonValue root) {
			var val = root["timer"].Number(3);

			if (ImGui.InputFloat("Timer", ref val)) {
				root["timer"] = val;
			}
			
			var am = root["amount"].Int(1);

			if (ImGui.InputInt("Amount", ref am)) {
				root["amount"] = am;
			}

			var randomly = root["randomly"].Bool(false);

			if (ImGui.Checkbox("Randomly", ref randomly)) {
				root["randomly"] = randomly;
			}
		}
	}
}