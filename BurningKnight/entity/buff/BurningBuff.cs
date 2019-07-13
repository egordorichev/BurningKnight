using BurningKnight.assets.particle.custom;
using BurningKnight.entity.component;

namespace BurningKnight.entity.buff {
	public class BurningBuff : Buff {
		public const string Id = "bk:burning";

		public BurningBuff() : base(Id) {
			Infinite = true;
		}
		
		private float tillDamage;
		private float lastParticle;

		public override void Update(float dt) {
			base.Update(dt);

			lastParticle += dt;

			if (lastParticle >= 0.1f) {
				lastParticle = 0;

				Entity.Area.Add(new FireParticle {
					Owner = Entity
				});
			}

			tillDamage -= dt;

			if (tillDamage <= 0) {
				tillDamage = 1.3f;
				Entity.GetComponent<HealthComponent>().ModifyHealth(-1, Entity, false);
			}
		}
	}
}