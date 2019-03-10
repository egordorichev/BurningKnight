using System.Collections.Generic;
using BurningKnight.entity.level.rooms;
using BurningKnight.entity.level.rooms.boss;
using BurningKnight.entity.level.rooms.connection;
using BurningKnight.entity.level.rooms.entrance;
using BurningKnight.entity.level.rooms.regular;
using Lens.util.math;

namespace BurningKnight.entity.level.builders {
	public class RegularBuilder : Builder {
		protected BossRoom Boss;
		protected float[] BranchTunnelChances = {2, 2, 1};
		protected EntranceRoom Entrance;
		protected EntranceRoom Exit;
		protected float ExtraConnectionChance = 0.2f;
		protected List<RoomDef> MultiConnection = new List<RoomDef>();
		protected float PathLength = 0.5f;
		protected float[] PathLenJitterChances = {0, 1, 0};
		protected float[] PathTunnelChances = {1, 3, 1};
		protected float PathVariance = 45f;
		protected List<RoomDef> SingleConnection = new List<RoomDef>();

		public void SetupRooms(List<RoomDef> Rooms) {
			Entrance = null;
			Exit = null;
			MultiConnection.Clear();
			SingleConnection.Clear();

			foreach (var Room in Rooms) {
				Room.SetEmpty();
			}

			foreach (var Room in Rooms) {
				if (Room is BossRoom room) {
					Exit = room;
				} else if (Room is EntranceRoom entranceRoom && entranceRoom.Exit) {
					Exit = entranceRoom;
				} else if (Room is EntranceRoom room1) {
					Entrance = room1;
				} else if (Room.GetMaxConnections(RoomDef.Connection.All) == 1) {
					SingleConnection.Add(Room);
				} else if (Room.GetMaxConnections(RoomDef.Connection.All) > 1) {
					MultiConnection.Add(Room);
				}
			}

			WeightRooms(MultiConnection);
			MultiConnection = new List<RoomDef>(MultiConnection);
		}

		protected void WeightRooms(List<RoomDef> Rooms) {
			/*foreach (var Room in Rooms) {
				if (Room is RegularRoom room) {
					for (var I = 1; I < room.GetSize().GetConnectionWeight(); I++) {
						Rooms.Add(room);
					}
					
					Rooms.Add(room);
				}
			}*/
		}

		public override List<RoomDef> Build(List<RoomDef> Init) {
			return Init;
		}

		public RegularBuilder SetPathVariance(float Var) {
			PathVariance = Var;

			return this;
		}

		public RegularBuilder SetPathLength(float Len, float[] Jitter) {
			PathLength = Len;
			PathLenJitterChances = Jitter;

			return this;
		}

		public RegularBuilder SetTunnelLength(float[] Path, float[] Branch) {
			PathTunnelChances = Path;
			BranchTunnelChances = Branch;

			return this;
		}

		public RegularBuilder SetExtraConnectionChance(float Chance) {
			ExtraConnectionChance = Chance;
			return this;
		}

		protected bool CreateBranches(List<RoomDef> Rooms, List<RoomDef> Branchable, List<RoomDef> RoomsToBranch, float[] ConnChances) {
			var I = 0;
			var N = 0;
			float Angle;
			int Tries;
			RoomDef Curr;
			
			var ConnectingRoomsThisBranch = new List<RoomDef>();
			var ConnectionChances = ConnChances; // fixme: clone

			while (I < RoomsToBranch.Count) {
				var R = RoomsToBranch[I];
				N++;
				ConnectingRoomsThisBranch.Clear();

				do {
					Curr = Branchable[Random.Int(Branchable.Count)];
				} while (Curr is ConnectionRoom);

				var ConnectingRooms = Random.Chances(ConnectionChances);

				if (ConnectingRooms == -1) {
					ConnectionChances = ConnChances; // fixme: clone?
					ConnectingRooms = Random.Chances(ConnectionChances);
				}

				ConnectionChances[ConnectingRooms]--;

				for (var J = 0; J < ConnectingRooms; J++) {
					var T = RoomRegistry.Generate(RoomType.Connection);
					Tries = 3;

					do {
						Angle = PlaceRoom(Rooms, Curr, T, RandomBranchAngle(Curr));
						Tries--;
					} while (Angle == -1 && Tries > 0);

					if (Angle == -1) {
						foreach (var C in ConnectingRoomsThisBranch) {
							C.ClearConnections();
							Rooms.Remove(C);
						}

						ConnectingRoomsThisBranch.Clear();

						break;
					}

					ConnectingRoomsThisBranch.Add(T);
					Rooms.Add(T);


					Curr = T;
				}

				if (ConnectingRoomsThisBranch.Count != ConnectingRooms) {
					if (N > 30) {
						return false;
					}

					continue;
				}

				Tries = 10;

				do {
					Angle = PlaceRoom(Rooms, Curr, R, RandomBranchAngle(Curr));
					Tries--;
				} while (Angle == -1 && Tries > 0);

				if (Angle == -1) {
					foreach (var T in ConnectingRoomsThisBranch) {
						T.ClearConnections();
						Rooms.Remove(T);
					}

					ConnectingRoomsThisBranch.Clear();

					if (N > 30) {
						return false;
					}

					continue;
				}

				foreach (var AConnectingRoomsThisBranch in ConnectingRoomsThisBranch) {
					if (Random.Int(3) <= 1) {
						Branchable.Add(AConnectingRoomsThisBranch);
					}
				}

				if (R.GetMaxConnections(RoomDef.Connection.All) > 1 && Random.Int(3) == 0) {
					if (R is RegularRoom room) {
						/*for (var J = 0; J < room.GetSize().GetConnectionWeight(); J++) {
							Branchable.Add(room);
						}*/
						
						Branchable.Add(room);
					} else {
						Branchable.Add(R);
					}
				}

				I++;
			}

			return true;
		}

		protected float RandomBranchAngle(RoomDef R) {
			return Random.Angle();
		}
	}
}