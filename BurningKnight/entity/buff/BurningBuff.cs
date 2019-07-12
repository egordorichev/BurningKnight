using BurningKnight.entity.component;

namespace BurningKnight.entity.buff {
	public class BurningBuff : Buff {
		public const string Id = "bk:burning";

		public BurningBuff() : base(Id) {
			Infinite = true;
		}
		
		private float tillDamage;

		public override void Update(float dt) {
			base.Update(dt);

			tillDamage -= dt;

			if (tillDamage <= 0) {
				tillDamage = 1.3f;
				Entity.GetComponent<HealthComponent>().ModifyHealth(-1, Entity, false);
			}
		}
	}
}