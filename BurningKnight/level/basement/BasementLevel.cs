using System.Collections.Generic;
using BurningKnight.level.biome;
using BurningKnight.level.builders;
using BurningKnight.level.hall;
using BurningKnight.level.rooms;

namespace BurningKnight.level.basement {
	public class BasementLevel : RegularLevel {
		public BasementLevel() : base(BiomeRegistry.Get(Biome.Castle)) {

		}

		protected override List<RoomDef> CreateRooms() { 
			return new List<RoomDef> {
					new BasementRoom()
			};
		}

		protected override Builder GetBuilder() {
			return new HallBuilder();
		}

		protected override Painter GetPainter() {
			return new Painter {
					Water = 0,
					Cobweb = 0,
					Grass = 0,
					Dirt = 0
			};
		}

		public override string GetMusic() {
			return "Gobbeon";
		}
	}
}