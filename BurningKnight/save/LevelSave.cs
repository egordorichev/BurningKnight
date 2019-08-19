using System;
using System.Threading;
using BurningKnight.entity.creature.mob.castle;
using BurningKnight.level;
using BurningKnight.level.biome;
using BurningKnight.level.hall;
using BurningKnight.level.hub;
using BurningKnight.level.tile;
using BurningKnight.state;
using Lens.entity;
using Lens.util;
using Lens.util.file;

namespace BurningKnight.save {
	public class LevelSave : EntitySaver {
		private static int I;

		public override void Save(Area area, FileWriter writer) {
			SmartSave(area.Tags[Tags.LevelSave], writer);
		}

		public override string GetPath(string path, bool old = false) {
			if (path.EndsWith(".lvl")) {
				return path;
			}
			
			return $"{path}level-{(old ? Run.LastDepth : Run.Depth)}.lvl";
		}

		private RegularLevel CreateLevel() {
			if (Run.Depth == -1) {
				return new HubLevel();
			}
			
			if (Run.Depth == 0) {
				return new HallLevel();
			}
			
			return new RegularLevel(BiomeRegistry.ForDepth(Run.Depth));
		}

		public static Biome BiomeGenerated;
		
		private bool GenerationThread(Area area) {
			var a = new Area();
		
			try {
				var level = CreateLevel();
				BiomeGenerated = level.Biome;
				a.Add(level);
				level.Generate(a, I);

				foreach (var e in a.Entities.ToAdd) {
					area.Add(e);
				}

				area.Entities.AddNew();
				I = 0;
			} catch (Exception e) {
				Log.Error(e);
				I++;

				a.Entities.AddNew();
				a.Destroy();
				a = new Area();
				Run.Level = null;

				if (I > 100) {
					Log.Error("Can't generate a level");

					var level = CreateLevel();
					BiomeGenerated = level.Biome;

					level.Width = 32;
					level.Height = 32;
					level.NoLightNoRender = false;
					level.DrawLight = false;
					
					a.Add(level);

					level.Setup();
					level.Fill(Tile.FloorA);
					level.TileUp();
					
					return false;
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
				done = GenerationThread(area);
				finished = true;
			});
			
			Log.Debug("Thread started");
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
					thread.Abort();
					aborted = true;

					break;
				}
			}

			if (aborted) {
				Generate(area);
			} else {
				Log.Debug($"Generation done, took {i} cycles");
			}
		}

		public override FileHandle GetHandle() {
			return new FileHandle(SaveManager.SlotDir);
		}

		public override void Delete() {
			var handle = GetHandle();

			if (!handle.Exists()) {
				return;
			}
			
			foreach (var file in handle.ListFileHandles()) {
				file.Delete();
			}
		}

		public LevelSave() : base(SaveType.Level) {
			
		}
	}
}