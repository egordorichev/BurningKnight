using BurningKnight.entity.creature.mob;
using Lens.entity.component;
using Lens.input;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.component {
	public class AimComponent : Component {
		public enum AimType {
			Cursor,
			Target
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
			} else {
				Aim = ((Mob) Entity).Target?.Center ?? Input.Mouse.GamePosition;
			}
		}
	}
}