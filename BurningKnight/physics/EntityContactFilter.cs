using Box2DX.Collision;
using Box2DX.Dynamics;
using BurningKnight.entity.component;

namespace BurningKnight.physics {
	public class EntityContactFilter : ContactFilter {
		public override bool ShouldCollide(Shape shape1, Shape shape2) {
			var a = shape1.UserData;
			var b = shape2.UserData;

			if (a is BodyComponent ac && b is BodyComponent bc) {
				if (!ac.ShouldCollide(bc.Entity)) {
					return false;
				}
				
				if (!bc.ShouldCollide(ac.Entity)) {
					return false;
				}
			}

			return true;
		}
	}
}