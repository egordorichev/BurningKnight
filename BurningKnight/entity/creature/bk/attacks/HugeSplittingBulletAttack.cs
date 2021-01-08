using System;
using BurningKnight.entity.creature.mob.boss;
using BurningKnight.entity.projectile;
using BurningKnight.entity.projectile.pattern;
using BurningKnight.util;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.bk.attacks {
	public class HugeSplittingBulletAttack : BossAttack<BurningKnight> {
		private const float Delay = 3f;
		private const int SmallCount = 16;
		private const int OrbiterCount = 8;
		
		public override void Init() {
			base.Init();

			var p = new ProjectilePattern(CircleWithCenterProjectilePattern.Make(14f, 3)) {
				Position = Self.Center
			};
			
			var bullet = Projectile.Make(Self, "huge", Self.AngleTo(Self.Target), 7);

			bullet.OnDeath = (pr, e, t) => {
				p.Kill();
				ExplosionMaker.Make(pr.Owner, 16, false, new Vec2(pr.Center));
				
				for (var i = 0; i < SmallCount; i++) {
					var b = Projectile.Make(Self, "green_small", (float) (((float) i) / SmallCount * Math.PI * 2), 6, true, 1, pr);
					b.Center = pr.Center;
					b.AddLight(32f, ProjectileColor.Green);
				}
			};

			bullet.CanBeReflected = false;
			bullet.CanBeBroken = false;
				
			p.Add(bullet);

			for (var j = 0; j < OrbiterCount; j++) {
				var b = Projectile.Make(Self, "green_small");

				b.CanBeBroken = false;
				b.CanBeReflected = false;
				
				b.AddLight(32f, ProjectileColor.Green);
				p.Add(b);
			}
				
			p.Launch(Self.AngleTo(Self.Target), 20);
			Self.Area.Add(p);
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (T >= Delay) {
				Self.SelectAttack();
			}
		}
	}
}