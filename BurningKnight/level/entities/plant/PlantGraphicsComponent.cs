using System;
using BurningKnight.entity.component;
using Lens;
using Lens.graphics;
using Lens.util;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.entities.plant {
	public class PlantGraphicsComponent : SliceComponent {
		public PlantGraphicsComponent(string image, string slice) : base(image, slice) {
			
		}

		public override void Render(bool shadow) {
			var tm = Engine.Instance.State.Time;
			var a = (float) (Math.Cos(tm - Entity.Y / 16 * Math.PI * 0.25f) * Math.Sin(tm * 0.9f + Entity.X / 16 * Math.PI * 0.3f) * (Sprite.Height * 0.05f));
			var origin = new Vector2(Sprite.Width / 2, Sprite.Height);
			
			Graphics.Render(Sprite, Entity.Position + origin, shadow ? -a : a, origin, shadow ? MathUtils.InvertY : Vector2.One);
		}
	}
}