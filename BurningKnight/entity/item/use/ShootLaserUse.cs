using System.Collections.Generic;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using Lens.entity;
using Lens.lightJson;
using Lens.util;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.item.use {
	public class ShootLaserUse : ShootUse {
		private List<Laser> lasers = new List<Laser>();

		public override void Update(Entity entity, Item item, float dt) {
			base.Update(entity, item, dt);

			if (lasers.Count > 0) {
				for (var i = lasers.Count - 1; i >= 0; i--) {
					var laser = lasers[i];
					
					if (laser.Done) {
						lasers.RemoveAt(i);
						return;
					}
				
					var aim = entity.GetComponent<AimComponent>();
					var from = aim.Center;
					var am = aim.RealAim;

					laser.Position = from;
				}
			}
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);

			var ac = 0.1f;

			SpawnProjectile = (entity, item) => {
				var aim = entity.GetComponent<AimComponent>();
				var from = aim.Center;
				var am = aim.RealAim;
				var a = MathUtils.Angle(am.X - from.X, am.Y - from.Y);
				
				var cnt = 1;
				var accurate = false;

				if (entity is Player pl) {
					var e = new PlayerShootEvent {
						Player = pl
					};

					entity.HandleEvent(e);

					cnt += e.Times - 1;
					accurate = e.Accurate;
				}

				for (var i = 0; i < cnt; i++) {
					var addition = 0f;
					
					if (accurate) {
						addition = (i - (int) (cnt * 0.5f)) * 0.2f + Rnd.Float(-ac / 2f, ac / 2f);
					} else if (cnt == 1 || i > 0) {
						addition = Rnd.Float(-ac / 2f, ac / 2f);
					}
					
					var laser = Laser.Make(entity, a, addition, item, damage: (item.Scourged ? 1.5f : 1f));

					laser.Position = from;
					laser.Color = ProjectileColor.Red;
					laser.Recalculate();
					
					lasers.Add(laser);
				}
			};
		}
	}
}