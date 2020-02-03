using BurningKnight.entity.events;
using Lens.entity;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.prefix {
	public class FatPrefix : Prefix {
		private static Vector4 color = new Vector4(0, 152 / 255f, 220 / 255f, 1);

		public override bool HandleEvent(Event e) {
			if (e is HealthModifiedEvent hme) {
				if (hme.Amount < 0) {
					hme.Amount *= 0.5f;
				}
			}
			
			return base.HandleEvent(e);
		}

		public override Vector4 GetColor() {
			return color;
		}
	}
}