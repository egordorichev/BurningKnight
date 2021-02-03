using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BurningKnight.assets.items;
using BurningKnight.entity.item;
using BurningKnight.entity.projectile;
using BurningKnight.level;
using BurningKnight.level.biome;
using BurningKnight.level.cutscene;
using BurningKnight.level.hall;
using BurningKnight.level.tile;
using BurningKnight.level.tutorial;
using BurningKnight.level.walls;
using BurningKnight.physics;
using BurningKnight.state;
using Lens.entity;
using Lens.util;
using Lens.util.file;
using Lens.util.math;

namespace BurningKnight.save {
	public class LevelSave : EntitySaver {
		public static int FailedAttempts;
		public static List<int> Fails = new List<int>();
		public static bool XL;
		public static float ChestRewardChance;
		public static float MimicChance;
		public static bool GenerateMarket;
		public static bool GenerateShops;
		public static bool GenerateTreasure;
		public static bool MeleeOnly;
		public static float MobDestructionChance;

		private static int I;

		public override void Save(Area area, FileWriter writer, bool old) {
			SmartSave(area.Tagged[Tags.LevelSave], writer);
			var d = (old ? Run.LastDepth : Run.Depth);
			
			if (d > 0) {
				Run.LastSavedDepth = d;
				Log.Error($"Set run last saved depth to {Run.LastSavedDepth}");
			}
		}

		public override string GetPath(string path, bool old = false) {
			if (path.EndsWith(".lvl")) {
				return path;
			}
			
			return $"{path}level-{(old ? Run.LastDepth : Run.Depth)}-l{(old ? Run.LastLoop : Run.Loop)}.lvl";
		}

		private RegularLevel CreateLevel() {
			if (Run.Depth < -2) {
				return new CutsceneLevel();
			}
			
			if (Run.Depth == -2) {
				return new TutorialLevel();
			}
			
			if (Run.Depth == 0) {
				return new HallLevel();
			}
			
			return new RegularLevel(BiomeRegistry.GenerateForDepth(Run.Depth));
		}

		public static Biome BiomeGenerated;

		private bool GenerationThread(string seed, Area area, int c = 0) {
			var a = new Area();
			Rnd.Seed = $"{seed}{Run.Depth}{c}{Run.Loop}";
			Log.Debug($"Thread seed is {Rnd.Seed} (int {Rnd.IntSeed})");
		
			try {
				Items.GeneratedOnFloor.Clear();
				
				var level = CreateLevel();
				BiomeGenerated = level.Biome;
				WallRegistry.Instance.ResetForBiome(BiomeGenerated);

				a.Add(level);

				if (!level.Generate()) {
					throw new Exception("Failed to paint");
				}

				foreach (var e in a.Entities.ToAdd) {
					area.Add(e);
				}

				area.EventListener.Copy(a.EventListener);
				area.Entities.AddNew();
				I = 0;
			} catch (Exception e) {
				Log.Error(e);
				I++;

				a.Entities.AddNew();
				a.Destroy();
				Run.Level = null;

				if (I > 10) {
					I = 0;
					return GenerationThread(seed, area, c + 1);
				}
				
				return GenerationThread(seed, area);
			}

			BiomeGenerated = null;
			return true;
		}

		private string sd;

		public override void Generate(Area area) {
			if (sd == null) {
				sd = Run.Seed;
			}

			var done = false;
			var finished = false;
			var aborted = false;
			
			var thread = new Thread(() => {
				try {
					done = GenerationThread(sd, area);
					finished = true;
				} catch (ThreadInterruptedException e) {
					Physics.Destroy();
					Physics.Init();
				}
			});
			
			Log.Debug("Level gen thread started");
			thread.Start();
			var i = 0;
			
			while (true) {
				Thread.Sleep(500);

				if (finished) {
					Log.Debug("Thread finished");
					break;
				}

				i++;

				if (i >= 15f) {
					Log.Debug("Thread took too long, aborting :(");
					thread.Interrupt();
					Rnd.Seed = Run.Seed = Rnd.GenerateSeed();
					aborted = true;
					FailedAttempts++;
					
					break;
				}
			}

			if (aborted) {
				Generate(area);
			} else {
				if (Run.Depth > 0) {
					Fails.Add(FailedAttempts);
				}

				Log.Debug($"Generation done, took {i} cycles, {FailedAttempts} failed attempts)");
				FailedAttempts = 0;
				sd = null;
			}
		}

		public override FileHandle GetHandle() {
			return new FileHandle(SaveManager.SlotDir);
		}

		public override void Delete() {
			var handle = Run.Depth > 0 ? GetHandle() : new FileHandle(SaveManager.SaveDir);

			if (!handle.Exists()) {
				return;
			}
			
			foreach (var file in handle.ListFileHandles()) {
				if (file.Extension == ".lvl" || file.Extension == "lvl") {
					file.Delete();
				}
			}
		}

		public LevelSave() : base(SaveType.Level) {
			ResetGen();
		}

		public static void ResetGen() {
			XL = false;
			ChestRewardChance = 5;
			MobDestructionChance = 0;
			MimicChance = 5;
			GenerateMarket = false;
			GenerateTreasure = false;
			GenerateShops = false;
			MeleeOnly = false;
			Item.Attact = false;
		}
	}
}