using System;
using System.Collections.Generic;
using BurningKnight.level.tile;
using BurningKnight.physics;
using BurningKnight.util;
using Lens.util;
using Microsoft.Xna.Framework;
using VelcroPhysics.Factories;
using VelcroPhysics.Shared;

namespace BurningKnight.level {
	public class HalfProjectileBodyComponent : LevelBodyComponent {
		protected override void RecreateChunk(int cx, int cy) {
			var level = Level;
			
			var body = BodyFactory.CreateBody(Physics.World, Vector2.Zero);
			body.FixedRotation = true;
			body.UserData = this;

			var i = cx + cy * cw;
			var c = chunks[i];

			if (c != null) {
				Physics.RemoveBody(c);
			}
			
			chunks[i] = body;

			var list = new List<Vector2>();

			for (int y = cy * ChunkSize; y < (cy + 1) * ChunkSize; y++) {
				for (int x = cx * ChunkSize; x < (cx + 1) * ChunkSize; x++) {
					if (!level.IsInside(x, y)) {
						continue;
					}

					var index = level.ToIndex(x, y);

					var tile = level.Liquid[index];
					var side = x == 0 || y == 0 || x == level.Width - 1 || y == level.Height - 1;

					if (side || TileFlags.Matches(tile, TileFlags.Solid)) {
						var sum = 0;

						if (!side) {
							foreach (var dir in PathFinder.Neighbours8) {
								var n = dir + index;

								if (level.IsInside(n) && (TileFlags.Matches(level.Tiles[n], TileFlags.Solid))) {
									sum++;
								}
							}
						}

						if (sum == 8) {
							continue;
						}

						var xx = (x) * 16;
						var yy = (y) * 16 - 8;

						list.Clear();
						
						const int v = 3;

						list.Add(new Vector2(xx, yy + (Check(level, x, y - 1) ? 0 : 7 + v)));
						list.Add(new Vector2(xx + 16, yy + (Check(level, x, y - 1) ? 0 : 7 + v)));
						list.Add(new Vector2(xx + 16, yy + (Check(level, x, y + 1) ? 16 : 8 + v)));
						list.Add(new Vector2(xx, yy + (Check(level, x, y + 1) ? 16 : 8 + v)));

						try {
							FixtureFactory.AttachPolygon(new Vertices(list), 1f, body);
						} catch (Exception e) {
							foreach (var p in list) {
								Log.Info($"{p.X - xx}:{p.Y - yy}");
							}
							
							Log.Error(e);
						}
					}
				}
			}
		}
		
		protected override bool Check(Level level, int x, int y) {
			return level.IsInside(x, y) && (level.CheckFor(x, y, TileFlags.HalfWall, true));
		}
	}
}