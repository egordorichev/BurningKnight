using BurningKnight.core.entity.creature.mob;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.item;
using BurningKnight.core.entity.level.builders;
using BurningKnight.core.entity.level.entities;
using BurningKnight.core.entity.level.entities.chest;
using BurningKnight.core.entity.level.levels;
using BurningKnight.core.entity.level.levels.creep;
using BurningKnight.core.entity.level.painters;
using BurningKnight.core.entity.level.rooms;
using BurningKnight.core.entity.level.rooms.connection;
using BurningKnight.core.entity.level.rooms.entrance;
using BurningKnight.core.entity.level.rooms.regular;
using BurningKnight.core.entity.level.rooms.special;
using BurningKnight.core.entity.level.rooms.treasure;
using BurningKnight.core.entity.level.save;
using BurningKnight.core.entity.pool;
using BurningKnight.core.entity.pool.room;
using BurningKnight.core.game.state;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level {
	public abstract class RegularLevel : Level {
		public static Entrance Ladder;
		protected bool IsBoss;

		public RegularLevel SetBoss(bool Boss) {
			IsBoss = Boss;

			return this;
		}

		public override Void Generate(int Attempt) {
			Random.Random.SetSeed(ItemSelectState.StringToSeed(Random.GetSeed()) + Dungeon.Depth * 128 + Attempt);
			Player.All.Clear();
			Mob.All.Clear();
			ItemHolder.All.Clear();
			Chest.All.Clear();
			Mimic.All.Clear();
			PlayerSave.All.Clear();
			LevelSave.All.Clear();
			Dungeon.Area.Destroy();
			Dungeon.Area.Add(Dungeon.Level);
			Level.GENERATED = true;
			this.ItemsToSpawn.Clear();

			if (Dungeon.Depth > 0) {
				for (int I = 0; I < Random.NewInt(4); I++) {
					this.ItemsToSpawn.Add(new Bomb());
				}
			} 

			this.Build();
			this.Paint();

			if (this.Rooms == null) {
				Log.Error("NO ROOMS!");
			} 

			Log.Info("Done painting");
			this.LoadPassable();
			Log.Info("Spawning entities...");

			if (!Random.GetSeed().Equals("BK")) {
				this.SpawnLevelEntities();
				this.SpawnEntities();
			} 

			Log.Info("Done!");
		}

		protected Void SpawnLevelEntities() {
			this.Free = new bool[GetSize()];

			if (Dungeon.Depth > 0) {
				MobPool.Instance.InitForFloor();
				Log.Info("Spawn modifier is x" + Player.MobSpawnModifier);

				foreach (Room Room in this.Rooms) {
					if (Room is RegularRoom && !(Room is PrebossRoom) || (Room is TreasureRoom && Random.Chance(20))) {
						float Weight = ((Random.NewFloat(1f, 3f) + Room.GetWidth() * Room.GetHeight() / 150) * Player.MobSpawnModifier);
						MobPool.Instance.InitForRoom();

						while (Weight > 0) {
							MobHub Mobs = MobPool.Instance.Generate();

							foreach (Class M in Mobs.Types) {
								try {
									Weight = SpawnMob((Mob) M.NewInstance(), Room, Weight);
								} catch (InstantiationException | IllegalAccessException) {
									E.PrintStackTrace();
								}
							}
						}
					} 
				}
			} 

			foreach (Item Item in this.ItemsToSpawn) {
				Point Point = null;

				while (Point == null) {
					Point = this.GetRandomFreePoint(RegularRoom.GetType());
				}

				ItemHolder Holder = new ItemHolder(Item);
				Holder.GetItem().Generate();
				Holder.X = Point.X * 16 + Random.NewInt(-4, 4);
				Holder.Y = Point.Y * 16 + Random.NewInt(-4, 4);
				Holder.Add();
				this.Area.Add(Holder);
			}

			this.ItemsToSpawn.Clear();
		}

		private float SpawnMob(Mob Mob, Room Room, float Weight) {
			Weight -= Mob.GetWeight();
			Point Point;
			int I = 0;

			do {
				Point = Room.GetRandomDoorFreeCell();

				if (I++ > 40) {
					Log.Error("Failed to place " + Mob.GetClass() + " in room " + Room.GetClass());

					break;
				} 
			} while (!Dungeon.Level.CheckFor((int) Point.X, (int) Point.Y, Terrain.PASSABLE));

			if (I <= 40) {
				Mob.Generate();
				Dungeon.Area.Add(Mob);
				LevelSave.Add(Mob);
				Mob.Tp((float) Math.Floor(Point.X * 16), (float) Math.Floor(Point.Y * 16));
			} 

			return Weight;
		}

		protected Void SpawnEntities() {

		}

		protected Void Paint() {
			Log.Info("Painting...");

			if (Dungeon.Depth > 0) {
				if (Random.Chance(75)) {
					this.ItemsToSpawn.Add(new Coin());

					while (Random.Chance(10)) {
						this.ItemsToSpawn.Add(new Coin());
					}
				} 
			} 

			Painter Painter = this.GetPainter();

			if (Painter != null) {
				Painter.Paint(this, this.Rooms);
			} else {
				Log.Error("No painter!");
			}


			for (int I = this.Rooms.Size() - 1; I >= 0; I--) {
				Room Room = this.Rooms.Get(I);

				if (Room is HandmadeRoom && ((HandmadeRoom) Room).Data.Sub.Size() > 0) {
					this.Rooms.Remove(I);
				} 
			}
		}

		protected Void Build() {
			Builder Builder = this.GetBuilder();
			List<Room> Rooms = this.CreateRooms();

			if (Dungeon.Depth > -2 && (GameSave.RunId != 0 || Dungeon.Depth != 1)) {
				Collections.Shuffle(Rooms, new Java.Util.Random(ItemSelectState.StringToSeed(Random.GetSeed())));
			} 

			int Attempt = 0;

			do {
				Log.Info("Generating (attempt " + Attempt + ")...");

				foreach (Room Room in Rooms) {
					Room.GetConnected().Clear();
					Room.GetNeighbours().Clear();
				}

				List<Room> Rm = new List<>();
				Rm.AddAll(Rooms);
				this.Rooms = Builder.Build(Rm);

				if (this.Rooms == null) {
					Log.Error("Failed!");
					Dungeon.Area.Destroy();
					Dungeon.Area.Add(Dungeon.Level);
					Level.GENERATED = true;
					this.ItemsToSpawn.Clear();

					if (Attempt >= 10) {
						Log.Error("Too many attempts to generate a level! Trying a different room set!");
						Attempt = 0;
						Rooms = this.CreateRooms();

						if (Dungeon.Depth > -2 && (GameSave.RunId != 0 || Dungeon.Depth != 1)) {
							Collections.Shuffle(Rooms, new Java.Util.Random(ItemSelectState.StringToSeed(Random.GetSeed())));
						} 
					} 

					Attempt++;
				} 
			} while (this.Rooms == null);
		}

		protected List CreateRooms<Room> () {
			List<Room> Rooms = new List<>();

			if (this is CreepLevel) {
				Rooms.Add(new FloatingRoom());

				return Rooms;
			} 

			if (GameSave.RunId == 0 && Dungeon.Depth == 1) {
				Rooms.Add(new TutorialChasmRoom());
				Log.Info("Added tutorial chasm room");
			} 

			if (Dungeon.Depth > -1) {
				this.Entrance = EntranceRoomPool.Instance.Generate();
				this.Exit = this is BossLevel ? BossRoomPool.Instance.Generate() : EntranceRoomPool.Instance.Generate();
				((EntranceRoom) this.Exit).Exit = true;
				Rooms.Add(this.Entrance);
				Rooms.Add(this.Exit);
			} 

			if (Dungeon.Depth == 0) {
				Rooms.Add(new LampRoom());
			} else if (Dungeon.Depth == -3) {
				Rooms.Add(new HandmadeRoom("tutorial"));
			} else if (Dungeon.Depth == -2) {
				Rooms.Add(new HandmadeRoom("shops"));
			} else if (Dungeon.Depth == -1) {
				Rooms.Add(new HandmadeRoom("hub"));
			} 

			if (Dungeon.Depth > 0) {
				if (GlobalSave.IsFalse("all_npcs_saved") && (Random.Chance(25) || Version.Debug)) {
					Rooms.Add(new NpcSaveRoom());
				} 
			} 

			bool Bk = Random.GetSeed().Equals("BK");

			if (this is BossLevel) {
				Rooms.Add(new PrebossRoom());
			} 

			int Regular = Bk ? 0 : this.GetNumRegularRooms();
			int Special = Bk ? 0 : this.GetNumSpecialRooms();
			int Connection = this.GetNumConnectionRooms();
			int Secret = Bk ? 0 : this.GetNumSecretRooms();
			Log.Info("Creating r" + Regular + " sp" + Special + " c" + Connection + " sc" + Secret + " rooms");

			for (int I = 0; I < Regular; I++) {
				RegularRoom Room;

				do {
					Room = RegularRoom.Create();
				} while (!Room.SetSize(0, Regular - I));

				I += Room.GetSize().RoomValue - 1;
				Rooms.Add(Room);
			}

			SpecialRoom.Init();

			for (int I = 0; I < Special; I++) {
				SpecialRoom Room = SpecialRoom.Create();

				if (Room != null) {
					Rooms.Add(Room);
				} 
			}

			if (Dungeon.Depth > 0 && !Bk) {
				TreasureRoom Room = TreasureRoomPool.Instance.Generate();
				Room.Weapon = Random.Chance(50);
				Rooms.Add(Room);

				if ((GameSave.RunId == 1 || Random.Chance(50)) && (GameSave.RunId != 0 || Dungeon.Depth != 1)) {
					Log.Info("Adding shop");
					Rooms.Add(ShopRoomPool.Instance.Generate());
				} 
			} 

			for (int I = 0; I < Connection; I++) {
				Rooms.Add(ConnectionRoom.Create());
			}

			for (int I = 0; I < Secret; I++) {
				Rooms.Add(SecretRoomPool.Instance.Generate());
			}

			List<HandmadeRoom> HandmadeRooms = new List<>();

			foreach (Room Room in Rooms) {
				if (Room is HandmadeRoom && ((HandmadeRoom) Room).Data.Sub.Size() > 0) {
					HandmadeRooms.Add((HandmadeRoom) Room);
				} 
			}

			foreach (HandmadeRoom Room in HandmadeRooms) {
				Room.AddSubRooms(Rooms);
			}

			return Rooms;
		}

		protected abstract Painter GetPainter();

		protected Builder GetBuilder() {
			if (Dungeon.Depth <= -1 || this is CreepLevel) {
				return new SingleRoomBuilder();
			} else {
				float R = Random.NewFloat();

				if (R < 0.33f) {
					LineBuilder Builder = new LineBuilder();

					if (GameSave.RunId == 0 && Dungeon.Depth <= 2) {
						Builder.SetPathLength(2, { 0, 1, 0 });
						Builder.SetExtraConnectionChance(0);

						if (Dungeon.Depth == 1) {
							Builder.SetAngle(90);
						} 
					} 

					return Builder;
				} else if (R < 0.66f) {
					return new LoopBuilder();
				} else {
					return new CastleBuilder();
				}

			}

		}

		protected int GetNumRegularRooms() {
			return Dungeon.Depth <= 0 ? 0 : (this is BossLevel ? 1 : Random.NewInt(3, 6));
		}

		protected int GetNumSpecialRooms() {
			return 0;
		}

		protected int GetNumSecretRooms() {
			return Dungeon.Depth <= 0 ? 0 : Random.NewInt(1, 3);
		}

		protected int GetNumConnectionRooms() {
			return 0;
		}
	}
}
