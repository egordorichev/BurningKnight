using BurningKnight.assets.items;
using BurningKnight.entity.creature.bk;
using BurningKnight.entity.creature.mob.boss;
using BurningKnight.entity.item.stand;
using BurningKnight.level.entities.decor;
using BurningKnight.level.rooms.granny;
using BurningKnight.level.rooms.oldman;
using BurningKnight.level.rooms.preboss;
using BurningKnight.level.tile;
using BurningKnight.save;
using BurningKnight.util.geometry;
using Lens;
using Lens.util;
using Microsoft.Xna.Framework;

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
			return 3;
		}

		public override int GetMinConnections(Connection Side) {
			if (Side == Connection.All) return 1;
			return 0;
		}

		public override bool CanConnect(RoomDef R) {
			if (R is GrannyRoom || R is OldManRoom || R is PrebossRoom) {
				return base.CanConnect(R);
			}

			return false;
		}

		public override bool CanConnect(RoomDef R, Dot P) {
			if (R is PrebossRoom) {
				var x = (int) P.X;
				var y = (int) P.Y;

				if ((x == Left || x == Right) && y != Top + GetHeight() / 2) {
					return false;
				}

				if ((y == Top || y == Bottom) && x != Left + GetWidth() / 2) {
					return false;
				}
			}

			return base.CanConnect(R, P);
		}

		public override void Paint(Level level) {
			PaintRoom(level);

			var boss = BossRegistry.Generate();

			if (boss == null) {
				Log.Error("Failed to generate the boss!");
				return;
			}
			
			level.Area.Add(boss);
			boss.Center = GetCenterVector();

			/*var trigger = new SpawnTrigger();
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
			c.Resize(1, 1);
			
			Painter.Fill(level, c, -3, Tiles.RandomFloor());
			PaintTunnel(level, Tile.FloorD, c);
			Painter.Fill(level, c, -2, Tiles.RandomFloor());

			var x = (c.Left - 2) * 16;
			var y = (c.Top - 2) * 16;
			
			var ta = new Torch();
			level.Area.Add(ta);
			ta.CenterX = x + 8;
			ta.Bottom = y + 12;
			
			ta = new Torch();
			level.Area.Add(ta);
			ta.CenterX = x + 4 * 16 + 8;
			ta.Bottom = y + 12;
			
			ta = new Torch();
			level.Area.Add(ta);
			ta.CenterX = x + 8;
			ta.Bottom = y + 4 * 16 + 12;

			ta = new Torch();
			level.Area.Add(ta);
			ta.CenterX = x + 4 * 16 + 8;
			ta.Bottom = y + 4 * 16 + 12;

			var st = new BkStand();
			level.Area.Add(st);

			st.CenterX = c.Left * 16 + 8;
			st.CenterY = c.Top * 16 + 8;
			st.SetItem(Items.CreateAndAdd("bk:the_key", level.Area), null);

			Painter.Fill(level, c, -2, Tile.FloorD);
			Painter.Fill(level, c, -1, Tiles.RandomFloor());*/
		}

		public override void SetupDoors(Level level) {
			foreach (var d in Connected.Values) {
				d.Type = DoorPlaceholder.Variant.Boss;
			}
		}

		public override Rect GetConnectionSpace() {
			return GetCenterRect();
		}
		
		protected virtual void PaintRoom(Level level) {
			
		}
	}
}