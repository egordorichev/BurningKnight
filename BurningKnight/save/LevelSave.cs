using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BurningKnight.assets.items;
using BurningKnight.level;
using BurningKnight.level.basement;
using BurningKnight.level.biome;
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
			
			return $"{path}level-{(old ? Run.LastDepth : Run.Depth)}.lvl";
		}

		private RegularLevel CreateLevel() {
			if (Run.Depth == -2) {
				return new TutorialLevel();
			}
			
			if (Run.Depth == -1) {
				return new BasementLevel();
			}
			
			if (Run.Depth == 0) {
				return new HallLevel();
			}
			
			return new RegularLevel(BiomeRegistry.GenerateForDepth(Run.Depth));
		}

		public static Biome BiomeGenerated;

		private void SetupDummy(Area area) {
			Log.Error("Can't generate a level");
			var a = new Area();

			var level = CreateLevel();
			BiomeGenerated = level.Biome;
			WallRegistry.Instance.ResetForBiome(BiomeGenerated);

			level.Width = 32;
			level.Height = 32;
			level.NoLightNoRender = false;
			level.DrawLight = false;
					
			a.Add(level);

			level.Setup();
			level.Fill(Tile.FloorA);
			level.TileUp();
			level.CreateBody();

			foreach (var e in a.Entities.ToAdd) {
				area.Add(e);
			}

			area.EventListener.Copy(a.EventListener);
			area.Entities.AddNew();
		}
		
		private bool GenerationThread(Area area, int c = 0) {
			var a = new Area();
			Rnd.Seed = $"{Run.Seed}{Run.Depth}{c}"; 
		
			try {
				Items.GeneratedOnFloor.Clear();
				
				var level = CreateLevel();
				BiomeGenerated = level.Biome;
				WallRegistry.Instance.ResetForBiome(BiomeGenerated);

				a.Add(level);
				level.Generate();

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
					return GenerationThread(area, c + 1);

					/*SetupDummy(area);
					
					return false;*/
				}
				
				return GenerationThread(area);
			}

			BiomeGenerated = null;
			return true;
		}

		public override void Generate(Area area) {
			var done = false;
			var finished = false;
			var aborted = false;
			
			var thread = new Thread(() => {
				try {
					done = GenerationThread(area);
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

				Log.Debug($"Generation done, took {i} cycles, {FailedAttempts} failed attempts (avrg {Fails.Average()})");
				FailedAttempts = 0;
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
			
		}
	}
}