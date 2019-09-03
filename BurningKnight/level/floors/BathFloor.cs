using BurningKnight.level.biome;
using BurningKnight.level.rooms;
using BurningKnight.level.tile;
using BurningKnight.save;
using BurningKnight.util.geometry;
using Lens.util.math;

namespace BurningKnight.level.floors {
	public class BathFloor : FloorPainter {
		public override void Paint(Level level, RoomDef room, Rect inside, bool gold) {
			FloorRegistry.Paint(level, room, -1, gold);

			var m = Random.Int(2, 4);
			var a = gold && Random.Chance();
			
			Painter.Fill(level, room, m, a ? Tile.FloorD : Tiles.RandomFloor());
			Painter.Fill(level, room, m + 1, !a && gold && Random.Chance() ? Tile.FloorD : Tiles.RandomNewFloor());
			Painter.Fill(level, room, m + 1, LevelSave.BiomeGenerated is DesertBiome ? Tiles.RandomNewFloor() : Tile.Water);
		}
	}
}