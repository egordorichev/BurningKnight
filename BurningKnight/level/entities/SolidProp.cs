using BurningKnight.entity.component;
using BurningKnight.util.geometry;

namespace BurningKnight.level.entities {
	public class SolidProp : Prop {
		public Rect collider;
		
		public override void AddComponents() {
			base.AddComponents();
			AddComponent(new RectBodyComponent(collider.Left, collider.Right, collider.GetWidth(), collider.GetHeight()));
		}
	}
}