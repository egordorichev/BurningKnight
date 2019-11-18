using BurningKnight.entity.creature.mob.boss;
using BurningKnight.entity.projectile;
using Lens.util.math;

namespace BurningKnight.entity.creature.bk.attacks {
	public class ShootAttack : BossAttack<BurningKnight> {
		private const float TotalTime = 10f;
		private const float Delay = 0.2f;

		private float sinceLast;
		
		public override void Update(float dt) {
			base.Update(dt);

			if (T >= TotalTime) {
				Self.SelectAttack();
				return;
			}

			Self.FlyTo(Self.Target.Center, 20f);
			sinceLast += dt;

			if (sinceLast >= Delay) {
				sinceLast = 0;
				Projectile.Make(Self, Rnd.Chance(30) ? "tiny" : "small", Self.AngleTo(Self.Target) + Rnd.Float(-0.5f, 0.5f), Rnd.Float(8, 15));
			}
		}
	}
}