using BurningKnight.entity.room;
using BurningKnight.level.tile;
using Lens.util.math;

namespace BurningKnight.level.rooms.trap {
	public class SpikePassageRoom : TrapRoom {
		public override void Paint(Level level) {
			base.Paint(level);

			var m = 1;

			if (Random.Chance()) {
				Painter.Fill(level, this, m, Tile.Chasm);
				m += Random.Int(1, 3);
				Painter.Fill(level, this, m, Tiles.RandomFloor());

				if (Random.Chance()) {
					PaintTunnel(level, Tiles.RandomFloor(), GetCenterRect(), true);
				}
				
				PaintTunnel(level, Tiles.RandomFloor(), GetCenterRect());
			}

			if (Random.Chance()) {
				Painter.FillEllipse(level, this, m, Tile.SpikeTmp);
			} else {
				Painter.Fill(level, this, m, Tile.SpikeTmp);
			}

			if (Random.Chance()) {
				m += Random.Int(2, 5);
				var f = Tiles.Pick(Tile.Chasm, Tile.WallA);

				if (Random.Chance()) {
					Painter.FillEllipse(level, this, m, f);
				} else {
					Painter.Fill(level, this, m, f);
				}
			}
		}

		public override void ModifyRoom(Room room) {
			base.ModifyRoom(room);
			room.AddController("bk:spike_field");
		}
	}
}