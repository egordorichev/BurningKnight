using BurningKnight.entity.component;
using BurningKnight.entity.creature.drop;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.prefix {
	public class GoldPrefix : Prefix {
		private static Vector4 color = new Vector4(1f, 200 / 255f, 37 / 255f, 1);

		public override void Init() {
			base.Init();
			Mob.GetComponent<DropsComponent>().Add(new SimpleDrop(1f, 1, 3, "bk:copper_coin"));
		}

		public override Vector4 GetColor() {
			return color;
		}
	}
}