using BurningKnight.entity.projectile;
using Lens.input;

namespace BurningKnight.entity.item.use {
	public class SimpleShootUse : ShootUse {
		public SimpleShootUse(string slice, float speed) : base((entity, item) => {
			Projectile.Make(entity, slice, entity.AngleTo(Input.Mouse.GamePosition), speed);
		}) {
			
		}
	}
}