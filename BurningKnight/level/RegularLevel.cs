using System.Collections.Generic;
using BurningKnight.entity.room;
using BurningKnight.level.biome;
using BurningKnight.level.builders;
using BurningKnight.level.rooms;
using BurningKnight.level.rooms.entrance;
using BurningKnight.level.rooms.preboss;
using BurningKnight.level.rooms.special;
using BurningKnight.level.rooms.trap;
using BurningKnight.save;
using BurningKnight.state;
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

		public override int GetPadding() {
			return 10;
		}

		public void Generate() {
			rooms = null;
			ItemsToSpawn = new List<string>();

			if (Run.Depth > 0) {
				ItemsToSpawn.Add("bk:bomb");
			}
			
			Build();
			Paint();

			if (rooms == null) {
				return;
			}

			TileUp();
			CreateBody();
			CreateDestroyableBody();
			LoadPassable();
			
			Log.Info("Done!");
		}

		protected void Paint() {
			Log.Info("Painting...");
			var p = GetPainter();
			LevelSave.BiomeGenerated.ModifyPainter(p);
			p.Paint(this, rooms);
			
			foreach (var def in rooms) {
				var room = new Room();

				room.Type = RoomDef.DecideType(def, def.GetType());
				room.MapX = def.Left;
				room.MapY = def.Top;
				room.MapW = def.GetWidth();
				room.MapH = def.GetHeight();
				
				Area.Add(room);

				def.ModifyRoom(room);

				room.Generate();
			}
		}

		protected void Build() {
			var Builder = GetBuilder();
			var Rooms = CreateRooms();

			Rooms = (List<RoomDef>) Rooms.Shuffle(Random.Generator);

			var Attempt = 0;

			do {
				Log.Info($"Generating (attempt {Attempt}, seed {Random.Seed})...");

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

		private bool IsFinal() {
			return Run.Depth == 3;
		}

		protected virtual List<RoomDef> CreateRooms() {
			var Rooms = new List<RoomDef>();
			var biome = LevelSave.BiomeGenerated;
			var final = IsFinal();

			if (final) {
				Log.Info("Prepare for the final!");
			}
			
			Rooms.Add(false ? new BossTestRoom() : RoomRegistry.Generate(RoomType.Entrance, biome));

			var Regular = final ? 0 : GetNumRegularRooms();
			var Special = final ? 0 : GetNumSpecialRooms();
			var trap = final ? 0 : GetNumTrapRooms();
			var Connection = final ? 1 : GetNumConnectionRooms();
			var Secret = final ? 0 : GetNumSecretRooms();
			
			Log.Info($"Creating r{Regular} sp{Special} c{Connection} sc{Secret} t{trap} rooms");

			for (var I = 0; I < Regular; I++) {
				Rooms.Add(RoomRegistry.Generate(RoomType.Regular, biome));
			}

			for (var i = 0; i < trap; i++) {
				Rooms.Add(RoomRegistry.Generate(RoomType.Trap, biome));
			}

			for (var I = 0; I < Special; I++) {
				var Room = RoomRegistry.Generate(RoomType.Special, biome);
				if (Room != null) Rooms.Add(Room);
			}
			
			for (var I = 0; I < Connection; I++) {
				Rooms.Add(RoomRegistry.Generate(RoomType.Connection, biome));
			}

			for (var I = 0; I < Secret; I++) {
				Rooms.Add(RoomRegistry.Generate(RoomType.Secret, biome));
			}

			if (!final) {
				Rooms.Add(RoomRegistry.Generate(RoomType.Treasure, biome));
				Rooms.Add(RoomRegistry.Generate(RoomType.Shop, biome));
			}
			
			Rooms.Add(RoomRegistry.Generate(RoomType.Boss, biome));
			Rooms.Add(new PrebossRoom());
			
			TombRoom.Insert(Rooms);
			
			LevelSave.BiomeGenerated.ModifyRooms(Rooms);
			
			return Rooms;
		}

		protected virtual Painter GetPainter() {
			return new Painter();
		}

		protected virtual Builder GetBuilder() {
			if (IsFinal()) {
				return new LineBuilder();
			}
			
			var R = Random.Float();

			if (R < 0.33f) {
				return new LineBuilder();
			}

			if (R < 0.66f) {
				return new LoopBuilder();
			}
			
			return new CastleBuilder();
		}

		protected virtual int GetNumRegularRooms() {
			return 5;
		}

		protected virtual int GetNumTrapRooms() {
			return 1;
		}

		protected virtual int GetNumSpecialRooms() {
			return 1;
		}

		protected virtual int GetNumSecretRooms() {
			return Run.Depth <= 0 ? 0 : 1;
		}

		protected int GetNumConnectionRooms() {
			return 0;
		}
	}
}