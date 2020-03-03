using BurningKnight.level.rooms.trap;
using BurningKnight.level.tile;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.biome {
	public class DesertBiome : Biome {
		public DesertBiome() : base("Believer", Biome.Desert, "desert_biome", new Color(28, 18, 28)) {
			
		}

		public override void ModifyPainter(Painter painter) {
			base.ModifyPainter(painter);
			
			painter.Water = 0;
			painter.Grass = 0;
			painter.Dirt = 0.45f;
			
			painter.Modifiers.Add((l, rm, x, y) => {
				if (l.Get(x, y, true) == Tile.Dirt) {
					l.Set(x, y, Tile.Sand);
				}
			});

			painter.Modifiers.Add((l, rm, x, y) => {
				if (rm is TrapRoom) {
					return;
				}
				
				var r = (byte) (Tile.Chasm);
				
				if (l.Get(x, y, true) == Tile.Lava) {
					var i = l.ToIndex(x, y);
					
					l.Liquid[i] = 0;
					l.Tiles[i] = r;
				}
			});
		}
		
		/*public override void ModifyRooms(List<RoomDef> rooms) {
			base.ModifyRooms(rooms);
			rooms.Add(new DesertWellRoom());
		}*/
	}
}