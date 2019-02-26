using BurningKnight.util;

namespace BurningKnight.entity.level.builders {
	public class CastleBuilder : RegularBuilder {
		public override List Build<Room>(List Init) {
			SetupRooms(Init);

			if (Entrance == null) return null;

			List<Room> Branchable = new List<>();
			Entrance.SetSize();
			Entrance.SetPos(0, 0);
			Branchable.Add(Entrance);
			List<Room> RoomsToBranch = new List<>(MultiConnection);

			if (Exit != null) RoomsToBranch.Add(Exit);

			RoomsToBranch.AddAll(SingleConnection);

			if (!this.CreateBranches(Init, Branchable, RoomsToBranch, BranchTunnelChances)) return null;

			FindNeighbours(Init);

			foreach (Room R in Init)
			foreach (Room N in R.GetNeighbours())
				if (!N.GetConnected().ContainsKey(R) && Random.NewFloat() < ExtraConnectionChance)
					R.ConnectWithRoom(N);

			return Init;
		}
	}
}