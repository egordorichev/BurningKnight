using System;
using BurningKnight.entity.creature.mob.boss;
using BurningKnight.entity.projectile;
using BurningKnight.entity.projectile.controller;

namespace BurningKnight.entity.creature.bk.attacks {
	public class AutoSkullAttack : BossAttack<BurningKnight> {
		private const float Delay = 2f;
		private int count;

		public override void Update(float dt) {
			base.Update(dt);

			if ((count + 1) * Delay <= T) {
				count++;

				if (count == 6) {
					Self.SelectAttack();
					return;
				}

				var skull = Projectile.Make(Self, "skull", Self.AngleTo(Self.Target), 10);

				skull.OnDeath += (p, t) => {
					if (!t) {
						return;
					}
					
					for (var i = 0; i < 8; i++) {
						var bullet = Projectile.Make(Self, "small", 
							((float) i) / 4 * (float) Math.PI, (i % 2 == 0 ? 2 : 1) * 2 + 1);
						
						bullet.Center = p.Center;
					}
				};

				skull.Controller += TargetProjectileController.Make(Self.Target, 0.5f);
				skull.Range = 5f;
				skull.IndicateDeath = true;
				skull.CanBeReflected = false;
				
				// todo: speed up a bit? start slow?
			}
		}
	}
}