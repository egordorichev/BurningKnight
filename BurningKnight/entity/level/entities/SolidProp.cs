using BurningKnight.entity.component;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.entities {
	public class SolidProp : Prop {
		public Rect collider;
		
		protected override void AddComponents() {
			base.AddComponents();
			AddComponent(new RectBodyComponent(collider.Left, collider.Right, collider.Width, collider.Height));
		}
	}
}