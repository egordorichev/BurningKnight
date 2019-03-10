using Lens.util.math;

namespace BurningKnight.entity.level.rooms.connection {
	public class TunnelRoom : ConnectionRoom {
		protected void Fill(Level Level) {
			
		}

		public override void Paint(Level Level) {
			Fill(Level);
			var Fl = Random.Chance(25) ? Random.Chance(33) ? Tile.Chasm 
				: Random.Chance(50) ? Tiles.RandomWall() : Tile.Lava : Tiles.RandomFloor();

			if (GetWidth() > 4 && GetHeight() > 4 && Random.Chance(50)) {
				PaintTunnel(Level, Fl, true);
			}

			if (Fl == Tile.Lava) {
				PaintTunnel(Level, Tiles.RandomFloor());
			}

			PaintTunnel(Level, Fl.Matches(Tile.Dirt, Tile.Lava) ? Random.Chance(50) ? 
				Tile.Water : Tile.Dirt : Tiles.RandomFloor());


			foreach (var Door in Connected.Values) {
				Door.Type = DoorPlaceholder.Variant.Tunnel;
			}	
		}
	}
}