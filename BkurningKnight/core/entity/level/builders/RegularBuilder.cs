using BurningKnight.core.entity.level.rooms;
using BurningKnight.core.entity.level.rooms.boss;
using BurningKnight.core.entity.level.rooms.connection;
using BurningKnight.core.entity.level.rooms.entrance;
using BurningKnight.core.entity.level.rooms.regular;
using BurningKnight.core.entity.level.save;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.level.builders {
	public class RegularBuilder : Builder {
		protected EntranceRoom Entrance;
		protected EntranceRoom Exit;
		protected BossRoom Boss;
		protected PrebossRoom Preboss;
		protected LampRoom Lamp;
		protected float PathVariance = 45f;
		protected float PathLength = 0.5f;
		protected float[] PathLenJitterChances = { 0, 1, 0 };
		protected float[] PathTunnelChances = { 1, 3, 1 };
		protected float[] BranchTunnelChances = { 2, 2, 1 };
		protected float ExtraConnectionChance = 0.2f;
		protected List<Room> MultiConnection = new List<>();
		protected List<Room> SingleConnection = new List<>();

		public Void SetupRooms(List Rooms) {
			this.Entrance = null;
			this.Exit = null;
			this.Lamp = null;
			this.MultiConnection.Clear();
			this.SingleConnection.Clear();

			foreach (Room Room in Rooms) {
				Room.SetEmpty();
			}

			foreach (Room Room in Rooms) {
				if (Room is BossRoom) {
					this.Boss = (BossRoom) Room;
					this.Exit = (EntranceRoom) Room;
				} else if (Room is EntranceRoom && ((EntranceRoom) Room).Exit) {
					this.Exit = (EntranceRoom) Room;
				} else if (Room is EntranceRoom) {
					this.Entrance = (EntranceRoom) Room;
				} else if (Room is PrebossRoom) {
					this.Preboss = (PrebossRoom) Room;
				} else if (Room.GetMaxConnections(Room.Connection.ALL) == 1) {
					this.SingleConnection.Add(Room);
				} else if (Room.GetMaxConnections(Room.Connection.ALL) > 1) {
					this.MultiConnection.Add(Room);
				} 

				if (Room is LampRoom) {
					this.Lamp = (LampRoom) Room;
				} 
			}

			if (Dungeon.Type != Dungeon.Type.INTRO) {
				this.WeightRooms(this.MultiConnection);

				if (GameSave.RunId != 0 || Dungeon.Depth != 1) {

				} 

				this.MultiConnection = new List<>(new LinkedHashSet<>(this.MultiConnection));
			} 
		}

		protected Void WeightRooms(List Rooms) {
			foreach (Room Room in Rooms.ToArray(new Room[0])) {
				if (Room is RegularRoom) {
					for (int I = 1; I < ((RegularRoom) Room).GetSize().GetConnectionWeight(); I++) {
						Rooms.Add(Room);
					}
				} 
			}
		}

		public override List Build<Room> (List Init) {
			return Init;
		}

		public RegularBuilder SetPathVariance(float Var) {
			this.PathVariance = Var;

			return this;
		}

		public RegularBuilder SetPathLength(float Len, float Jitter) {
			this.PathLength = Len;
			this.PathLenJitterChances = Jitter;

			return this;
		}

		public RegularBuilder SetTunnelLength(float Path, float Branch) {
			this.PathTunnelChances = Path;
			this.BranchTunnelChances = Branch;

			return this;
		}

		public RegularBuilder SetExtraConnectionChance(float Chance) {
			this.ExtraConnectionChance = Chance;

			return this;
		}

		protected bool CreateBranches(List Rooms, List Branchable, List RoomsToBranch, float ConnChances) {
			int I = 0;
			int N = 0;
			float Angle;
			int Tries;
			Room Curr;
			List<Room> ConnectingRoomsThisBranch = new List<>();
			float[] ConnectionChances = ConnChances.Clone();

			while (I < RoomsToBranch.Size()) {
				Room R = RoomsToBranch.Get(I);
				N++;
				ConnectingRoomsThisBranch.Clear();

				do {
					Curr = Branchable.Get(Random.NewInt(Branchable.Size()));
				} while (Curr is Org.Rexcellentgames.Burningknight.Entity.Level.Rooms.Connection.ConnectionRoom);

				int ConnectingRooms = Random.Chances(ConnectionChances);

				if (ConnectingRooms == -1) {
					ConnectionChances = ConnChances.Clone();
					ConnectingRooms = Random.Chances(ConnectionChances);
				} 

				ConnectionChances[ConnectingRooms]--;

				for (int J = 0; J < ConnectingRooms; J++) {
					ConnectionRoom T = ConnectionRoom.Create();
					Tries = 3;

					do {
						Angle = PlaceRoom(Rooms, Curr, T, RandomBranchAngle(Curr));
						Tries--;
					} while (Angle == -1 && Tries > 0);

					if (Angle == -1) {
						foreach (Room C in ConnectingRoomsThisBranch) {
							C.ClearConnections();
							Rooms.Remove(C);
						}

						ConnectingRoomsThisBranch.Clear();

						break;
					} else {
						ConnectingRoomsThisBranch.Add(T);
						Rooms.Add(T);
					}


					Curr = T;
				}

				if (ConnectingRoomsThisBranch.Size() != ConnectingRooms) {
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
					foreach (Room T in ConnectingRoomsThisBranch) {
						T.ClearConnections();
						Rooms.Remove(T);
					}

					ConnectingRoomsThisBranch.Clear();

					if (N > 30) {
						return false;
					} 

					continue;
				} 

				foreach (Room AConnectingRoomsThisBranch in ConnectingRoomsThisBranch) {
					if (Random.NewInt(3) <= 1) Branchable.Add(AConnectingRoomsThisBranch);

				}

				if (R.GetMaxConnections(Room.Connection.ALL) > 1 && Random.NewInt(3) == 0) {
					if (R is RegularRoom) {
						for (int J = 0; J < ((RegularRoom) R).GetSize().GetConnectionWeight(); J++) {
							Branchable.Add(R);
						}
					} else {
						Branchable.Add(R);
					}

				} 

				I++;
			}

			return true;
		}

		protected float RandomBranchAngle(Room R) {
			return Random.NewFloat(360f);
		}
	}
}
