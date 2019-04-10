using System.Collections.Generic;
using BurningKnight.level.biome;
using BurningKnight.level.builders;
using BurningKnight.level.rooms;
using BurningKnight.level.rooms.entrance;
using BurningKnight.state;
using Lens.entity;
using Lens.util;
using Lens.util.math;
using MonoGame.Extended.Collections;

namespace BurningKnight.level {
	public class RegularLevel : Level {
		private List<RoomDef> rooms;

		public RegularLevel(BiomeInfo biome) : base(biome) {
			
		}

		public RegularLevel() : base(null) {
			
		}
		
		public void Generate(Area area, int Attempt) {
			rooms = null;

			Build();
			Paint();

			if (rooms == null) {
				return;
			}

			TileUp();
			CreateBody();
			Log.Info("Done!");
		}

		protected void Paint() {
			Log.Info("Painting...");

			var Painter = GetPainter();
			Painter.Paint(this, rooms);
			
			foreach (var def in rooms) {
				var room = new Room();

				room.Type = RoomDef.DecideType(def.GetType());
				room.MapX = def.Left;
				room.MapY = def.Top;
				room.MapW = def.GetWidth();
				room.MapH = def.GetHeight();
				
				Area.Add(room);
			}
		}

		protected void Build() {
			var Builder = GetBuilder();
			var Rooms = CreateRooms();

			Rooms = (List<RoomDef>) Rooms.Shuffle(Random.Generator);

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
						Rooms = (List<RoomDef>) Rooms.Shuffle(Random.Generator);
					}

					Attempt++;
				}
			} while (rooms == null);
		}

		protected virtual List<RoomDef> CreateRooms() {
			var Rooms = new List<RoomDef>();

			var Entrance = RoomRegistry.Generate(RoomType.Entrance);
			var Exit = (ExitRoom) RoomRegistry.Generate(RoomType.Exit);
			Exit.To = Run.Depth + 1;
			
			Rooms.Add(Entrance);
			Rooms.Add(Exit);

			var Regular = GetNumRegularRooms();
			var Special = GetNumSpecialRooms();
			var Connection = GetNumConnectionRooms();
			var Secret = GetNumSecretRooms();
			Log.Info("Creating r" + Regular + " sp" + Special + " c" + Connection + " sc" + Secret + " rooms");

			for (var I = 0; I < Regular; I++) {
				Rooms.Add(RoomRegistry.Generate(RoomType.Regular));
			}

			for (var I = 0; I < Special; I++) {
				var Room = RoomRegistry.Generate(RoomType.Special);
				if (Room != null) Rooms.Add(Room);
			}

			Rooms.Add(RoomRegistry.Generate(RoomType.Treasure));

			for (var I = 0; I < Connection; I++) {
				Rooms.Add(RoomRegistry.Generate(RoomType.Connection));
			}

			for (var I = 0; I < Secret; I++) {
				Rooms.Add(RoomRegistry.Generate(RoomType.Secret));
			}

			return Rooms;
		}

		protected Painter GetPainter() {
			return new Painter();
		}

		protected virtual Builder GetBuilder() {
			if (true) {
				return new LineBuilder();
			}
			
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
			return 5;
		}

		protected int GetNumSpecialRooms() {
			return 0;
		}

		protected int GetNumSecretRooms() {
			return Run.Depth <= 0 ? 0 : 1;
		}

		protected int GetNumConnectionRooms() {
			return Random.Int(3);
		}
	}
}