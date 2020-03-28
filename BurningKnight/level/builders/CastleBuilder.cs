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

			if (Boss != null) {
				while (true) {
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

				a = Rnd.Angle();
				i = 0;

				if (Granny != null) {
					while (true) {
						var an = PlaceRoom(Init, Boss, Granny, a);

						if ((int) an != -1) {
							break;
						}

						i++;

						if (i > 72) {
							return null;
						}

						a += 5;
					}
				}

				a = Rnd.Angle();
				i = 0;

				if (OldMan != null) {
					while (true) {
						var an = PlaceRoom(Init, Boss, OldMan, a);

						if ((int) an != -1) {
							break;
						}

						i++;

						if (i > 72) {
							return null;
						}

						a += 5;
					}
				}
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