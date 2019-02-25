using BurningKnight.core.entity.level.features;
using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class HalfRoomChasm : RegularRoom {
		public override Void Paint(Level Level) {
			foreach (LDoor Door in this.Connected.Values()) {
				Door.SetType(LDoor.Type.REGULAR);
			}

			byte F = Terrain.RandomFloor();
			Painter.Fill(Level, this, Terrain.WALL);
			Painter.Fill(Level, this, 1, Terrain.CHASM);
			Rect Rect = null;

			if (this.GetCurrentConnections(Room.Connection.TOP) > 0) {
				Rect = new Rect(this.Left + 1, this.Top + 1, this.Right, this.Bottom - this.GetHeight() / 2);
			} else if (this.GetCurrentConnections(Room.Connection.BOTTOM) > 0) {
				Rect = new Rect(this.Left + 1, this.Top + 1 + this.GetHeight() / 2, this.Right, this.Bottom);
			} else if (this.GetCurrentConnections(Room.Connection.RIGHT) > 0) {
				Rect = new Rect(this.Left + 1 + this.GetWidth() / 2, this.Top + 1, this.Right, this.Bottom);
			} else if (this.GetCurrentConnections(Room.Connection.LEFT) > 0) {
				Rect = new Rect(this.Left + 1, this.Top + 1, this.Right - this.GetWidth() / 2, this.Bottom);
			} 

			if (Rect != null) {
				this.PaintTunnel(Level, F, true);
				Painter.Fill(Level, Rect, F);
			} 
		}
	}
}
