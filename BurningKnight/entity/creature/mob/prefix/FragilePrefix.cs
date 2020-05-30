using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.util;
using Lens.entity;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.prefix {
	public class FragilePrefix : Prefix {
		private static Vector4 color = new Vector4(146 / 255f, 161 / 255f, 185 / 255f, 1);

		public override bool HandleEvent(Event e) {
			if (e is HealthModifiedEvent hme) {
				if (hme.Amount < 0) {
					AnimationUtil.Poof(Mob.Center, 1);
					var room = Mob.GetComponent<RoomComponent>().Room;

					if (room == null) {
						return base.HandleEvent(e);
					}

					Mob.Center = room.GetRandomFreeTile() * 16 + new Vector2(8, 8);
					AnimationUtil.Poof(Mob.Center, 1);
				}
			}
			
			return base.HandleEvent(e);
		}

		public override Vector4 GetColor() {
			return color;
		}
	}
}