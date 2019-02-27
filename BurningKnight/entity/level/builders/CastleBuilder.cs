using System.Collections.Generic;
using Lens.util.math;

namespace BurningKnight.entity.level.builders {
	public class CastleBuilder : RegularBuilder {
		public override List<Room> Build<Room>(List<Room> Init) {
			SetupRooms(Init);

			if (Entrance == null) {
				return null;
			}

			var Branchable = new List<Room>();
			Entrance.SetSize();
			Entrance.SetPos(0, 0);
			
			Branchable.Add(Entrance);
			var RoomsToBranch = new List<Room>(MultiConnection);

			if (Exit != null) {
				RoomsToBranch.Add(Exit);
			}

			RoomsToBranch.AddRange(SingleConnection);

			if (!this.CreateBranches(Init, Branchable, RoomsToBranch, BranchTunnelChances)) {
				return null;
			}

			FindNeighbours(Init);

			foreach (Room R in Init) {
				foreach (Room N in R.Neighbours) {
					if (!N.Connected().ContainsKey(R) && Random.Float() < ExtraConnectionChance) {
						R.ConnectWithRoom(N);
					}
				}
			}

			return Init;
		}
	}
}