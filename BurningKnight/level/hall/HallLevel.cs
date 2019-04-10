using System.Collections.Generic;
using BurningKnight.level.biome;
using BurningKnight.level.builders;
using BurningKnight.level.rooms;

namespace BurningKnight.level.hall {
	public class HallLevel : RegularLevel {
		public HallLevel() : base(BiomeRegistry.Get(Biome.Castle)) {
			
		}

		protected override List<RoomDef> CreateRooms() {
			var rooms = new List<RoomDef>();
			
			rooms.Add(new HallRoom());

			return rooms;
		}

		protected override Builder GetBuilder() {
			return new HallBuilder();
		}
	}
}