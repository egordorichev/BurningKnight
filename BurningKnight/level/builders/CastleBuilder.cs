using System.Collections.Generic;
using BurningKnight.level.rooms;
using Lens.util.math;

namespace BurningKnight.level.builders {
	public class CastleBuilder : RegularBuilder {
		public override List<RoomDef> Build(List<RoomDef> Init) {
			SetupRooms(Init);

			if (Entrance == null) {
				return null;
			}

			var Branchable = new List<RoomDef>();
			Entrance.SetSize();
			Entrance.SetPos(0, 0);
			
			Branchable.Add(Entrance);
			var RoomsToBranch = new List<RoomDef>(MultiConnection);

			if (Exit != null) {
				RoomsToBranch.Add(Exit);
			}

			RoomsToBranch.AddRange(SingleConnection);

			if (!CreateBranches(Init, Branchable, RoomsToBranch, BranchTunnelChances)) {
				return null;
			}

			FindNeighbours(Init);

			foreach (RoomDef R in Init) {
				foreach (RoomDef N in R.Neighbours) {
					if (!N.Connected.ContainsKey(R) && Random.Float() < ExtraConnectionChance) {
						R.ConnectWithRoom(N);
					}
				}
			}

			return Init;
		}
	}
}