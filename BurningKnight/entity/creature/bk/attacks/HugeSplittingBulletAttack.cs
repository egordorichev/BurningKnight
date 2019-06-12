using System;
using BurningKnight.entity.creature.mob.boss;
using BurningKnight.entity.projectile;

namespace BurningKnight.entity.creature.bk.attacks {
	public class HugeSplittingBulletAttack : BossAttack<BurningKnight> {
		private const float Delay = 3f;
		private const int SmallCount = 32;
		
		public override void Init() {
			base.Init();

			var bullet = Projectile.Make(Self, "huge", Self.AngleTo(Self.Target), 4);

			bullet.OnDeath = (p, t) => {
				for (var i = 0; i < SmallCount; i++) {
					var b = Projectile.Make(Self, "small", (float) (((float) i) / SmallCount * Math.PI * 2), 6, true, -1, p);
					b.Center = p.Center;
				}
			};

			bullet.CanBeReflected = false;
			bullet.CanBeBroken = false;
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (T >= Delay) {
				Self.SelectAttack();
			}
		}
	}
}