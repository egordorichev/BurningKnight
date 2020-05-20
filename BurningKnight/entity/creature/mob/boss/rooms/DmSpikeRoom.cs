using BurningKnight.entity.creature.player;
using BurningKnight.entity.room;
using BurningKnight.level;
using BurningKnight.level.tile;
using BurningKnight.state;
using BurningKnight.util.geometry;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.boss.rooms {
	public class DmSpikeRoom : DmRoom {
		public override void PlaceMage(Room room, DM mage) {
			var w = new Dot(Rnd.Int(Left + 1, Right), Top + 2);
			
			Painter.Fill(Run.Level, new Rect().Setup(w.X - 1, w.Y - 1, 3, 2), Tile.FloorA);
			
			mage.BottomCenter = w * 16 + new Vector2(8, -8);
			Painter.DrawLine(Run.Level, new Dot(w.X - 1, w.Y), new Dot(w.X + 1, w.Y), Tile.WallA);
		}

		public override void PlacePlayer(Room room, Player player) {
			var w = new Dot(Rnd.Int(Left + 1, Right), Bottom - 1);
			
			Painter.Fill(Run.Level, new Rect().Setup(w.X - 1, w.Y - 1, 3, 2), Tile.FloorA);
			player.BottomCenter = w * 16 + new Vector2(8);
		}

		public override void Paint(Level level, Room room) {
			Painter.Fill(level, this, 1, Tile.SpikeOffTmp);
			
			room.AddController("bk:spike_field");
			room.Generate();
		}

		public override int GetMinWidth() {
			return 12;
		}

		public override int GetMinHeight() {
			return 15;
		}

		public override int GetMaxWidth() {
			return 17;
		}

		public override int GetMaxHeight() {
			return 23;
		}
	}
}