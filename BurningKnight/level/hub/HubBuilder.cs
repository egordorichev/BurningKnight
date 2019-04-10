using System.Collections.Generic;
using BurningKnight.level.builders;
using BurningKnight.level.rooms;

namespace BurningKnight.level.hub {
	public class HubBuilder : RegularBuilder {
		public override List<RoomDef> Build(List<RoomDef> Init) {
			SetupRooms(Init);

			Entrance.SetSize();
			Entrance.SetPos(0, 0);
			
			PlaceRoom(Init, Entrance, Exit, 95);
			
			return Init;
		}
	}
}