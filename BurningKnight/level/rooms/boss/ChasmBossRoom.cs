using BurningKnight.entity.creature.bk;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;

namespace BurningKnight.level.rooms.boss {
	public class ChasmBossRoom : BossRoom {
		public override void Paint(Level level) {
			Painter.Fill(level, this, 1, Tile.WallA);

			var c = GetCenterRect();
			
			Painter.Fill(level, c, -3, Tiles.RandomFloor());

			PaintTunnel(level, Tiles.RandomFloor());
			
			var trigger = new SpawnTrigger();
			trigger.X = (c.Left - 2) * 16;
			trigger.Y = (c.Top - 2) * 16;
			trigger.Width = 5 * 16;
			trigger.Height = 5 * 16;

			level.Area.Add(trigger);
			
			Painter.Fill(level, c, -2, Tile.FloorD);
			Painter.Fill(level, c, -1, Tiles.RandomFloor());
		}

		public override Rect GetConnectionSpace() {
			return GetCenterRect();
		}
	}
}