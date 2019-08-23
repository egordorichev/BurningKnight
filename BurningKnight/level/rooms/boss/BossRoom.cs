using BurningKnight.entity.creature.bk;
using BurningKnight.level.entities.decor;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens;

namespace BurningKnight.level.rooms.boss {
	public class BossRoom : RoomDef {
		public override int GetMinWidth() {
			return Display.Width / 16;
		}

		public override int GetMinHeight() {
			return Display.Width / 16;
		}

		public override int GetMaxWidth() {
			return Display.Width / 16 + 1;
		}

		public override int GetMaxHeight() {
			return Display.Width / 16 + 1;
		}

		public override int GetMaxConnections(Connection Side) {
			return 1;
		}

		public override int GetMinConnections(Connection Side) {
			if (Side == Connection.All) return 1;
			return 0;
		}

		public override void Paint(Level level) {
			PaintRoom(level);
			
			var trigger = new SpawnTrigger();
			var w = GetWidth() - 2;
			var h = GetHeight() - 2;
			var s = w * h;
			
			trigger.Tiles = new byte[s];
			trigger.Liquid = new byte[s];
			trigger.RoomX = (ushort) (Left + 1);
			trigger.RoomY = (ushort) (Top + 1);
			trigger.RoomWidth = (byte) w;
			trigger.RoomHeight = (byte) h;

			for (var y = 0; y < h; y++) {
				for (var x = 0; x < w; x++) {
					var li = level.ToIndex(Left + 1 + x, Top + 1 + y);
					var i = x + y * w;

					trigger.Tiles[i] = level.Tiles[li];
					trigger.Liquid[i] = level.Liquid[li];
				}
			}
			
			Painter.Fill(level, this, 1, Tile.WallA);

			var c = GetCenterRect();
			
			Painter.Fill(level, c, -3, Tiles.RandomFloor());

			PaintTunnel(level, Tiles.RandomFloor());
			
			trigger.X = (c.Left - 2) * 16;
			trigger.Y = (c.Top - 2) * 16;
			trigger.Width = 5 * 16;
			trigger.Height = 5 * 16;

			level.Area.Add(trigger);
			
			var ta = new Torch();
			level.Area.Add(ta);
			ta.CenterX = trigger.X + 8;
			ta.Bottom = trigger.Y + 12;
			
			ta = new Torch();
			level.Area.Add(ta);
			ta.CenterX = trigger.X + 4 * 16 + 8;
			ta.Bottom = trigger.Y + 12;
			
			ta = new Torch();
			level.Area.Add(ta);
			ta.CenterX = trigger.X + 8;
			ta.Bottom = trigger.Y + 4 * 16 + 12;

			ta = new Torch();
			level.Area.Add(ta);
			ta.CenterX = trigger.X + 4 * 16 + 8;
			ta.Bottom = trigger.Y + 4 * 16 + 12;

			Painter.Fill(level, c, -2, Tile.FloorD);
			Painter.Fill(level, c, -1, Tiles.RandomFloor());
		}

		public override Rect GetConnectionSpace() {
			return GetCenterRect();
		}
		
		protected virtual void PaintRoom(Level level) {
			
		}
	}
}