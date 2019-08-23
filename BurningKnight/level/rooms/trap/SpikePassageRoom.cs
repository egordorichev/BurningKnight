using BurningKnight.entity.room;
using BurningKnight.entity.room.input;
using BurningKnight.level.tile;
using Lens.util.math;

namespace BurningKnight.level.rooms.trap {
	public class SpikePassageRoom : TrapRoom {
		public override void Paint(Level level) {
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

			m += Random.Int(2, 5);

			if (Random.Chance()) {
				Painter.FillEllipse(level, this, m, Tiles.RandomFloor());
			} else {
				Painter.Fill(level, this, m, Tiles.RandomFloor());
			}

			var c = GetTileCenter();
			Painter.Set(level, c, Tiles.RandomFloor());
			
			var input = new Lever();
			input.Position = c * 16;
			level.Area.Add(input);
		}

		public override void ModifyRoom(Room room) {
			base.ModifyRoom(room);
			room.AddController("bk:spike_field");
		}
	}
}