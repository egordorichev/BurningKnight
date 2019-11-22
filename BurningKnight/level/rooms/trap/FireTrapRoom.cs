using BurningKnight.level.tile;

namespace BurningKnight.level.rooms.trap {
	public class FireTrapRoom : TrapRoom {
		public override void Paint(Level level) {
			Painter.Fill(level, this, 1, Tile.FireTrapTmp);
			var t = Tiles.RandomFloor();
			
			foreach (var d in Connected.Values) {
				if (d.Type != DoorPlaceholder.Variant.Secret) {
					var w = 2;
					var h = 2;
					
					Painter.Fill(level, d.X - w, d.Y - w, w * 2 + 1, h * 2 + 1, t);
				}
			}
		}
	}
}