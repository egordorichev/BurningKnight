using Lens.entity;
using Lens.entity.component;
using Lens.util;
using Lens.util.file;

namespace BurningKnight.save {
	public class SaveableEntity : Entity {
		public override void AddComponents() {
			base.AddComponents();
			
			AddTag(Tags.LevelSave);
		}

		public virtual void Save(FileWriter stream) {
			stream.WriteInt32((int) X);
			stream.WriteInt32((int) Y);

			foreach (var component in components.Values) {
				if (component is SaveableComponent saveable) {
					saveable.Save(stream);
				}
			}
		}

		public virtual void Load(FileReader stream) {
			X = stream.ReadInt32();
			Y = stream.ReadInt32();
			
			foreach (var component in components.Values) {
				if (component is SaveableComponent saveable) {
					saveable.Load(stream);
				}
			}
		}
	}
}