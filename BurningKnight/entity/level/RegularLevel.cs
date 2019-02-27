using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.item;
using BurningKnight.entity.level.builders;
using BurningKnight.entity.level.entities;
using BurningKnight.entity.level.entities.chest;
using BurningKnight.entity.level.levels;
using BurningKnight.entity.level.levels.creep;
using BurningKnight.entity.level.painters;
using BurningKnight.entity.level.rooms;
using BurningKnight.entity.level.rooms.connection;
using BurningKnight.entity.level.rooms.entrance;
using BurningKnight.entity.level.rooms.regular;
using BurningKnight.entity.level.rooms.special;
using BurningKnight.entity.level.rooms.treasure;
using BurningKnight.entity.level.save;
using BurningKnight.entity.pool;
using BurningKnight.entity.pool.room;
using BurningKnight.game.state;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level {
	public abstract class RegularLevel : Level {
		public override void Generate(int Attempt) {
			Random.Random.SetSeed(ItemSelectState.StringToSeed(Random.GetSeed()) + Dungeon.Depth * 128 + Attempt);
			Player.All.Clear();
			Mob.All.Clear();
			ItemHolder.All.Clear();
			Chest.All.Clear();
			Mob.All.Clear();
			PlayerSave.All.Clear();
			LevelSave.All.Clear();
			Dungeon.Area.Destroy();
			Dungeon.Area.Add(Dungeon.Level);
			Level.GENERATED = true;
			this.ItemsToSpawn.Clear();

			if (Dungeon.Depth > 0)
				for (var I = 0; I < Random.NewInt(4); I++)
					this.ItemsToSpawn.Add(new Bomb());

			Build();
			Paint();

			if (this.Rooms == null) Log.Error("NO ROOMS!");

			Log.Info("Done painting");
			this.LoadPassable();
			Log.Info("Spawning entities...");

			if (!Random.GetSeed().Equals("BK")) {
				SpawnLevelEntities();
				SpawnEntities();
			}

			Log.Info("Done!");
		}

		protected void SpawnLevelEntities() {
			this.Free = new bool[GetSize()];

			if (Dungeon.Depth > 0) {
				MobPool.Instance.InitForFloor();
				Log.Info("Spawn modifier is x" + Player.MobSpawnModifier);

				foreach (RoomDef Room in this.Rooms)
					if (Room is RegularRoomDef && !(Room is PrebossRoomDef) || Room is TreasureRoomDef && Random.Chance(20)) {
						var Weight = (Random.NewFloat(1f, 3f) + Room.GetWidth() * Room.GetHeight() / 150) * Player.MobSpawnModifier;
						MobPool.Instance.InitForRoom();

						while (Weight > 0) {
							var Mobs = MobPool.Instance.Generate();

							foreach (Class M in Mobs.Types) {
								try {
									Weight = SpawnMob((Mob) M.NewInstance(), Room, Weight);
								}
								catch (InstantiationException |

								IllegalAccessException) {
									E.PrintStackTrace();
								}
							}
						}
					}
			}

			foreach (Item Item in this.ItemsToSpawn) {
				Point Point = null;

				while (Point == null) Point = this.GetRandomFreePoint(RegularRoomDef.GetType());

				var Holder = new ItemHolder(Item);
				Holder.GetItem().Generate();
				Holder.X = Point.X * 16 + Random.NewInt(-4, 4);
				Holder.Y = Point.Y * 16 + Random.NewInt(-4, 4);
				Holder.Add();
				Area.Add(Holder);
			}

			this.ItemsToSpawn.Clear();
		}

		private float SpawnMob(Mob Mob, RoomDef roomDef, float Weight) {
			Weight -= Mob.GetWeight();
			Point Point;
			var I = 0;

			do {
				Point = roomDef.GetRandomDoorFreeCell();

				if (I++ > 40) {
					Log.Error("Failed to place " + Mob.GetClass() + " in room " + roomDef.GetClass());

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

		protected void SpawnEntities() {
		}

		protected void Paint() {
			Log.Info("Painting...");

			if (Dungeon.Depth > 0)
				if (Random.Chance(75)) {
					this.ItemsToSpawn.Add(new Coin());

					while (Random.Chance(10)) this.ItemsToSpawn.Add(new Coin());
				}

			var Painter = GetPainter();

			if (Painter != null)
				Painter.Paint(this, this.Rooms);
			else
				Log.Error("No painter!");


			for (var I = this.Rooms.Size() - 1; I >= 0; I--) {
				RoomDef roomDef = this.Rooms.Get(I);

				if (roomDef is HandmadeRoomDef && ((HandmadeRoomDef) roomDef).Data.Sub.Size() > 0) this.Rooms.Remove(I);
			}
		}

		protected void Build() {
			var Builder = GetBuilder();
			List<RoomDef> Rooms = this.CreateRooms();

			if (Dungeon.Depth > -2 && (GameSave.RunId != 0 || Dungeon.Depth != 1)) Collections.Shuffle(Rooms, new Java.Util.Random(ItemSelectState.StringToSeed(Random.GetSeed())));

			var Attempt = 0;

			do {
				Log.Info("Generating (attempt " + Attempt + ")...");

				foreach (RoomDef Room in Rooms) {
					Room.GetConnected().Clear();
					Room.GetNeighbours().Clear();
				}

				List<RoomDef> Rm = new List<>();
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

						if (Dungeon.Depth > -2 && (GameSave.RunId != 0 || Dungeon.Depth != 1)) Collections.Shuffle(Rooms, new Java.Util.Random(ItemSelectState.StringToSeed(Random.GetSeed())));
					}

					Attempt++;
				}
			} while (this.Rooms == null);
		}

		protected List CreateRooms<Room>() {
			List<Room> Rooms = new List<>();

			if (this is CreepLevel) {
				Rooms.Add(new FloatingRoomDef());

				return Rooms;
			}

			if (GameSave.RunId == 0 && Dungeon.Depth == 1) {
				Rooms.Add(new TutorialChasmRoomDef());
				Log.Info("Added tutorial chasm room");
			}

			if (Dungeon.Depth > -1) {
				Entrance = EntranceRoomPool.Instance.Generate();
				Exit = this is BossLevel ? BossRoomPool.Instance.Generate() : EntranceRoomPool.Instance.Generate();
				((EntranceRoomDef) Exit).Exit = true;
				Rooms.Add(Entrance);
				Rooms.Add(Exit);
			}

			if (Dungeon.Depth == 0)
				Rooms.Add(new LampRoomDef());
			else if (Dungeon.Depth == -3)
				Rooms.Add(new HandmadeRoomDef("tutorial"));
			else if (Dungeon.Depth == -2)
				Rooms.Add(new HandmadeRoomDef("shops"));
			else if (Dungeon.Depth == -1) Rooms.Add(new HandmadeRoomDef("hub"));

			if (Dungeon.Depth > 0)
				if (GlobalSave.IsFalse("all_npcs_saved") && (Random.Chance(25) || Version.Debug))
					Rooms.Add(new NpcSaveRoomDef());

			var Bk = Random.GetSeed().Equals("BK");

			if (this is BossLevel) Rooms.Add(new PrebossRoomDef());

			var Regular = Bk ? 0 : GetNumRegularRooms();
			var Special = Bk ? 0 : GetNumSpecialRooms();
			var Connection = GetNumConnectionRooms();
			var Secret = Bk ? 0 : GetNumSecretRooms();
			Log.Info("Creating r" + Regular + " sp" + Special + " c" + Connection + " sc" + Secret + " rooms");

			for (var I = 0; I < Regular; I++) {
				RegularRoomDef roomDef;

				do {
					roomDef = RegularRoomDef.Create();
				} while (!roomDef.SetSize(0, Regular - I));

				I += roomDef.GetSize().RoomValue - 1;
				Rooms.Add(roomDef);
			}

			SpecialRoomDef.Init();

			for (var I = 0; I < Special; I++) {
				var Room = SpecialRoomDef.Create();

				if (Room != null) Rooms.Add(Room);
			}

			if (Dungeon.Depth > 0 && !Bk) {
				var Room = TreasureRoomPool.Instance.Generate();
				Room.Weapon = Random.Chance(50);
				Rooms.Add(Room);

				if ((GameSave.RunId == 1 || Random.Chance(50)) && (GameSave.RunId != 0 || Dungeon.Depth != 1)) {
					Log.Info("Adding shop");
					Rooms.Add(ShopRoomPool.Instance.Generate());
				}
			}

			for (var I = 0; I < Connection; I++) Rooms.Add(ConnectionRoomDef.Create());

			for (var I = 0; I < Secret; I++) Rooms.Add(SecretRoomPool.Instance.Generate());

			List<HandmadeRoomDef> HandmadeRooms = new List<>();

			foreach (Room Room in Rooms)
				if (Room is HandmadeRoomDef && ((HandmadeRoomDef) Room).Data.Sub.Size() > 0)
					HandmadeRooms.Add((HandmadeRoomDef) Room);

			foreach (HandmadeRoomDef Room in HandmadeRooms) Room.AddSubRooms(Rooms);

			return Rooms;
		}

		protected abstract Painter GetPainter();

		protected Builder GetBuilder() {
			if (Dungeon.Depth <= -1 || this is CreepLevel) return new SingleRoomBuilder();

			var R = Random.NewFloat();

			if (R < 0.33f) {
				var Builder = new LineBuilder();

				if (GameSave.RunId == 0 && Dungeon.Depth <= 2) {
					Builder.SetPathLength(2,  {
						0, 1, 0
					});
					Builder.SetExtraConnectionChance(0);

					if (Dungeon.Depth == 1) Builder.SetAngle(90);
				}

				return Builder;
			}

			if (R < 0.66f)
				return new LoopBuilder();
			return new CastleBuilder();
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