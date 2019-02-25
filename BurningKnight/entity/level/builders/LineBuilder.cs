using BurningKnight.entity.level.rooms.connection;
using BurningKnight.util;

namespace BurningKnight.entity.level.builders {
	public class LineBuilder : RegularBuilder {
		private float Direction = Random.NewFloat(0, 360);

		public LineBuilder SetAngle(float Angle) {
			Direction = Angle % 360f;

			return this;
		}

		public override List Build<Room>(List Init) {
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

			var RoomsOnPath = (int) (MultiConnection.Size() * PathLength) + Random.Chances(PathLenJitterChances);
			RoomsOnPath = Math.Min(RoomsOnPath, MultiConnection.Size());
			Room Curr = Entrance;
			float[] PathTunnels = PathTunnelChances.Clone();
			var Boss = Preboss != null;

			for (var I = 0; I <= RoomsOnPath + (Boss ? 1 : 0); I++) {
				if (I == RoomsOnPath && Exit == null) continue;

				var Tunnels = Random.Chances(PathTunnels);

				if (Tunnels == -1) {
					PathTunnels = PathTunnelChances.Clone();
					Tunnels = Random.Chances(PathTunnels);
				}

				PathTunnels[Tunnels]--;

				if (I != 0 && (!Boss || I < RoomsOnPath - 1) && Dungeon.Depth != 0)
					for (var J = 0; J < Tunnels; J++) {
						var T = ConnectionRoom.Create();

						if (PlaceRoom(Init, Curr, T, Direction + Random.NewFloat(-PathVariance, PathVariance)) == -1) return null;

						Branchable.Add(T);
						Init.Add(T);
						Curr = T;
					}

				Room R;

				if (Boss)
					R = I > RoomsOnPath ? Exit : (I == RoomsOnPath ? Preboss : MultiConnection.Get(I));
				else
					R = I == RoomsOnPath ? Exit : MultiConnection.Get(I);


				if (PlaceRoom(Init, Curr, R, Direction + Random.NewFloat(-PathVariance, PathVariance)) == -1) return null;

				Branchable.Add(R);
				Curr = R;
			}

			List<Room> RoomsToBranch = new List<>();

			for (var I = RoomsOnPath; I < MultiConnection.Size(); I++) RoomsToBranch.Add(MultiConnection.Get(I));

			RoomsToBranch.AddAll(SingleConnection);
			WeightRooms(Branchable);
			CreateBranches(Init, Branchable, RoomsToBranch, BranchTunnelChances);
			FindNeighbours(Init);

			foreach (Room R in Init)
			foreach (Room N in R.GetNeighbours())
				if (!N.GetConnected().ContainsKey(R) && Random.NewFloat() < ExtraConnectionChance)
					R.ConnectWithRoom(N);

			return Init;
		}
	}
}