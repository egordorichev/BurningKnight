using BurningKnight.core.entity.level.rooms.connection;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.level.builders {
	public class LineBuilder : RegularBuilder {
		private float Direction = Random.NewFloat(0, 360);

		public LineBuilder SetAngle(float Angle) {
			this.Direction = Angle % 360f;

			return this;
		}

		public override List Build<Room> (List Init) {
			SetupRooms(Init);

			if (Entrance == null) {
				Log.Error("No entrance!");

				return null;
			} 

			List<Room> Branchable = new List<>();
			Entrance.SetSize();
			Entrance.SetPos(0, 0);
			Branchable.Add(Entrance);

			if (MultiConnection.Size() == 0) {
				PlaceRoom(Init, Entrance, Boss, Random.NewFloat(360));

				return Init;
			} 

			int RoomsOnPath = (int) (this.MultiConnection.Size() * PathLength) + Random.Chances(PathLenJitterChances);
			RoomsOnPath = Math.Min(RoomsOnPath, this.MultiConnection.Size());
			Room Curr = Entrance;
			float[] PathTunnels = PathTunnelChances.Clone();
			bool Boss = Preboss != null;

			for (int I = 0; I <= RoomsOnPath + (Boss ? 1 : 0); I++) {
				if (I == RoomsOnPath && Exit == null) {
					continue;
				} 

				int Tunnels = Random.Chances(PathTunnels);

				if (Tunnels == -1) {
					PathTunnels = PathTunnelChances.Clone();
					Tunnels = Random.Chances(PathTunnels);
				} 

				PathTunnels[Tunnels]--;

				if (I != 0 && (!Boss || I < RoomsOnPath - 1) && Dungeon.Depth != 0) {
					for (int J = 0; J < Tunnels; J++) {
						ConnectionRoom T = ConnectionRoom.Create();

						if (PlaceRoom(Init, Curr, T, Direction + Random.NewFloat(-PathVariance, PathVariance)) == -1) {
							return null;
						} 

						Branchable.Add(T);
						Init.Add(T);
						Curr = T;
					}
				} 

				Room R;

				if (Boss) {
					R = (I > RoomsOnPath ? Exit : (I == RoomsOnPath ? Preboss : this.MultiConnection.Get(I)));
				} else {
					R = (I == RoomsOnPath ? Exit : this.MultiConnection.Get(I));
				}


				if (PlaceRoom(Init, Curr, R, Direction + Random.NewFloat(-PathVariance, PathVariance)) == -1) {
					return null;
				} 

				Branchable.Add(R);
				Curr = R;
			}

			List<Room> RoomsToBranch = new List<>();

			for (int I = RoomsOnPath; I < this.MultiConnection.Size(); I++) {
				RoomsToBranch.Add(this.MultiConnection.Get(I));
			}

			RoomsToBranch.AddAll(this.SingleConnection);
			WeightRooms(Branchable);
			CreateBranches(Init, Branchable, RoomsToBranch, BranchTunnelChances);
			FindNeighbours(Init);

			foreach (Room R in Init) {
				foreach (Room N in R.GetNeighbours()) {
					if (!N.GetConnected().ContainsKey(R) && Random.NewFloat() < ExtraConnectionChance) {
						R.ConnectWithRoom(N);
					} 
				}
			}

			return Init;
		}
	}
}
