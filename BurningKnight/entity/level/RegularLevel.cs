using System.Collections.Generic;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.item;
using BurningKnight.entity.level.builders;
using BurningKnight.entity.level.entities;
using BurningKnight.entity.level.painters;
using BurningKnight.entity.level.rooms;
using BurningKnight.entity.level.rooms.connection;
using BurningKnight.entity.level.rooms.regular;
using BurningKnight.entity.level.rooms.special;
using BurningKnight.entity.pool;
using BurningKnight.entity.pool.room;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.util;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.util.math;

namespace BurningKnight.entity.level {
	public abstract class RegularLevel : Level {
		private List<RoomDef> rooms;

		public RegularLevel(string tileset) {
			var animation = Animations.Get(tileset);
		}
		
		public void Generate(Area area, int Attempt) {
			rooms = null;

			Build();
			Paint();

			if (rooms == null) {
				Log.Error("NO ROOMS!");
				return;
			}

			Log.Info("Done!");
		}

		protected void Paint() {
			Log.Info("Painting...");

			var Painter = GetPainter();
			Painter.Paint(this, rooms);				
		}

		protected void Build() {
			var Builder = new LineBuilder(); // GetBuilder();
			var Rooms = CreateRooms();

			// if (Dungeon.Depth > -2 && (GameSave.RunId != 0 || Dungeon.Depth != 1)) Collections.Shuffle(Rooms, new Java.Util.Random(ItemSelectState.StringToSeed(Random.GetSeed())));

			var Attempt = 0;

			do {
				Log.Info("Generating (attempt " + Attempt + ")...");

				foreach (var Room in Rooms) {
					Room.Connected.Clear();
					Room.Neighbours.Clear();
				}

				var Rm = new List<RoomDef>();
				Rm.AddRange(Rooms);
				rooms = Builder.Build(Rm);

				if (rooms == null) {
					Log.Error("Failed!");
					Area.Destroy();
					Area.Add(Run.Level);

					if (Attempt >= 10) {
						Log.Error("Too many attempts to generate a level! Trying a different room set!");
						Attempt = 0;
						Rooms = CreateRooms();

						// if (Dungeon.Depth > -2 && (GameSave.RunId != 0 || Dungeon.Depth != 1)) Collections.Shuffle(Rooms, new Java.Util.Random(ItemSelectState.StringToSeed(Random.GetSeed())));
					}

					Attempt++;
				}
			} while (rooms == null);
		}

		protected List<RoomDef> CreateRooms() {
			var Rooms = new List<RoomDef>();

			var Entrance = EntranceRoomPool.Instance.Generate();
			var Exit = EntranceRoomPool.Instance.Generate();
			
			Exit.Exit = true;
			Rooms.Add(Entrance);
			Rooms.Add(Exit);

			/*if (Dungeon.Depth > 0)
				if (GlobalSave.IsFalse("all_npcs_saved") && (Random.Chance(25) || Engine.Version.Debug))
					Rooms.Add(new NpcSaveRoomDef());*/

			var Regular = GetNumRegularRooms();
			var Special = GetNumSpecialRooms();
			var Connection = GetNumConnectionRooms();
			var Secret = GetNumSecretRooms();
			Log.Info("Creating r" + Regular + " sp" + Special + " c" + Connection + " sc" + Secret + " rooms");

			for (var I = 0; I < Regular; I++) {
				Rooms.Add(RegularRoomPool.Instance.Generate());
			}

			SpecialRoom.Init();

			for (var I = 0; I < Special; I++) {
				var Room = SpecialRoom.Create();
				if (Room != null) Rooms.Add(Room);
			}

			if (Run.Depth > 0) {
				var Room = TreasureRoomPool.Instance.Generate();
				Rooms.Add(Room);

				if ((Run.Id == 1 || Random.Chance(50)) && (Run.Id != 0 || Run.Depth != 1)) {
					Log.Info("Adding shop");
					Rooms.Add(ShopRoomPool.Instance.Generate());
				}
			}

			for (var I = 0; I < Connection; I++) Rooms.Add(ConnectionRoomDef.Create());

			for (var I = 0; I < Secret; I++) Rooms.Add(SecretRoomPool.Instance.Generate());

			return Rooms;
		}

		protected Painter GetPainter() {
			return new Painter();
		}

		protected Builder GetBuilder() {
			if (Run.Depth <= -1) {
				return new SingleRoomBuilder();
			}

			var R = Random.Float();

			if (R < 0.33f) {
				var Builder = new LineBuilder();

				if (Run.Id == 0 && Run.Depth <= 2) {
					Builder.SetPathLength(2, new [] { 0f, 1f, 0f });
					Builder.SetExtraConnectionChance(0);

					if (Run.Depth == 1) {
						Builder.SetAngle(90);
					}
				}

				return Builder;
			}

			if (R < 0.66f) return new LoopBuilder();
			return new CastleBuilder();
		}

		protected int GetNumRegularRooms() {
			return Random.Int(3, 6);
		}

		protected int GetNumSpecialRooms() {
			return 0;
		}

		protected int GetNumSecretRooms() {
			return Run.Depth <= 0 ? 0 : 1;
		}

		protected int GetNumConnectionRooms() {
			return 0;
		}
	}
}