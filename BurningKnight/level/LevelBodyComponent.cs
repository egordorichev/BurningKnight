using System.Collections.Generic;
using BurningKnight.entity.component;
using BurningKnight.level.tile;
using BurningKnight.physics;
using BurningKnight.util;
using Microsoft.Xna.Framework;
using VelcroPhysics.Factories;
using VelcroPhysics.Shared;

namespace BurningKnight.level {
	public class LevelBodyComponent : BodyComponent {
		private bool dirty;

		public override void Init() {
			base.Init();
			Entity.AlwaysActive = true;
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (dirty) {
				dirty = false;
				Create();
			}
		}

		public void CreateBody() {
			if (Body == null) {
				Create();
			} else {
				dirty = true;
			}
		}

		private void Create() {
			if (Body != null) {
				Physics.World.RemoveBody(Body);
			}
			
			var level = (Level) Entity;
			
			Body = BodyFactory.CreateBody(Physics.World, Vector2.Zero);
			Body.FixedRotation = true;
			Body.UserData = this;

			var list = new List<Vector2>();
			
			for (int y = 0; y < level.Height; y++) {
				for (int x = 0; x < level.Width; x++) {
					var index = level.ToIndex(x, y);
					var tile = level.Tiles[index];
					var side = x == 0 || y == 0 || x == level.Width - 1 || y == level.Height - 1;

					if (side || TileFlags.Matches(tile, TileFlags.Solid) && ((Tile) tile) != Tile.Planks) {
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
						
						var xx = x * 16;
						var yy = y * 16 - 8;
						
						list.Clear();

						if (side || Check(level, x - 1, y) || Check(level, x, y - 1)) {
							list.Add(new Vector2(xx, yy + (Check(level, x, y - 1) ? 0 : 8)));
						} else {
							list.Add(new Vector2(xx, yy + 12));
							list.Add(new Vector2(xx + 6, yy + (Check(level, x, y - 1) ? 0 : 8)));
						}

						if (side || Check(level, x + 1, y) || Check(level, x, y - 1)) {
							list.Add(new Vector2(xx + 16, yy + (Check(level, x, y - 1) ? 0 : 8)));
						} else {
							list.Add(new Vector2(xx + 16, yy + 12));
							list.Add(new Vector2(xx + 10, yy + (Check(level, x, y - 1) ? 0 : 8)));
						}
						
						if (side || Check(level, x + 1, y) || Check(level, x, y + 1)) {
							list.Add(new Vector2(xx + 16, yy + 16));
						} else {
							list.Add(new Vector2(xx + 16, yy + 10));
							list.Add(new Vector2(xx + 10, yy + 16));
						}
						
						if (side || Check(level, x - 1, y) || Check(level, x, y + 1)) {
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

		private bool Check(Level level, int x, int y) {
			return level.IsInside(x, y) && (level.CheckFor(x, y, TileFlags.Solid)) && level.Get(x, y) != Tile.Planks;
		}
	}
}