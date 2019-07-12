using BurningKnight.entity.component;

namespace BurningKnight.entity.buff {
	public class BurningBuff : Buff {
		public const string Id = "bk:burning";
		public BurningBuff() : base(Id) {}
		
		private float lastHurt;
		
		public override void Update(float dt) {
			base.Update(dt);

			lastHurt += dt;

			if (lastHurt >= 0.5f) {
				lastHurt = 0;
				Entity.GetComponent<HealthComponent>().ModifyHealth(-1, WhoGave);
			}
		}
	}
}