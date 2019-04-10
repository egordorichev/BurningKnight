using System.Collections.Generic;
using BurningKnight.entity.component;
using BurningKnight.level.tile;
using BurningKnight.physics;
using BurningKnight.util;
using Microsoft.Xna.Framework;
using VelcroPhysics.Factories;
using VelcroPhysics.Shared;

namespace BurningKnight.level {
	public class ChasmBodyComponent : BodyComponent {
		public void CreateBody() {
			if (Body != null) {
				Physics.World.RemoveBody(Body);
			}

			var level = ((Chasm) Entity).Level;
			
			Body = BodyFactory.CreateBody(Physics.World, Vector2.Zero);
			Body.FixedRotation = true;
			Body.UserData = this;

			var list = new List<Vector2>();
			
			for (int y = 0; y < level.Height; y++) {
				for (int x = 0; x < level.Width; x++) {
					var index = level.ToIndex(x, y);
					var tile = level.Tiles[index];

					if (TileFlags.Matches(tile, TileFlags.Hole)) {
						var sum = 0;

						foreach (var dir in PathFinder.Neighbours4) {
							var n = dir + index;
							
							if (!level.IsInside(n) || TileFlags.Matches(level.Tiles[n], TileFlags.Hole) || TileFlags.Matches(level.Tiles[n], TileFlags.Solid)) {
								sum++;
							}
						}

						if (sum == 4) {
							continue;
						}
						
						var xx = x * 16;
						var yy = y * 16 - 8;
						
						list.Clear();

						if (Check(level, x - 1, y) || Check(level, x, y - 1)) {
							list.Add(new Vector2(xx + (Check(level, x - 1, y) ? 0 : 4), Check(level, x, y - 1) ? yy : yy + 8));
						} else {
							list.Add(new Vector2(xx + 4, yy + 16));
							list.Add(new Vector2(xx + 8, yy + 8));
						}

						if (Check(level, x + 1, y) || Check(level, x, y - 1)) {
							list.Add(new Vector2(xx + (Check(level, x + 1, y) ? 16 : 12), Check(level, x, y - 1) ? yy : yy + 8));
						} else {
							list.Add(new Vector2(xx + 12, yy + 16));
							list.Add(new Vector2(xx + 8, yy + 8));
						}
						
						list.Add(new Vector2(xx + (Check(level, x + 1, y) ? 16 : 12), yy + 16));
						list.Add(new Vector2(xx + (Check(level, x - 1, y) ? 0 : 4), yy + 16));
						
						FixtureFactory.AttachPolygon(new Vertices(list), 1f, Body);			
					}
				}
			}
		}

		private bool Check(Level level, int x, int y) {
			return level.IsInside(x, y) && (level.CheckFor(x, y, TileFlags.Solid) || level.CheckFor(x, y, TileFlags.Hole));
		}
	}
}