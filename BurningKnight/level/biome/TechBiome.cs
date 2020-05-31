using BurningKnight.level.builders;
using BurningKnight.level.rooms.trap;
using BurningKnight.level.rooms.treasure;
using BurningKnight.level.tile;
using BurningKnight.state;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.biome {
	public class TechBiome: Biome {
		public TechBiome() : base("Pirate Bay", Biome.Tech, "tech_biome", new Color(27, 27, 27)) {
		
		}

		public override void ModifyPainter(Level level, Painter painter) {
			base.ModifyPainter(level, painter);

			painter.Grass = 0;
			painter.Water = 0;
			painter.Dirt = 0;
			painter.Cobweb = 0;
		
			painter.Modifiers.Add((l, rm, x, y) => {
				if (rm is TrapRoom) {
					return;
				}
				
				if (l.Get(x, y, true) == Tile.Lava) {
					var i = l.ToIndex(x, y);
					
					l.Liquid[i] = 0;
					l.Tiles[i] = (byte) Tile.Chasm;
				}
			});
		}

		public override bool HasTorches() {
			return false;
		}

		public override bool HasPaintings() {
			return false;
		}

		public override bool HasBrekables() {
			return false;
		}

		public override int GetNumRegularRooms() {
			return 0;
		}

		public override int GetNumSecretRooms() {
			return 0;
		}

		public override int GetNumSpecialRooms() {
			return 0;
		}

		public override int GetNumTrapRooms() {
			return 0;
		}

		public override Builder GetBuilder() {
			return new LoopBuilder();
		}

		public override bool HasCobwebs() {
			return false;
		}

		public override bool HasTnt() {
			return false;
		}
	}
}