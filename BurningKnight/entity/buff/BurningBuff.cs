using BurningKnight.assets.particle.custom;
using BurningKnight.entity.component;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.buff {
	public class BurningBuff : Buff {
		public static Vector4 Color = new Vector4(0.5f, 0f, 0f, 1f);
		public const string Id = "bk:burning";
		public const float Delay = 1f;
		
		private float tillDamage = Delay;
		private float lastParticle;

		public BurningBuff() : base(Id) {
			Infinite = true;
		}

		public override void Init() {
			base.Init();
			Entity.GetComponent<BuffsComponent>().Remove<FrozenBuff>();
		}

		public override void Update(float dt) {
			base.Update(dt);

			lastParticle += dt;

			if (lastParticle >= 0.5f) {
				lastParticle = 0;

				Entity.Area.Add(new FireParticle {
					Owner = Entity
				});
			}

			tillDamage -= dt;

			if (tillDamage <= 0) {
				tillDamage = Delay;
				Entity.GetComponent<HealthComponent>().ModifyHealth(-1, Entity);
			}
		}
	}
}