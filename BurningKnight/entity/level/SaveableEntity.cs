using BurningKnight.entity.level.save;
using Lens.entity;
using Lens.util.file;

namespace BurningKnight.entity.level {
	public class SaveableEntity : Entity {
		public void Save(FileWriter Writer) {
			Writer.WriteInt32((int) X);
			Writer.WriteInt32((int) Y);
		}

		public void Load(FileReader Reader) {
			X = Reader.ReadInt32();
			Y = Reader.ReadInt32();
		}

		public SaveableEntity Add() {
			LevelSave.All.Add(this);

			return this;
		}

		public SaveableEntity Remove() {
			LevelSave.All.Remove(this);

			return this;
		}
	}
}