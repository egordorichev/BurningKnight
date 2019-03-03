using System.Collections.Generic;
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

			var list = new List<Vector2>();
			
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
						
						list.Clear();

						if (Check(level, x - 1, y, TileFlags.Solid) || Check(level, x, y - 1, TileFlags.Solid)) {
							list.Add(new Vector2(xx, yy));
						} else {
							list.Add(new Vector2(xx, yy + 6));
							list.Add(new Vector2(xx + 6, yy));
						}

						if (Check(level, x + 1, y, TileFlags.Solid) || Check(level, x, y - 1, TileFlags.Solid)) {
							list.Add(new Vector2(xx + 16, yy));
						} else {
							list.Add(new Vector2(xx + 16, yy + 6));
							list.Add(new Vector2(xx + 10, yy));
						}
						
						if (Check(level, x + 1, y, TileFlags.Solid) || Check(level, x, y + 1, TileFlags.Solid)) {
							list.Add(new Vector2(xx + 16, yy + 16));
						} else {
							list.Add(new Vector2(xx + 16, yy + 10));
							list.Add(new Vector2(xx + 10, yy + 16));
						}
						
						if (Check(level, x - 1, y, TileFlags.Solid) || Check(level, x, y + 1, TileFlags.Solid)) {
							list.Add(new Vector2(xx, yy + 16));
						} else {
							list.Add(new Vector2(xx, yy + 10));
							list.Add(new Vector2(xx + 6, yy + 16));
						}
						
						FixtureFactory.AttachPolygon(new Vertices(list), 1f, Body);			
					}
				}
			}
		}

		private bool Check(Level level, int x, int y, int flag) {
			return level.IsInside(x, y) && level.CheckFor(x, y, flag);
		}
	}
}