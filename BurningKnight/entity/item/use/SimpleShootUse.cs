using BurningKnight.entity.projectile;
using Lens.input;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.item.use {
	public class SimpleShootUse : ShootUse {
		public SimpleShootUse(string slice, float speed) : base((entity, item) => {
			Projectile.Make(entity, slice, entity.AngleTo(Input.Mouse.GamePosition), speed).AddLight(32f, Color.Red);
		}) {
			
		}
	}
}