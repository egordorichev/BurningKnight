using BurningKnight.assets.lighting;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.biome {
	public class CaveBiome : Biome {
		public CaveBiome() : base("Cursed legend", Biome.Cave, "cave_biome", new Color(61, 61, 61)) {}

		public override void Apply() {
			base.Apply();

			var v = 0f;
			Lights.ClearColor = new Color(v, v, v, 1f);
		}

		public override bool HasBrekables() {
			return false;
		}

		public override bool HasCobwebs() {
			return true;
		}

		public override bool HasPaintings() {
			return false;
		}

		public override bool HasTorches() {
			return false;
		}
		
		private static Color mapColor = new Color(61, 61, 61);

		public override Color GetMapColor() {
			return mapColor;
		}
		
		public override void ModifyPainter(Level level, Painter painter) {
			base.ModifyPainter(level, painter);

			painter.Grass = 0;
			painter.Water = 0.1f;
			painter.Dirt = 0.35f;
			painter.Cobweb = 0.1f;
		}
	}
}