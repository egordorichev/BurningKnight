using BurningKnight.core.entity.level.save;
using BurningKnight.core.util.file;

namespace BurningKnight.core.entity.level {
	public class SaveableEntity : StatefulEntity {
		public Void Save(FileWriter Writer) {
			Writer.WriteInt32((int) this.X);
			Writer.WriteInt32((int) this.Y);
		}

		public Void Load(FileReader Reader) {
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
