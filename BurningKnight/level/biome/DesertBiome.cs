using System.Collections.Generic;
using BurningKnight.level.rooms;
using BurningKnight.level.rooms.special;
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
		}

		public override void ModifyRooms(List<RoomDef> rooms) {
			base.ModifyRooms(rooms);
			rooms.Add(new DesertWellRoom());
		}

		public override Tile GetFilling() {
			return Tile.Chasm;
		}
	}
}