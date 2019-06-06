using BurningKnight.level.entities;
using Lens.entity;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.secret {
	public class SecretMachineRoom : SecretRoom {
		public override void Paint(Level Level) {
			base.Paint(Level);

			Entity prop;
			var r = Random.Int(2);

			if (r == 0) {
				prop = new VendingMachine();
			} else {
				prop = new RerollMachine();
			}

			Level.Area.Add(prop);
			prop.Center = GetCenter() * 16 + new Vector2(Random.Float(-8, 8), Random.Float(-8, 8));
		}
	}
}