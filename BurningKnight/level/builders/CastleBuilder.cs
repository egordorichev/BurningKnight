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
			
			var a = Rnd.Angle() - 90;
			var i = 0;
					
			while (true && Boss != null) {
				var an = PlaceRoom(Init, Exit, Boss, a);
						
				if ((int) an != -1) {
					break;
				}

				i++;

				if (i > 36) {
					return null;
				}
						
				a += 10;
			}

			FindNeighbours(Init);

			foreach (RoomDef R in Init) {
				foreach (RoomDef N in R.Neighbours) {
					if (!N.Connected.ContainsKey(R) && Rnd.Float() < ExtraConnectionChance) {
						R.ConnectWithRoom(N);
					}
				}
			}

			return Init;
		}
	}
}