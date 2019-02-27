using BurningKnight.entity.level.rooms.connection;
using BurningKnight.entity.level.rooms.regular;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.builders {
	public class LoopBuilder : RegularBuilder {
		private int Exponent;
		private float Intensity = 1;
		private Point LoopCenter;
		private float Offset;

		public LoopBuilder SetShape(int Exponent, float Intensity, float Offset) {
			this.Exponent = Math.Abs(Exponent);
			this.Intensity = Intensity % 1;
			this.Offset = Offset % 0.5f;

			return this;
		}

		private float TargetAngle(float PercentAlong) {
			PercentAlong += Offset;

			return 360f * (float) (Intensity * CurveEquation(PercentAlong) + (1 - Intensity) * PercentAlong - Offset);
		}

		private double CurveEquation(double X) {
			return Math.Pow(4, 2 * Exponent) * Math.Pow(X % 0.5f - 0.25, 2 * Exponent + 1) + 0.25 + 0.5 * Math.Floor(2 * X);
		}

		public override List Build<Room>(List Init) {
			SetupRooms(Init);

			if (Entrance == null) return null;

			Entrance.SetPos(0, 0);
			Entrance.SetSize();
			var StartAngle = Random.NewFloat(0, 360);
			List<Room> Loop = new List<>();
			var RoomsOnLoop = (int) (MultiConnection.Size() * PathLength) + Random.Chances(PathLenJitterChances);
			RoomsOnLoop = Math.Min(RoomsOnLoop, MultiConnection.Size());
			RoomsOnLoop++;
			float[] PathTunnels = PathTunnelChances.Clone();

			for (var I = 0; I < RoomsOnLoop; I++) {
				if (I == 0)
					Loop.Add(Entrance);
				else
					Loop.Add(MultiConnection.Remove(0));


				var Tunnels = Random.Chances(PathTunnels);

				if (Tunnels == -1) {
					PathTunnels = PathTunnelChances.Clone();
					Tunnels = Random.Chances(PathTunnels);
				}

				PathTunnels[Tunnels]--;

				for (var J = 0; J < Tunnels; J++) Loop.Add(ConnectionRoomDef.Create());
			}

			if (Exit != null) Loop.Add((Loop.Size() + 1) / 2, Exit);

			Room Prev = Entrance;
			float TargetAngle;

			for (var I = 1; I < Loop.Size(); I++) {
				Room R = Loop.Get(I);
				TargetAngle = StartAngle + this.TargetAngle(I / (float) Loop.Size());

				if (PlaceRoom(Init, Prev, R, TargetAngle) != -1) {
					Prev = R;

					if (!Init.Contains(Prev)) Init.Add(Prev);
				}
				else {
					return null;
				}
			}

			while (!Prev.ConnectTo(Entrance)) {
				RegularRoomDef C = RegularRoomDef.Create();

				if (PlaceRoom(Loop, Prev, C, AngleBetweenRooms(Prev, Entrance)) == -1) return null;

				Loop.Add(C);
				Init.Add(C);
				Prev = C;
			}

			LoopCenter = new Point();

			foreach (Room R in Loop) {
				LoopCenter.X += (R.Left + R.Right) / 2f;
				LoopCenter.Y += (R.Top + R.Bottom) / 2f;
			}

			LoopCenter.X /= Loop.Size();
			LoopCenter.Y /= Loop.Size();
			List<Room> Branchable = new List<>(Loop);
			List<Room> RoomsToBranch = new List<>();
			RoomsToBranch.AddAll(MultiConnection);
			RoomsToBranch.AddAll(SingleConnection);
			WeightRooms(Branchable);
			CreateBranches(Init, Branchable, RoomsToBranch, BranchTunnelChances);
			FindNeighbours(Init);

			foreach (Room R in Init)
			foreach (Room N in R.GetNeighbours())
				if (!N.GetConnected().ContainsKey(R) && Random.NewFloat() < ExtraConnectionChance)
					R.ConnectWithRoom(N);

			if (!Prev.GetConnected().ContainsKey(Entrance)) {
				Prev.Neighbours.Add(Entrance);
				Entrance.Neighbours.Add(Prev);
				Prev.GetConnected().Put(Entrance, null);
				Entrance.GetConnected().Put(Prev, null);
			}

			return Init;
		}
	}
}