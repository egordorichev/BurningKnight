using Lens.entity;
using Lens.util.file;

namespace BurningKnight.entity {
	public class SaveableEntity : Entity {
		protected override void AddComponents() {
			base.AddComponents();
			
			AddTag(Tags.LevelSave);
		}

		public virtual void Save(FileWriter stream) {
			stream.WriteInt32((int) X);
			stream.WriteInt32((int) Y);
		}

		public virtual void Load(FileReader stream) {
			X = stream.ReadInt32();
			Y = stream.ReadInt32();
		}
	}
}