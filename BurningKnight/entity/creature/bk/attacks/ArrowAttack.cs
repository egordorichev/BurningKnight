using BurningKnight.entity.creature.mob.boss;
using BurningKnight.entity.projectile;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.bk.attacks {
	public class ArrowAttack : BossAttack<BurningKnight> {
		private const float Delay = 2f;
		private int count;

		public override void Update(float dt) {
			base.Update(dt);

			if (T >= (count + 1) * Delay) {
				count++;

				if (count == 4) {
					Self.SelectAttack();
					return;
				}
				
				ProjectileTemplate.Make(Self, "green_small", Self.Center, Self.AngleTo(Self.Target), 5, 2, 0,
					p => p.AddLight(16f, Projectile.GreenLight),
					
					"  x ",
					"xxxx",
					"  x ");
			}
		}
	}
}