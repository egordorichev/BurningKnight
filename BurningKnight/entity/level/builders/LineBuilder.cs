using System;
using System.Collections.Generic;
using BurningKnight.entity.level.rooms;
using BurningKnight.entity.level.rooms.connection;
using BurningKnight.state;
using BurningKnight.util;
using Lens.util;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.level.builders {
	public class LineBuilder : RegularBuilder {
		private float Direction = Random.Angle();

		public LineBuilder SetAngle(float Angle) {
			Direction = Angle % 360f;

			return this;
		}

		public override List<RoomDef> Build(List<RoomDef> Init) {
			SetupRooms(Init);

			if (Entrance == null) {
				Log.Error("No entrance!");
				return null;
			}

			var Branchable = new List<RoomDef>();
			
			Entrance.SetSize();
			Entrance.SetPos(0, 0);
			Branchable.Add(Entrance);

			if (MultiConnection.Count == 0) {
				PlaceRoom(Init, Entrance, Boss, Random.Angle());

				return Init;
			}

			var RoomsOnPath = (int) (MultiConnection.Count * PathLength) + Random.Chances(PathLenJitterChances);
			RoomsOnPath = Math.Min(RoomsOnPath, MultiConnection.Count);
			RoomDef Curr = Entrance;
			var PathTunnels = PathTunnelChances; // fixme: clone?

			for (var I = 0; I <= RoomsOnPath; I++) {
				if (I == RoomsOnPath && Exit == null) continue;

				var Tunnels = Random.Chances(PathTunnels);

				if (Tunnels == -1) {
					PathTunnels = PathTunnelChances; // fixme: clone?
					Tunnels = Random.Chances(PathTunnels);
				}

				PathTunnels[Tunnels]--;

				if (I != 0 && Run.Depth != 0)
					for (var J = 0; J < Tunnels; J++) {
						var T = RoomRegistry.Generate(RoomType.Connection);

						if ((int) PlaceRoom(Init, Curr, T, Direction + Random.Float(-PathVariance, PathVariance)) == -1) {
							return null;
						}

						Branchable.Add(T);
						Init.Add(T);
						Curr = T;
					}

				var R = I == RoomsOnPath ? Exit : MultiConnection[I];


				if ((int) PlaceRoom(Init, Curr, R, Direction + Random.Float(-PathVariance, PathVariance)) == -1) {
					return null;
				}

				Branchable.Add(R);
				Curr = R;
			}

			var RoomsToBranch = new List<RoomDef>();

			for (var I = RoomsOnPath; I < MultiConnection.Count; I++) {
				RoomsToBranch.Add(MultiConnection[I]);
			}

			RoomsToBranch.AddRange(SingleConnection);
			
			WeightRooms(Branchable);
			CreateBranches(Init, Branchable, RoomsToBranch, BranchTunnelChances);
			FindNeighbours(Init);

			foreach (var R in Init) {
				foreach (var N in R.Neighbours) {
					if (!N.Connected.ContainsKey(R) && Random.Float() < ExtraConnectionChance) {
						R.ConnectWithRoom(N);
					}
				}
			}

			return Init;
		}
	}
}