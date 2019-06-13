using System;
using BurningKnight.entity.creature.mob.boss;
using BurningKnight.entity.projectile;
using BurningKnight.entity.projectile.controller;

namespace BurningKnight.entity.creature.bk.attacks {
	public class AutoWallAttack : BossAttack<BurningKnight> {
		private const float Delay = 0.2f;
		private const int SmallCount = 16;

		private int count;
		private float a;

		public override void Update(float dt) {
			base.Update(dt);

			if (T >= Delay * (count + 1)) {
				count++;

				if (count == SmallCount + 1) {
					Self.SelectAttack();
					return;
				}
				
				var b = Projectile.Make(Self, "small", Self.AngleTo(Self.Target), 10);
				
				b.Center = Self.Center;
				b.Range = 5f;
				b.Controller += TargetProjectileController.Make(Self.Target, 0.5f);
			}
		}
	}
}