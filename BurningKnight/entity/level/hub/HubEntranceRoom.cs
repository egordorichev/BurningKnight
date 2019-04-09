using BurningKnight.entity.level.rooms.entrance;
using BurningKnight.entity.level.tile;
using BurningKnight.util.geometry;
using Lens.util;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.level.hub {
	public class HubEntranceRoom : EntranceRoom {
		public HubEntranceRoom() {
			IgnoreEntranceRooms = true;
		}

		public override void PaintFloor(Level level) {
			
		}

		public override void Paint(Level level) {
			Painter.Prefab(level, "entrance", Left, Top);
			PaintTunnel(level, Tile.FloorA, new Rect(GetCenter()), false, false);
		}
		
		protected override void Fill(Level level) {
			
		}

		public override int GetMinWidth() {
			return 7;
		}

		public override int GetMinHeight() {
			return 7;
		}

		public override int GetMaxWidth() {
			return 8;
		}

		public override int GetMaxHeight() {
			return 8;
		}

		public override bool CanConnect(Vector2 p) {
			return ((int) p.X) == Right && ((int) p.Y) == Top + 3;
		}
	}
}