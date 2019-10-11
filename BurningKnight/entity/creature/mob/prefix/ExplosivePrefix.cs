using BurningKnight.assets;
using BurningKnight.entity.events;
using Lens.entity;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.prefix {
	public class ExplosivePrefix : Prefix {
		public override bool HandleEvent(Event e) {
			if (e is DiedEvent) {
				var bomb = new Bomb();
				Mob.Area.Add(bomb);
				bomb.Center = Mob.Center;
			}
			
			return base.HandleEvent(e);
		}

		public override Color GetColor() {
			return Palette.Default[19]; // Brown
		}
	}
}