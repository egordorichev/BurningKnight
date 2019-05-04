using System;
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
			
			return $"{path}level:{(old ? Run.LastDepth : Run.Depth)}.lvl";
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

		public override void Generate(Area area) {
			try {
				var level = CreateLevel();
				area.Add(level);
				level.Generate(area, I);

				I = 0;
			} catch (Exception e) {
				Log.Error(e);
				I++;

				// FIXME: this part removes player, and thats ayayayay
				
				// Make sure the area.Add() entities received their tags in order to be removed
				area.Entities.AddNew();
				
				foreach (var en in area.Tags[Tags.LevelSave]) {
					Log.Error(en.GetType().FullName);
					en.Done = true;
				}
				
				area.AutoRemove();

				if (I > 100) {
					Log.Error("Can't generate a level");

					var level = CreateLevel();

					level.Width = 32;
					level.Height = 32;
					level.NoLightNoRender = false;
					level.DrawLight = false;
					
					area.Add(level);

					level.Setup();
					level.Fill(Tile.FloorA);
					level.TileUp();
					
					return;
				}
				
				Generate(area);
			}
		}

		public override FileHandle GetHandle() {
			return new FileHandle(SaveManager.SlotDir);
		}

		public override void Delete() {
			var handle = GetHandle();

			foreach (var file in handle.ListFileHandles()) {
				file.Delete();
			}
		}
	}
}