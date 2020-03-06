using BurningKnight.level.tile;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.biome {
	public class HubBiome : Biome {
		public HubBiome() : base("Outsider", Biome.Hub, "hub_biome", new Color(30, 111, 80)) {
			
		}

		public override string GetStepSound(Tile tile) {
			if (tile == Tile.FloorB) {
				return $"player_step_wood_{Rnd.Int(1, 4)}";
			}
			
			return base.GetStepSound(tile);
		}
	}
}