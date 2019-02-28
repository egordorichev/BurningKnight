using BurningKnight.entity.component;

namespace BurningKnight.physics {
	public class EntityContactListener {
		/*public override void Add(ContactPoint point) {
			base.Add(point);
			
			var a = point.Shape1.UserData;
			var b = point.Shape2.UserData;

			if (a is BodyComponent ac && b is BodyComponent bc) {
				ac.OnCollision(bc.Entity);
				bc.OnCollision(ac.Entity);
			}
		}

		public override void Remove(ContactPoint point) {
			base.Remove(point);
			
			var a = point.Shape1.UserData;
			var b = point.Shape2.UserData;

			if (a is BodyComponent ac && b is BodyComponent bc) {
				ac.OnCollisionEnd(bc.Entity);
				bc.OnCollisionEnd(ac.Entity);
			}
		}*/
	}
}