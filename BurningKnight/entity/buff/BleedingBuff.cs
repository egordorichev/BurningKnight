using BurningKnight.assets.particle.custom;
using BurningKnight.entity.component;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.buff {
	public class BleedingBuff : Buff {
		public static Vector4 Color = new Vector4(0.5f, 0f, 0f, 1f);
		public const string Id = "bk:bleeding";
		public const float Delay = 0.5f;
		private float tillDamage = Delay;

		public BleedingBuff() : base(Id) {
			
		}

		public override void Update(float dt) {
			base.Update(dt);

			tillDamage -= dt;

			if (tillDamage <= 0) {
				tillDamage = Delay;
				Entity.GetComponent<HealthComponent>().ModifyHealth(-1, Entity);
			}
		}

		public override string GetIcon() {
			return "blood";
		}
	}
}