using System.Collections.Generic;
using BurningKnight.level.rooms;

namespace BurningKnight.level.builders {
	public class SingleRoomBuilder : Builder {
		public override List<RoomDef> Build(List<RoomDef> Init) {
			var Room = Init[0];
			
			Room.SetSize();
			Room.SetPos(0, 0);

			return Init;
		}
	}
}