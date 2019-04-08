using System;
using BurningKnight.entity.creature.mob.castle;
using BurningKnight.entity.level;
using BurningKnight.entity.level.biome;
using BurningKnight.entity.level.hub;
using BurningKnight.entity.level.tile;
using BurningKnight.state;
using Lens.entity;
using Lens.util;
using Lens.util.file;

namespace BurningKnight.save {
	public class LevelSave : Saver {
		private static int I;

		public override void Save(Area area, FileWriter writer) {
			var all = area.Tags[Tags.LevelSave];
			writer.WriteInt32(all.Count);

			foreach (SaveableEntity entity in all) {
				writer.WriteString(entity.GetType().FullName.Replace("BurningKnight.", ""));
				entity.Save(writer);
			}
		}

		public override string GetPath(string path, bool old = false) {
			if (path.EndsWith(".lvl")) {
				return path;
			}
			
			return $"{path}level:{(old ? Run.LastDepth : Run.Depth)}.lvl";
		}

		public override void Load(Area area, FileReader reader) {
			var count = reader.ReadInt32();

			for (var I = 0; I < count; I++) {
				var entity = (SaveableEntity) Activator.CreateInstance(Type.GetType($"BurningKnight.{reader.ReadString()}", true, false));

				area.Add(entity, false);
				entity.Load(reader);
				entity.PostInit();
			}
		}

		private RegularLevel CreateLevel() {
			if (Run.Depth == -1) {
				return new HubLevel();
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

				foreach (var en in area.Tags[Tags.LevelSave]) {
					en.Done = true;
				}

				if (I > 100) {
					Log.Error("Can't generate a level");

					var level = CreateLevel();

					level.Width = 32;
					level.Height = 32;
					
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