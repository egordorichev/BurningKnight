using System;
using BurningKnight.entity.creature.mob.boss;
using BurningKnight.entity.projectile;
using BurningKnight.entity.projectile.pattern;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.creature.bk.attacks {
	public class MissileAttack : BossAttack<BurningKnight> {
		private const float Delay = 5f;
		private const int SmallCount = 8;
		private const int InnerCount = 16;
		
		private int count;

		public override void Update(float dt) {
			base.Update(dt);

			if ((count + 1) * Delay <= T) {
				count++;

				if (count == 4) {
					Self.SelectAttack();
					return;
				}

				var m = new Missile(Self, Self.Target);
				m.AddLight(32f, Projectile.RedLight);

				m.OnDeath += (p, t) => {
					for (var i = 0; i < SmallCount; i++) {
						var an = (float) (((float) i) / SmallCount * Math.PI * 2);
						
						var pp = new ProjectilePattern(CircleProjectilePattern.Make(4.5f, 10 * (i % 2 == 0 ? 1 : -1))) {
							Position = p.Center
						};

						for (var j = 0; j < 2; j++) {
							var b = Projectile.Make(Self, "small");
							pp.Add(b);
							b.Range = 2f;
							b.AddLight(32f, Projectile.RedLight);
						}
				
						pp.Launch(an, 40);
						Self.Area.Add(pp);
					}

					var a = p.AngleTo(Self.Target);
					
					for (var i = 0; i < InnerCount; i++) {
						var s = Random.Chance(40);
						var b = Projectile.Make(Self, s ? "green_tiny" : "green_small", a + Random.Float(-0.3f, 0.3f), Random.Float(2, 12), true, 1, p);
						
						b.Center = p.Center;
						b.AddLight(s ? 16f : 32f, Projectile.GreenLight);
					}
				};
			}
		}
	}
}