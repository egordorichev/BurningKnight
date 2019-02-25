using BurningKnight.core.util;

namespace BurningKnight.core.entity.level.builders {
	public class CastleBuilder : RegularBuilder {
		public override List Build<Room> (List Init) {
			this.SetupRooms(Init);

			if (this.Entrance == null) {
				return null;
			} 

			List<Room> Branchable = new List<>();
			this.Entrance.SetSize();
			this.Entrance.SetPos(0, 0);
			Branchable.Add(this.Entrance);
			List<Room> RoomsToBranch = new List<>(this.MultiConnection);

			if (this.Exit != null) {
				RoomsToBranch.Add(this.Exit);
			} 

			RoomsToBranch.AddAll(this.SingleConnection);

			if (!this.CreateBranches(Init, Branchable, RoomsToBranch, this.BranchTunnelChances)) {
				return null;
			} 

			FindNeighbours(Init);

			foreach (Room R in Init) {
				foreach (Room N in R.GetNeighbours()) {
					if (!N.GetConnected().ContainsKey(R) && Random.NewFloat() < this.ExtraConnectionChance) {
						R.ConnectWithRoom(N);
					} 
				}
			}

			return Init;
		}
	}
}
