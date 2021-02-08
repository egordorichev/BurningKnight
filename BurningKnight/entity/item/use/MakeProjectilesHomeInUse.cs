using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
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
				/* if (pce.Projectile is Laser l) {
					Timer.Add(() => {
						var room = pce.Owner.GetComponent<RoomComponent>().Room;
						var ent = room?.FindClosest(l.Position, Tags.MustBeKilled);

						if (ent == null) {
							return;
						}

						Log.Debug(ent.GetType().FullName);

						var ac = better ? 0.05f : 0.15f;

						l.PlayerRotated = false;
						l.BodyComponent.Body.Rotation = MathUtils.Angle(ent.Center.Y - l.Position.Y, ent.Center.X - l.Position.X);
					}, 0.05f);
				} else { */
					if (better) {
						ProjectileCallbacks.AttachUpdateCallback(pce.Projectile, TargetProjectileController.MakeBetter(speed));
					} else {
						ProjectileCallbacks.AttachUpdateCallback(pce.Projectile, TargetProjectileController.Make(null, speed));
					}
				// }
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