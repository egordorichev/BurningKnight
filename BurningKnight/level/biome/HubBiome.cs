using BurningKnight.level.tile;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.biome {
	public class HubBiome : Biome {
		public HubBiome() : base("Outsider", Biome.Hub, Events.Halloween ? "spooky_biome" : (Events.XMas ? "xmas_biome" : "hub_biome"), new Color(30, 111, 80)) {
			
		}

		public override void ModifyPainter(Level level, Painter painter) {
			base.ModifyPainter(level, painter);

			painter.Fireflies = 10;
			painter.FirefliesChance = 100f;
		}

		public override string GetStepSound(Tile tile) {
			if (tile == Tile.FloorB) {
				return $"player_step_wood_{Rnd.Int(1, 4)}";
			} else if (tile == Tile.FloorD) {
				return $"player_step_grass_{Rnd.Int(1, 4)}";
			}
			
			return base.GetStepSound(tile);
		}
	}
}