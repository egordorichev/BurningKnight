using BurningKnight.entity.creature.mob;
using Lens.entity.component;
using Lens.input;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.component {
	public class AimComponent : Component {
		public enum AimType {
			Cursor,
			Target,
			AnyPlayer
		}

		public AimComponent(AimType t) {
			TheType = t;
		}

		public AimType TheType;
		public Vector2 Aim;

		public override void Update(float dt) {
			base.Update(dt);

			if (TheType == AimType.Cursor) {
				Aim = Input.Mouse.GamePosition;
			} else if (TheType == AimType.Target) {
				Aim = ((Mob) Entity).Target?.Center ?? Input.Mouse.GamePosition;
			} else {
				var a = GetComponent<RoomComponent>().Room.Tagged[Tags.Player];

				if (a.Count > 0) {
					Aim = a[0].Center;
				}
			}
		}
	}
}