using System.Collections.Generic;
using BurningKnight.assets.lighting;
using BurningKnight.level.rooms;
using BurningKnight.level.rooms.regular;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.biome {
	public class JungleBiome : Biome {
		public JungleBiome() : base("Botanical Expedition", Biome.Jungle, "jungle_biome", new Color(30, 111, 80)) {}

		public override void Apply() {
			base.Apply();
			Lights.ClearColor = ColorUtils.WhiteColor;
		}

		public override void ModifyRooms(List<RoomDef> rooms) {
			base.ModifyRooms(rooms);
			rooms.Add(new JungleRoom());
		}

		public override int GetNumRegularRooms() {
			return 0;
		}
		
		public override int GetNumTrapRooms() {
			return 0;
		}

		public override bool HasTorches() {
			return false;
		}

		public override bool HasSpikes() {
			return true;
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