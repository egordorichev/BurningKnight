using System.Collections.Generic;
using BurningKnight.level.biome;
using BurningKnight.level.builders;
using BurningKnight.level.rooms;
using BurningKnight.level.tile;

namespace BurningKnight.level.hub {
	public class HubLevel : RegularLevel {
		// castle is tmp so that my eyes do not hurt so much
		public HubLevel() : base(BiomeRegistry.Get(Biome.Castle)) {
			NoLightNoRender = false;
			DrawLight = false;
		}

		protected override List<RoomDef> CreateRooms() {
			return new List<RoomDef> {
				new HubRoom()
			};
		}

		public override Tile GetFilling() {
			return Tile.FloorC;
		}

		protected override Builder GetBuilder() {
			return new SingleRoomBuilder();
		}

		public override int GetPadding() {
			return 0; 
		}

		public override string GetMusic() {
			return "Outsider";
		}
	}
}