using BurningKnight.entity.creature.mob.boss;
using BurningKnight.entity.projectile;
using Lens.util;
using Lens.util.math;

namespace BurningKnight.entity.creature.bk.attacks {
	public class AutoSkullAttack : BossAttack<BurningKnight> {
		public override void Init() {
			base.Init();
			
			ProjectileTemplate.Make(Self, "small", Self.Center, Random.AnglePI(), 30, 2,
				"  x  ",
				" xxx ",
				"xxxxx",
				"  x  ",
				"  x  ",
				"  x  ");
		}
	}
}