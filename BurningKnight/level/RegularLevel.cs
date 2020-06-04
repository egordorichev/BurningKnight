using System.Collections.Generic;
using BurningKnight.entity.creature.npc;
using BurningKnight.entity.room;
using BurningKnight.level.biome;
using BurningKnight.level.builders;
using BurningKnight.level.rooms;
using BurningKnight.level.rooms.challenge;
using BurningKnight.level.rooms.darkmarket;
using BurningKnight.level.rooms.entrance;
using BurningKnight.level.rooms.payed;
using BurningKnight.level.rooms.preboss;
using BurningKnight.level.rooms.regular;
using BurningKnight.level.rooms.scourged;
using BurningKnight.level.rooms.secret;
using BurningKnight.level.rooms.shop;
using BurningKnight.level.rooms.shop.sub;
using BurningKnight.level.rooms.special;
using BurningKnight.level.rooms.special.minigame;
using BurningKnight.level.rooms.special.shop;
using BurningKnight.level.rooms.spiked;
using BurningKnight.level.rooms.trap;
using BurningKnight.level.tile;
using BurningKnight.level.variant;
using BurningKnight.save;
using BurningKnight.state;
using Lens.util;
using Lens.util.math;
using MonoGame.Extended.Collections;
using Builder = BurningKnight.level.builders.Builder;

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

		public bool Generate() {
			Run.Level = this;
			rooms = null;
			ItemsToSpawn = new List<string>();
			Variant = VariantRegistry.Generate(LevelSave.BiomeGenerated.Id);

			if (Variant == null) {
				Variant = new RegularLevelVariant();
			}

			if (Run.Depth > 0) {
				if (GlobalSave.IsTrue("saved_npc")) {
					for (var i = 0; i < Rnd.Int(1, Run.Depth); i++) {
						ItemsToSpawn.Add("bk:emerald");
					}
				}
			}
			
			Build();

			if (!Paint()) {
				return false;
			}

			if (rooms == null) {
				return false;
			}

			TileUp();
			CreateBody();
			CreateDestroyableBody();
			LoadPassable();

			LevelSave.ResetGen();
			Log.Info("Done!");

			return true;
		}

		protected bool Paint() {
			Log.Info("Painting...");
			var p = GetPainter();
			LevelSave.BiomeGenerated.ModifyPainter(this, p);
			
			return p.Paint(this, rooms);
		}

		protected void Build() {
			var Builder = GetBuilder();
			var Rooms = CreateRooms();

			Rooms = (List<RoomDef>) Rooms.Shuffle(Rnd.Generator);

			var Attempt = 0;

			do {
				Log.Info($"Generating (attempt {Attempt}, seed {Rnd.Seed})...");

				foreach (var Room in Rooms) {
					Room.Connected.Clear();
					Room.Neighbours.Clear();
				} 

				var Rm = new List<RoomDef>();
				Rm.AddRange(Rooms);
				rooms = Builder.Build(Rm);

				var a = rooms == null;
				var b = false;
				
				if (!a) {
					foreach (var r in Rm) {
						if (r.IsEmpty()) {
							Log.Error("Found an empty room!");
							b = true;
							break;
						}
					}
				}
				
				if (a || b) {
					rooms = null;
				
					Log.Error($"Failed! {Builder.GetType().Name}");
					Area.Destroy();
					Area.Add(Run.Level);
					LevelSave.FailedAttempts++;
					Builder = GetBuilder();

					if (Attempt >= 10) {
						Log.Error("Too many attempts to generate a level! Trying a different room set!");
						Attempt = 0;
						Rooms = CreateRooms();
						Rooms = (List<RoomDef>) Rooms.Shuffle(Rnd.Generator);
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
			var rush = Run.Type == RunType.BossRush;
			var first = Run.Depth % 2 == 1;
			var loop = Run.Loop > 0;

			if (!rush && biome is DesertBiome) {
				rooms.Add(new DesertWellRoom());
			}

			if (final) {
				Log.Info("Prepare for the final!");
			}
			
			Log.Info($"Generating a level for {biome.Id} biome");
			
			rooms.Add(new EntranceRoom());
			
			if (Run.Depth == 5 && LevelSave.GenerateMarket && Run.Loop == 0) {
				rooms.Add(new ShopRoom());
				rooms.Add(new ExitRoom());

				if (GlobalSave.IsTrue(ShopNpc.Gobetta) && Rnd.Chance(10)) {
					rooms.Add(new GobettaShopRoom());
				}

				if (Rnd.Chance(2)) {
					rooms.Add(new TrashGoblinRoom());
				}

				if (Rnd.Chance(3)) {
					rooms.Add(new ChestMinigameRoom());
				}

				if (Rnd.Chance(4)) {
					rooms.Add(new VendingRoom());
				}

				if (GlobalSave.IsTrue(ShopNpc.Roger) && Rnd.Chance(10)) {
					rooms.Add(new RogerShopRoom());
				}
				
				if (Rnd.Chance(30)) {
					rooms.Add(RoomRegistry.Generate(RoomType.Secret, biome));
				}
				
				rooms.Add(new EmptyRoom());
				return rooms;
			}

			if (!rush && !loop) {
				if (Run.Depth == 2) {
					rooms.Add(new SecretKeyRoom());
				} else if (Run.Depth == 4) {
					rooms.Add(new ClawMinigameRoom());
				}
			}

			if (!final) {
				var cn = LevelSave.XL ? 2 : 1;

				var regular = rush || final ? 0 : biome.GetNumRegularRooms() * cn;
				var special = rush || final ? 0 : biome.GetNumSpecialRooms() * cn;
				var trap = rush || final ? 0 : biome.GetNumTrapRooms();
				var connection = rush || final ? 1 : GetNumConnectionRooms();
				var secret = rush || final ? 0 : biome.GetNumSecretRooms() * cn;

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

				if (!rush && !final && Run.Type != RunType.Challenge && !loop) {
					if (!LevelSave.GenerateShops && first) {
						if (LevelSave.XL) {
							rooms.Add(RoomRegistry.Generate(RoomType.Treasure, biome));
						}

						rooms.Add(RoomRegistry.Generate(RoomType.Treasure, biome));
					}

					if (!LevelSave.GenerateTreasure && (!first || LevelSave.GenerateShops)) {
						rooms.Add(RoomRegistry.Generate(RoomType.Shop, biome));
					}
				}

				if (LevelSave.GenerateTreasure && !first) {
					rooms.Add(RoomRegistry.Generate(RoomType.Treasure, biome));
				}

				if (!LevelSave.GenerateShops && loop && Run.Depth == 1 && Run.Type != RunType.Challenge) {
					rooms.Add(RoomRegistry.Generate(RoomType.Treasure, biome));
				}

				if (rush) {
					rooms.Add(RoomRegistry.Generate(RoomType.Boss, biome));
					rooms.Add(new PrebossRoom());

					if (Run.Depth > 1 && Run.Depth < 11) {
						if (!LevelSave.GenerateShops) {
							rooms.Add(RoomRegistry.Generate(RoomType.Connection, biome));
							rooms.Add(RoomRegistry.Generate(RoomType.Treasure, biome));
						} else {
							rooms.Add(RoomRegistry.Generate(RoomType.Connection, biome));
							rooms.Add(RoomRegistry.Generate(RoomType.Shop, biome));
						}	
					}
				} else if (first) {
					rooms.Add(new ExitRoom());
				} else {
					rooms.Add(RoomRegistry.Generate(RoomType.Boss, biome));
					rooms.Add(new PrebossRoom());

					if (Run.Depth < 10) {
						rooms.Add(RoomRegistry.Generate(RoomType.Granny, biome));
						rooms.Add(RoomRegistry.Generate(RoomType.OldMan, biome));
					}
				}

				if (!rush) {
					if (Rnd.Chance(95)) {
						if (Rnd.Chance(2 + Run.Scourge * 5)) {
							rooms.Add(new ScourgedRoom());
						} else {
							if (Rnd.Chance()) {
								rooms.Add(new ChallengeRoom());
							} else {
								rooms.Add(new SpikedRoom());
							}
						}
					}

					var addDarkMarket = (Run.Depth > 2 && Rnd.Chance(10) && GameSave.IsFalse("saw_blackmarket"));

					if (addDarkMarket) {
						rooms.Add(new DarkMarketEntranceRoom());
						rooms.Add(new DarkMarketRoom());
					}

					if (!addDarkMarket && Rnd.Chance(1)) {
						secret--;
						rooms.Add(new SecretDarkMarketEntranceRoom());
						rooms.Add(new DarkMarketRoom());
					}

					for (var I = 0; I < secret; I++) {
						rooms.Add(RoomRegistry.Generate(RoomType.Secret, biome));
					}

					if (Rnd.Chance()) {
						var c = Rnd.Int(0, 3);

						for (var i = 0; i < c; i++) {
							rooms.Add(RoomRegistry.Generate(RoomType.SubShop, biome));
						}
					}

					if (NpcSaveRoom.ShouldBeAdded()) {
						rooms.Add(new NpcSaveRoom());
						rooms.Add(new NpcKeyRoom());
					}

					TombRoom.Insert(rooms);
					biome.ModifyRooms(rooms);
				}
			} else {
				rooms.Add(RoomRegistry.Generate(RoomType.Boss, biome));
				rooms.Add(new PrebossRoom());
			}

			return rooms;
		}

		protected virtual Painter GetPainter() {
			return new Painter();
		}

		protected virtual Builder GetBuilder() {
			Builder builder;

			if (IsFinal() || Run.Type == RunType.BossRush) {
				builder = new LineBuilder();
			} else {
				builder = LevelSave.BiomeGenerated.GetBuilder();

				if (builder is RegularBuilder b) {
					if (LevelSave.BiomeGenerated.Id == Biome.Ice) {
						b.SetTunnelLength(new float[] {4, 6, 4}, new float[] {1, 3, 1});
					} else if (GetFilling() == Tile.Chasm) {
						b.SetTunnelLength(new float[] {4, 3, 4}, new float[] {1, 3, 1});
					}
				}
			}

			return builder;
		}

		protected int GetNumConnectionRooms() {
			return 0;
		}
	}
}