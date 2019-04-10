using System.Collections.Generic;
using BurningKnight.level.builders;
using BurningKnight.level.rooms;

namespace BurningKnight.level.hall {
	public class HallBuilder : RegularBuilder {
		public override List<RoomDef> Build(List<RoomDef> Init) {
			SetupRooms(Init);

			Exit.SetSize();
			Exit.SetPos(0, 0);
			
			// PlaceRoom(Init, Entrance, Exit, 95);
			
			return Init;
		}
	}
}