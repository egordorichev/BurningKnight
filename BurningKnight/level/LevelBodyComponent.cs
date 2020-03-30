using System;
using System.Collections.Generic;
using BurningKnight.entity.component;
using BurningKnight.level.tile;
using BurningKnight.physics;
using BurningKnight.util;
using Lens.util;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Factories;
using VelcroPhysics.Shared;
using VelcroPhysics.Utilities;
using MathUtils = Lens.util.MathUtils;

namespace BurningKnight.level {
	public class LevelBodyComponent : BodyComponent {
		public const byte ChunkSize = 8;

		public Level Level;
		
		private bool dirty;
		protected Body[] chunks;
		protected int cw;
		private int ch;
		private int cs;

		protected List<int> toUpdate = new List<int>();

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
				var level = Level;
				var updated = new List<int>();

				foreach (var u in toUpdate) {
					var cx = (int) Math.Floor(level.FromIndexX(u) / (float) ChunkSize);
					var cy = (int) Math.Floor(level.FromIndexY(u) / (float) ChunkSize);
					var ci = cx + cy * cw;

					if (updated.Contains(ci)) {
						continue;
					}

					updated.Add(ci);
					Physics.RemoveBody(chunks[ci]);
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
					Physics.RemoveBody(c);
				}

				chunks = null;
			}

			dirty = false;
			toUpdate.Clear();
		}

		private void Create() {
			var level = Level;
			
			cw = (int) Math.Floor(level.Width / (float) ChunkSize + 0.5f);
			ch = (int) Math.Floor(level.Height / (float) ChunkSize + 0.5f);
			cs = cw * ch;
			
			if (chunks != null) {
				foreach (var c in chunks) {
					Physics.RemoveBody(c);
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

		protected virtual void RecreateChunk(int cx, int cy) {
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

					var tile = level.Tiles[index];
					var liquid = level.Liquid[index];
					var side = x == 0 || y == 0 || x == level.Width - 1 || y == level.Height - 1;

					if (side || TileFlags.Matches(tile, TileFlags.Solid) || TileFlags.Matches(liquid, TileFlags.Solid)) {
						var sum = 0;

						if (!side) {
							foreach (var dir in PathFinder.Neighbours8) {
								var n = dir + index;

								if (level.IsInside(n) && (TileFlags.Matches(level.Tiles[n], TileFlags.Solid) || TileFlags.Matches(level.Liquid[n], TileFlags.Solid))) {
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
							list.Add(new Vector2(xx + 16, yy + (Check(level, x, y + 1) ? 17 : 24)));
						} else {
							list.Add(new Vector2(xx + 16, yy + 18));
							list.Add(new Vector2(xx + 10, yy + 24));
						}

						if (side || Check(level, x - 1, y) || Check(level, x, y + 1)) {
							list.Add(new Vector2(xx, yy + (Check(level, x, y + 1) ? 17 : 24)));
						} else {
							list.Add(new Vector2(xx, yy + 18));
							list.Add(new Vector2(xx + 6, yy + 24));
						}

						try {
							FixtureFactory.AttachPolygon(new Vertices(list), 1f, body);
						} catch (Exception e) {
							Log.Error(e);
						}
					}
				}
			}
		}

		public void ReCreateBodyChunk(int x, int y) {
			toUpdate.Add(x + y * Level.Width);
		}

		protected virtual bool Check(Level level, int x, int y) {
			return level.IsInside(x, y) && (level.CheckFor(x, y, TileFlags.Solid) || level.CheckFor(x, y, TileFlags.Solid, true));
		}
	}
}