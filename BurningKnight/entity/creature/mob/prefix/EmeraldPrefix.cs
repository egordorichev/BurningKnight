using BurningKnight.entity.component;
using BurningKnight.entity.creature.drop;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.prefix {
	public class EmeraldPrefix : Prefix {
		private static Vector4 color = new Vector4(0, 220 / 255f, 152 / 255f, 1);

		public override void Init() {
			base.Init();
			Mob.GetComponent<DropsComponent>().Add(new SimpleDrop(1f, 2, 5, "bk:emerald"));
		}

		public override Vector4 GetColor() {
			return color;
		}
	}
}