using BurningKnight.entity.events;

namespace BurningKnight.entity.creature.mob.jungle {
	public class Explobee : Bee {
		public override void Init() {
			base.Init();
			Speed = 0.5f;
		}

		protected override string GetAnimation() {
			return "explobee";
		}

		protected override bool HandleDeath(DiedEvent d) {
			ExplosionMaker.Make(this);
			return base.HandleDeath(d);
		}
	}
}