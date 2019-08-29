using System;
using System.Collections.Generic;
using BurningKnight.entity.component;
using BurningKnight.level.tile;
using BurningKnight.physics;
using BurningKnight.util;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Factories;
using VelcroPhysics.Shared;

namespace BurningKnight.level {
	public class LevelBodyComponent : BodyComponent {
		public const byte ChunkSize = 8;
		
		private bool dirty;
		private Body[] chunks;
		private int cw;
		private int ch;
		private int cs;

		private List<int> toUpdate = new List<int>();

		public override void Init() {
			base.Init();
			Entity.AlwaysActive = true;
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (dirty) {
				dirty = false;
				Create();
				toUpdate.Clear();
				return;
			}

			if (toUpdate.Count > 0) {
				var updated = new List<int>();
				var level = (Level) Entity;

				foreach (var u in toUpdate) {
					var cx = (int) Math.Floor(level.FromIndexX(u) / (float) ChunkSize);
					var cy = (int) Math.Floor(level.FromIndexY(u) / (float) ChunkSize);
					var ci = cx + cy * cw;

					if (updated.Contains(ci)) {
						continue;
					}

					updated.Add(ci);
					Physics.World.RemoveBody(chunks[ci]);
					RecreateChunk(cx, cy);
				}

				toUpdate.Clear();
			}
		}

		public void CreateBody() {
			if (chunks == null) {
				Create();
			} else {
				dirty = true;
			}
		}

		public override void Destroy() {
			base.Destroy();
			
			if (chunks != null) {
				foreach (var c in chunks) {
					Physics.World.RemoveBody(c);
				}

				chunks = null;
			}

			dirty = false;
			toUpdate.Clear();
		}

		private void Create() {
			var level = (Level) Entity;
			
			cw = (int) Math.Floor(level.Width / (float) ChunkSize + 0.5f);
			ch = (int) Math.Floor(level.Height / (float) ChunkSize + 0.5f);
			cs = cw * ch;
			
			if (chunks != null) {
				foreach (var c in chunks) {
					Physics.World.RemoveBody(c);
				}
			} else {
				chunks = new Body[cs];
			}

			for (int cy = 0; cy < ch; cy++) {
				for (int cx = 0; cx < cw; cx++) {
					RecreateChunk(cx, cy);
				}
			}
		}

		private void RecreateChunk(int cx, int cy) {
			var level = (Level) Entity;
			
			var body = BodyFactory.CreateBody(Physics.World, Vector2.Zero);
			body.FixedRotation = true;
			body.UserData = this;

			var i = cx + cy * cw;
			var c = chunks[i];

			if (c != null) {
				Physics.World.RemoveBody(c);
			}
			
			chunks[i] = body;

			var list = new List<Vector2>();

			for (int y = cy * ChunkSize; y < (cy + 1) * ChunkSize; y++) {
				for (int x = cx * ChunkSize; x < (cx + 1) * ChunkSize; x++) {
					if (!level.IsInside(x, y)) {
						continue;
					}

					var index = level.ToIndex(x, y);

					var tile = level.Tiles[index];
					var side = x == 0 || y == 0 || x == level.Width - 1 || y == level.Height - 1;

					if (side || TileFlags.Matches(tile, TileFlags.Solid) && ((Tile) tile) != Tile.Planks) {
						var sum = 0;

						if (!side) {
							foreach (var dir in PathFinder.Neighbours8) {
								var n = dir + index;

								if (level.IsInside(n) && (TileFlags.Matches(level.Tiles[n], TileFlags.Solid)) &&
								    ((Tile) level.Tiles[n]) != Tile.Planks) {
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

						FixtureFactory.AttachPolygon(new Vertices(list), 1f, body);
					}
				}
			}
		}

		public void ReCreateBodyChunk(int x, int y) {
			toUpdate.Add(x + y * ((Level) Entity).Width);
		}

		private bool Check(Level level, int x, int y) {
			return level.IsInside(x, y) && (level.CheckFor(x, y, TileFlags.Solid)) && level.Get(x, y) != Tile.Planks;
		}
	}
}