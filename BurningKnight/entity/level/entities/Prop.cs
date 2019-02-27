using BurningKnight.save;
using Lens.entity.component.graphics;

namespace BurningKnight.entity.level.entities {
	public class Prop : SaveableEntity {
		public string Sprite;

		protected override void AddComponents() {
			base.AddComponents();

			if (Sprite != null) {
				AddComponent(new ImageComponent(Sprite));
			}
		}
	}
}