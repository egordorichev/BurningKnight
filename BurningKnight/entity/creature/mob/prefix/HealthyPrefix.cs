using BurningKnight.entity.component;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.prefix {
	public class HealthyPrefix : Prefix {
		private static Vector4 color = new Vector4(255 / 255f, 0, 64 / 255f, 1);

		public override void Init() {
			base.Init();

			var health = Mob.GetComponent<HealthComponent>();
			health.InitMaxHealth = health.MaxHealth * 2;
		}

		public override Vector4 GetColor() {
			return color;
		}
	}
}