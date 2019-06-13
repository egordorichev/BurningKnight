using System;
using BurningKnight.entity.creature.mob.boss;
using BurningKnight.entity.projectile;
using BurningKnight.entity.projectile.pattern;

namespace BurningKnight.entity.creature.bk.attacks {
	public class SpinningHellAttack : BossAttack<BurningKnight> {
		private const float Delay = 0.2f;
		private const float Total = 10f;
		
		private float sinceLast;
		
		public override void Update(float dt) {
			base.Update(dt);

			if (T >= Total) {
				Self.SelectAttack();
				return;
			}
			
			sinceLast += dt;

			if (sinceLast >= Delay) {
				sinceLast = 0;

				for (var i = 0; i < 4; i++) {
					/*var p = new ProjectilePattern(CircleProjectilePattern.Make(4.5f, 9)) {
						Position = Self.Center
					};

					for (var j = 0; j < 2; j++) {
						p.Add(Projectile.Make(Self, "small"));
					}
				
					p.Launch((float) ((float) i / 4 * Math.PI * 2 + T * 0.7f * (1 + T * 0.2f)), 40);
					Self.Area.Add(p);*/
					
					Projectile.Make(Self, "small", (float) ((float) i / 4 * Math.PI * 2 + T * 0.7f * (1 + T * 0.2f)), 6);
				}
			}
		}
	}
}