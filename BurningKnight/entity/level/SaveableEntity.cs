using BurningKnight.entity.level.save;
using Lens.util.file;

namespace BurningKnight.entity.level {
	public class SaveableEntity : StatefulEntity {
		public void Save(FileWriter Writer) {
			Writer.WriteInt32((int) this.X);
			Writer.WriteInt32((int) this.Y);
		}

		public void Load(FileReader Reader) {
			this.X = Reader.ReadInt32();
			this.Y = Reader.ReadInt32();
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