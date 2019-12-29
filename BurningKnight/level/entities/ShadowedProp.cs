using BurningKnight.entity.component;

namespace BurningKnight.level.entities {
	public class ShadowedProp : SlicedProp {
		public ShadowedProp(string slice = null, int depth = 0) : base(slice, depth) {
			
		}
		
		public override void AddComponents() {
			base.AddComponents();
			AddComponent(new ShadowComponent());
		}
	}
}