using BurningKnight.core.entity.level.rooms.connection;
using BurningKnight.core.entity.level.rooms.regular;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.builders {
	public class LoopBuilder : RegularBuilder {
		private int Exponent;
		private float Intensity = 1;
		private float Offset;
		private Point LoopCenter;

		public LoopBuilder SetShape(int Exponent, float Intensity, float Offset) {
			this.Exponent = Math.Abs(Exponent);
			this.Intensity = Intensity % 1;
			this.Offset = Offset % 0.5f;

			return this;
		}

		private float TargetAngle(float PercentAlong) {
			PercentAlong += Offset;

			return 360f * (float) (Intensity * CurveEquation(PercentAlong) + (1 - Intensity) * (PercentAlong) - Offset);
		}

		private double CurveEquation(double X) {
			return Math.Pow(4, 2 * Exponent) * (Math.Pow((X % 0.5f) - 0.25, 2 * Exponent + 1)) + 0.25 + 0.5 * Math.Floor(2 * X);
		}

		public override List Build<Room> (List Init) {
			this.SetupRooms(Init);

			if (this.Entrance == null) {
				return null;
			} 

			this.Entrance.SetPos(0, 0);
			this.Entrance.SetSize();
			float StartAngle = Random.NewFloat(0, 360);
			List<Room> Loop = new List<>();
			int RoomsOnLoop = (int) (this.MultiConnection.Size() * this.PathLength) + Random.Chances(this.PathLenJitterChances);
			RoomsOnLoop = Math.Min(RoomsOnLoop, this.MultiConnection.Size());
			RoomsOnLoop++;
			float[] PathTunnels = this.PathTunnelChances.Clone();

			for (int I = 0; I < RoomsOnLoop; I++) {
				if (I == 0) {
					Loop.Add(Entrance);
				} else {
					Loop.Add(this.MultiConnection.Remove(0));
				}


				int Tunnels = Random.Chances(PathTunnels);

				if (Tunnels == -1) {
					PathTunnels = this.PathTunnelChances.Clone();
					Tunnels = Random.Chances(PathTunnels);
				} 

				PathTunnels[Tunnels]--;

				for (int J = 0; J < Tunnels; J++) {
					Loop.Add(ConnectionRoom.Create());
				}
			}

			if (this.Exit != null) {
				Loop.Add((Loop.Size() + 1) / 2, this.Exit);
			} 

			Room Prev = this.Entrance;
			float TargetAngle;

			for (int I = 1; I < Loop.Size(); I++) {
				Room R = Loop.Get(I);
				TargetAngle = StartAngle + this.TargetAngle(I / (float) Loop.Size());

				if (PlaceRoom(Init, Prev, R, TargetAngle) != -1) {
					Prev = R;

					if (!Init.Contains(Prev)) {
						Init.Add(Prev);
					} 
				} else {
					return null;
				}

			}

			while (!Prev.ConnectTo(this.Entrance)) {
				RegularRoom C = RegularRoom.Create();

				if (PlaceRoom(Loop, Prev, C, AngleBetweenRooms(Prev, this.Entrance)) == -1) {
					return null;
				} 

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
			RoomsToBranch.AddAll(this.MultiConnection);
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

			if (!Prev.GetConnected().ContainsKey(this.Entrance)) {
				Prev.Neighbours.Add(this.Entrance);
				this.Entrance.Neighbours.Add(Prev);
				Prev.GetConnected().Put(this.Entrance, null);
				this.Entrance.GetConnected().Put(Prev, null);
			} 

			return Init;
		}
	}
}
