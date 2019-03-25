using BurningKnight.entity.projectile;
using Lens.input;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.item.use {
	public class SimpleShootUse : ShootUse {
		public SimpleShootUse(string slice, float speed, float range = 96) : base((entity, item) => {
			var projectile = Projectile.Make(entity, slice, entity.AngleTo(Input.Mouse.GamePosition), speed);

			projectile.AddLight(32f, Color.Red);
			projectile.Range = (range * 0.5f) / speed;
		}) {
			
		}
	}
}