using BurningKnight.entity.room;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.trap {
	public class CageRoom : TrapRoom {
		public override void Paint(Level level) {
			var m = Random.Int(2, 5);
			
			Painter.Rect(level, this, m, Tile.PistonDown);

			var r = Shrink(m + 1);
			r.Right++;
			r.Bottom++;
			
			Painter.Fill(level, r, Tile.Plate);

			Painter.Set(level, new Dot(Random.Int(r.Left, r.Right), Random.Int(r.Top, r.Bottom)), Tile.FloorD);
		}

		public override void ModifyRoom(Room room) {
			base.ModifyRoom(room);
			room.AddController("bk:piston_activator");
		}
	}
}