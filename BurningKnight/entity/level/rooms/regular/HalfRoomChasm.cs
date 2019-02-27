using BurningKnight.entity.level.features;
using BurningKnight.entity.level.painters;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.regular {
	public class HalfRoomDefChasm : RegularRoomDef {
		public override void Paint(Level Level) {
			foreach (LDoor Door in Connected.Values()) Door.SetType(LDoor.Type.REGULAR);

			var F = Terrain.RandomFloor();
			Painter.Fill(Level, this, Terrain.WALL);
			Painter.Fill(Level, this, 1, Terrain.CHASM);
			Rect Rect = null;

			if (GetCurrentConnections(RoomDef.Connection.Top) > 0)
				Rect = new Rect(Left + 1, Top + 1, Right, Bottom - GetHeight() / 2);
			else if (GetCurrentConnections(RoomDef.Connection.Bottom) > 0)
				Rect = new Rect(Left + 1, Top + 1 + GetHeight() / 2, Right, Bottom);
			else if (GetCurrentConnections(RoomDef.Connection.Right) > 0)
				Rect = new Rect(Left + 1 + GetWidth() / 2, Top + 1, Right, Bottom);
			else if (GetCurrentConnections(RoomDef.Connection.Left) > 0) Rect = new Rect(Left + 1, Top + 1, Right - GetWidth() / 2, Bottom);

			if (Rect != null) {
				PaintTunnel(Level, F, true);
				Painter.Fill(Level, Rect, F);
			}
		}
	}
}