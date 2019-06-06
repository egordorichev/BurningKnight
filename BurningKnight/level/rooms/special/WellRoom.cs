using BurningKnight.level.entities;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.special {
	public class WellRoom : SpecialRoom {
		public override void Paint(Level level) {
			var well = new Well();
			level.Area.Add(well);
			well.Center = GetCenter() * 16 + new Vector2(Random.Float(-8, 8), Random.Float(-8, 8));
		}
	}
}