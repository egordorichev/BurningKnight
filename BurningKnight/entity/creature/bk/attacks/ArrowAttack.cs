using BurningKnight.entity.creature.mob.boss;
using BurningKnight.entity.projectile;

namespace BurningKnight.entity.creature.bk.attacks {
	public class ArrowAttack : BossAttack<BurningKnight> {
		public override void Init() {
			ProjectileTemplate.Make(Self, "small", Self.Center, Self.AngleTo(Self.Target), 10, 2,
				"   x  ",
				"    x ",
				"xxxxxx",
				"    x ",
				"   x  ");
		}
	}
}