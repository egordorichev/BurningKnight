using BurningKnight.entity.component;
using BurningKnight.entity.events;
using Lens.entity;

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
			GetAnyComponent<BodyComponent>().KnockbackFrom(d.From, 2f);
			ExplosionMaker.Make(this, 16);

			return base.HandleDeath(d);
		}

		protected override void OnHit(Entity e) {
			// No slowness debuffs
		}

		protected override void AddBody() {
			AddComponent(new RectBodyComponent(2, 9, 12, 1));		
			AddComponent(new SensorBodyComponent(2, 3, 10, 10));
		}
	}
}