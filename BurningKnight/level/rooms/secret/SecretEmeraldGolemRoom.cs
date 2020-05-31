using BurningKnight.entity.creature.npc;
using BurningKnight.level.walls;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.secret {
	public class SecretEmeraldGolemRoom : SecretRoom {
		public override void Paint(Level level) {
			new EllipseWalls().Paint(level, this, Shrink());
			
			var golem = new EmeraldGolem();
			level.Area.Add(golem);
			golem.BottomCenter = GetCenter() * 16 + new Vector2(8, 0);
		}

		public override void PaintFloor(Level level) {
			
		}
	}
}