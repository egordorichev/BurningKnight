using Microsoft.Xna.Framework;

namespace BurningKnight.level.biome {
	public class JungleBiome : Biome {
		public JungleBiome() : base("Botanical Expedition", Biome.Jungle, "jungle_biome", new Color(30, 111, 80)) {}

		public override int GetNumRegularRooms() {
			return 1;
		}
		
		public override int GetNumTrapRooms() {
			return 0;
		}

		public override bool HasTorches() {
			return false;
		}

		public override bool HasPaintings() {
			return false;
		}

		public override bool HasTnt() {
			return false;
		}

		public override bool HasBrekables() {
			return false;
		}

		public override bool HasPlants() {
			return true;
		}
	}
}