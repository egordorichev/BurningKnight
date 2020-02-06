using BurningKnight.entity.component;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.prefix {
	public class RegenerativePrefix : Prefix {
		private static Vector4 color = new Vector4(237f / 255f, 118 / 255f, 20 / 255f, 1);

		private float sinceLast;
		
		public override void Update(float dt) {
			base.Update(dt);

			sinceLast -= dt;
			
			if (sinceLast < 0) {
				sinceLast = 1;

				var health = Mob.GetComponent<HealthComponent>();

				if (!health.IsFull()) {
					health.ModifyHealth(1f, Mob);
				}
			}
		}

		public override Vector4 GetColor() {
			return color;
		}
	}
}