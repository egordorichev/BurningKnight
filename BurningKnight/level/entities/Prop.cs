using BurningKnight.entity.component;
using BurningKnight.entity.editor;
using BurningKnight.save;
using Lens.entity.component.graphics;
using Lens.util.file;

namespace BurningKnight.level.entities {
	public class Prop : SaveableEntity, PlaceableEntity {
		public string Sprite;
		
		public Prop(string slice = null, int depth = 0) {
			Sprite = slice;
			Depth = depth;
		}

		// Used for loading
		public Prop() {
			
		}
		
		public override void PostInit() {
			base.PostInit();
			AddComponent(new SliceComponent("props", Sprite));
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteString(Sprite);
			stream.WriteSbyte((sbyte) Depth);
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			Sprite = stream.ReadString();
			Depth = stream.ReadSbyte();
		}
	}
}