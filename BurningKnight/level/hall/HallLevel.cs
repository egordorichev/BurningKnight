using System.Collections.Generic;
using BurningKnight.level.biome;
using BurningKnight.level.builders;
using BurningKnight.level.rooms;

namespace BurningKnight.level.hall {
	public class HallLevel : RegularLevel {
		public HallLevel() : base(BiomeRegistry.Get(Biome.Hub)) {

		}

		protected override List<RoomDef> CreateRooms() { 
			return new List<RoomDef> {
				new HallRoom()
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
			return "Hub";
		}
	}
}