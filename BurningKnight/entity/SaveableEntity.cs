using BurningKnight.entity.level.save;
using Lens.entity;
using Lens.util.file;

namespace BurningKnight.entity {
	public class SaveableEntity : Entity {
		public virtual void Save(FileWriter stream) {
			stream.WriteInt32((int) X);
			stream.WriteInt32((int) Y);
		}

		public virtual void Load(FileReader stream) {
			X = stream.ReadInt32();
			Y = stream.ReadInt32();
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