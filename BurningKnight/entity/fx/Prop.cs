using BurningKnight.save;
using Lens.entity.component.graphics;
using Lens.util.file;

namespace BurningKnight.entity.fx {
	public class Prop : SaveableEntity {
		private string sprite;
		
		public Prop(string slice = null, int depth = 0) {
			sprite = slice;
			Depth = depth;
		}

		// Used for loadingx
		public Prop() {
			
		}
		
		public override void PostInit() {
			base.PostInit();
			SetGraphicsComponent(new SliceComponent("props", sprite));
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteString(sprite);
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			sprite = stream.ReadString();
		}
	}
}