using BurningKnight.entity.component;
using BurningKnight.physics;
using BurningKnight.util;
using Microsoft.Xna.Framework;
using VelcroPhysics.Factories;
using VelcroPhysics.Shared;

namespace BurningKnight.entity.level {
	public class LevelBodyComponent : BodyComponent {
		public void CreateBody() {
			var level = (Level) Entity;
			
			Body = BodyFactory.CreateBody(Physics.World, Vector2.Zero);
			Body.FixedRotation = true;
			Body.UserData = this;

			for (int y = 0; y < level.Height; y++) {
				for (int x = 0; x < level.Width; x++) {
					var index = level.ToIndex(x, y);
					var tile = level.Tiles[index];

					if (TileFlags.Matches(tile, TileFlags.Solid)) {
						var sum = 0;

						foreach (var dir in PathFinder.Neighbours8) {
							var n = dir + index;
							
							if (!level.IsInside(n) || TileFlags.Matches(level.Tiles[n], TileFlags.Solid)) {
								sum++;
							}
						}

						if (sum == 8) {
							continue;
						}
						
						var xx = x * 16;
						var yy = y * 16 - 8;
						
						FixtureFactory.AttachPolygon(new Vertices(4) {
							new Vector2(xx, yy), new Vector2(xx + 16, yy), 
							new Vector2(xx + 16, yy + 16), new Vector2(xx, yy + 16)
						}, 1f, Body);			
					}
				}
			}
		}
	}
}