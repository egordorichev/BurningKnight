using System.Linq;
using BurningKnight.level.entities;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;

namespace BurningKnight.level.rooms.connection {
	public class LibraryConnectionRoom : ConnectionRoom {
		public bool BottomHalf;
		private bool doorZero;
		
		public override void PaintFloor(Level level) {
			
		}

		public override void Paint(Level level) {
			var old = Painter.Clip;
			
			Painter.Clip = null;
			Painter.Fill(level, this, Tile.WallA);
			Painter.Clip = old;

			var doors = Connected.Values.ToArray();
			var rooms = Connected.Keys.ToArray();
			var spot = doorZero ? doors[0] : doors[1];

			var dot = new Dot(spot.X, spot.Y);
			
			if (dot.X == Left) {
				dot.X += 4;
			} else if (dot.Y == Top) {
				dot.Y += 4;
			} else if (dot.X == Right) {
				dot.X -= 4;
			} else if (dot.Y == Bottom) {
				dot.Y -= 4;
			}
			
			Painter.Fill(level, dot.X - 2, dot.Y - 2, 5, 5, Tiles.RandomFloor());
			Painter.Set(level, dot.X, dot.Y, Tile.FloorD);

			var t = new Teleporter {
				Id = "a"
			};
			
			level.Area.Add(t);
			t.Position = dot * 16;
		}

		private bool InvestigateDoor(RoomDef room, DoorPlaceholder cameFrom) {
			if (room is LibraryConnectionRoom && room != this) {
				return true;
			}
			
			foreach (var door in room.Connected) {
				if (door.Value != cameFrom) {
					return InvestigateDoor(door.Key, door.Value);
				}
			}

			return false;
		}

		public override void SetupDoors(Level level) {
			var pair = Connected.First();
			var doors = Connected.Values.ToArray();

			doorZero = !BottomHalf || InvestigateDoor(pair.Key, pair.Value);
			
			if (!doorZero) {
				doors[0].Type = DoorPlaceholder.Variant.Regular;
				doors[1].Type = DoorPlaceholder.Variant.Hidden;
			} else {
				doors[1].Type = DoorPlaceholder.Variant.Regular;
				doors[0].Type = DoorPlaceholder.Variant.Hidden;
			}
		}

		public override int GetMinWidth() {
			return 21;
		}

		public override int GetMaxWidth() {
			return 22;
		}

		public override int GetMinHeight() {
			return 21;
		}

		public override int GetMaxHeight() {
			return 22;
		}

		public override int GetMaxConnections(Connection Side) {
			if (Side == Connection.All) {
				return 2;
			}
			
			return base.GetMaxConnections(Side);
		}

		public override bool CanConnect(RoomDef r, Dot p) {
			if (p.X == Left + 1 || p.X == Right - 1 || p.Y == Top + 1 || p.Y == Bottom - 1) {
				return false;
			}
			
			return base.CanConnect(r, p);
		}
	}
}