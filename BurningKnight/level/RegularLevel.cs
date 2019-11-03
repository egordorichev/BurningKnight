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

				if (GlobalSave.IsTrue("saved_npc")) {
					for (var i = 0; i < Random.Int(1, Run.Depth); i++) {
						ItemsToSpawn.Add("bk:emerald");
					}
				}
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
				if (!def.ConvertToEntity()) {
					continue;
				}
				
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
			return Run.Depth == Run.ContentEndDepth;
		}

		protected virtual List<RoomDef> CreateRooms() {
			var rooms = new List<RoomDef>();
			var biome = LevelSave.BiomeGenerated;
			var final = IsFinal();
			var first = Run.Depth % 2 == 1;

			if (final) {
				Log.Info("Prepare for the final!");
			}
			
			rooms.Add(first ? new EntranceRoom() : new PortalEntranceRoom());

			var regular = final ? 0 : GetNumRegularRooms();
			var special = final ? 0 : GetNumSpecialRooms();
			var trap = final ? 0 : GetNumTrapRooms();
			var connection = final ? 1 : GetNumConnectionRooms();
			var secret = final ? 0 : GetNumSecretRooms();
			
			Log.Info($"Creating r{regular} sp{special} c{connection} sc{secret} t{trap} rooms");

			for (var I = 0; I < regular; I++) {
				rooms.Add(RoomRegistry.Generate(RoomType.Regular, biome));
			}

			for (var i = 0; i < trap; i++) {
				rooms.Add(RoomRegistry.Generate(RoomType.Trap, biome));
			}

			for (var I = 0; I < special; I++) {
				var room = RoomRegistry.Generate(RoomType.Special, biome);
				if (room != null) rooms.Add(room);
			}
			
			for (var I = 0; I < connection; I++) {
				rooms.Add(RoomRegistry.Generate(RoomType.Connection, biome));
			}

			for (var I = 0; I < secret; I++) {
				rooms.Add(RoomRegistry.Generate(RoomType.Secret, biome));
			}

			if (!final) {
				rooms.Add(RoomRegistry.Generate(RoomType.Treasure, biome));

				if (!first) {
					rooms.Add(RoomRegistry.Generate(RoomType.Shop, biome));
				}
			}

			if (first) {
				rooms.Add(new ExitRoom());				
			} else {
				rooms.Add(RoomRegistry.Generate(RoomType.Boss, biome));
				rooms.Add(new PrebossRoom());	
			}

			if (NpcSaveRoom.ShouldBeAdded()) {
				rooms.Add(new NpcSaveRoom());
				rooms.Add(new NpcKeyRoom());
			}
			
			TombRoom.Insert(rooms);
			
			LevelSave.BiomeGenerated.ModifyRooms(rooms);
			
			return rooms;
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
			return Run.Depth + 2;
		}

		protected virtual int GetNumTrapRooms() {
			return Random.Int(0, 2);
		}

		protected virtual int GetNumSpecialRooms() {
			return Random.Int(0, 2);
		}

		protected virtual int GetNumSecretRooms() {
			return Run.Depth <= 0 ? 0 : 1;
		}

		protected int GetNumConnectionRooms() {
			return 0;
		}
	}
}