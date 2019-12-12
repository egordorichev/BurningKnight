using BurningKnight.entity.component;
using BurningKnight.entity.events;
using Lens.util.math;

namespace BurningKnight.entity.creature.mob.jungle {
	public class BigBee : Bee {
		public override void Init() {
			base.Init();
			Speed = 0.25f;
		}

		protected override string GetAnimation() {
			return "bigbee";
		}

		protected override bool HandleDeath(DiedEvent d) {
			for (var i = 0; i < Rnd.Int(2, 5); i++) {
				var bee = new Bee();
				Area.Add(bee);
				bee.Center = Center;
			}
			
			return base.HandleDeath(d);
		}

		protected override void AddBody() {
			AddComponent(new RectBodyComponent(2, 17, 19, 1));		
			AddComponent(new SensorBodyComponent(2, 4, 19, 13));
		}
	}
}