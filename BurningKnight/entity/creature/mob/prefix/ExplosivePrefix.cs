using BurningKnight.assets;
using BurningKnight.entity.events;
using Lens.entity;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.prefix {
	public class ExplosivePrefix : Prefix {
		private static Vector4 color = new Vector4(138 / 255f, 72 / 255f, 54 / 255f, 1);
		
		public override bool HandleEvent(Event e) {
			if (e is DiedEvent) {
				var bomb = new Bomb(Mob);
				Mob.Area.Add(bomb);
				bomb.Center = Mob.Center;
			}
			
			return base.HandleEvent(e);
		}

		public override Vector4 GetColor() {
			return color;
		}
	}
}