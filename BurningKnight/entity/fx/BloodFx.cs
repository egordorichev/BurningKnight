using System;
using System.Collections.Generic;
using Lens.entity;
using Lens.graphics;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.fx {
	public class BloodFx : Entity {
		private Polygon poly;

		public override void PostInit() {
			base.PostInit();

			Depth = Layers.Blood;

			var points = new List<Vector2>();
			var count = (float) Random.Int(10, 20);
			var d = 8;
			
			for (var i = 0; i < count; i++) {
				var a = Math.PI * 2 * (i / count);
				points.Add(new Vector2((float) Math.Cos(a) * d, (float) Math.Sin(a) * d));
			}
			
			poly = new Polygon(points.ToArray());
		}

		public override void Render() {
			// todo: fill and not draw?
			Graphics.Batch.DrawPolygon(Position, poly, Color.Red, 1f);
		}
	}
}