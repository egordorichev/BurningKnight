using System.Collections.Generic;
using BurningKnight.level.biome;
using BurningKnight.level.builders;
using BurningKnight.level.rooms;
using BurningKnight.level.tile;

namespace BurningKnight.level.hub {
	public class HubLevel : RegularLevel {
		public HubLevel() : base(BiomeRegistry.Get(Biome.Hub)) {
			
		}

		protected override List<RoomDef> CreateRooms() {
			var rooms = new List<RoomDef>();
			
			rooms.Add(new HubEntranceRoom());
			rooms.Add(new HubExitRoom());

			return rooms;
		}

		public override Tile GetFilling() {
			return Tile.FloorC;
		}

		protected override Builder GetBuilder() {
			return new HubBuilder();
		}
	}
}