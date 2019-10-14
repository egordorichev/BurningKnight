using BurningKnight.assets.items;
using BurningKnight.level.entities;
using BurningKnight.state;
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
			
			for (var i = 0; i < Random.Int(1, Run.Depth); i++) {
				var item = Items.CreateAndAdd("bk:emerald", Level.Area);
				item.Center = GetRandomFreeCell() * 16 + new Vector2(Random.Float(-4, 4), Random.Float(-4, 4));
			}
		}
	}
}