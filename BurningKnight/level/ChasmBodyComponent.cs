using System;
using System.Collections.Generic;
using BurningKnight.entity.component;
using BurningKnight.level.tile;
using BurningKnight.physics;
using BurningKnight.state;
using BurningKnight.util;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Factories;
using VelcroPhysics.Shared;

namespace BurningKnight.level {
	public class ChasmBodyComponent : BodyComponent {
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
				var level = Run.Level;

				foreach (var u in toUpdate) {
					var cx = (int) Math.Floor(level.FromIndexX(u) / (float) LevelBodyComponent.ChunkSize);
					var cy = (int) Math.Floor(level.FromIndexY(u) / (float) LevelBodyComponent.ChunkSize);
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

		public void CreateBody() {
			if (chunks == null) {
				Create();
			} else {
				dirty = true;
			}
		}

		private void Create() {
			var level = Run.Level;
			
			cw = (int) Math.Floor(level.Width / (float) LevelBodyComponent.ChunkSize + 0.5f);
			ch = (int) Math.Floor(level.Height / (float) LevelBodyComponent.ChunkSize + 0.5f);
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

		public void ReCreateBodyChunk(int x, int y) {
			toUpdate.Add(x + y * Run.Level.Width);
		}
		
		private void RecreateChunk(int cx, int cy) {
			var level = Run.Level;
			
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

			for (int y = cy * LevelBodyComponent.ChunkSize; y < (cy + 1) * LevelBodyComponent.ChunkSize; y++) {
				for (int x = cx * LevelBodyComponent.ChunkSize; x < (cx + 1) * LevelBodyComponent.ChunkSize; x++) {
					if (!level.IsInside(x, y)) {
						continue;
					}
					
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

						if (sum >= 4) {
							continue;
						}
						
						var xx = x * 16;
						var yy = y * 16;
						
						list.Clear();

						if (Check(level, x - 1, y) || Check(level, x, y - 1)) {
							list.Add(new Vector2(xx + (Check(level, x - 1, y) ? 0 : 4), yy));
						} else {
							list.Add(new Vector2(xx + 4, yy + 4));
							list.Add(new Vector2(xx + 6 + 4, yy));
						}

						if (Check(level, x + 1, y) || Check(level, x, y - 1)) {
							list.Add(new Vector2(xx + 16 - (Check(level, x + 1, y) ? 0 : 4), yy));
						} else {
							list.Add(new Vector2(xx + 16 - 4, yy));
							list.Add(new Vector2(xx + 10 - 4, yy + 4));
						}

						if (Check(level, x + 1, y) || Check(level, x, y + 1)) {
							list.Add(new Vector2(xx + 16 - (Check(level, x + 1, y) ? 0 : 4), yy + 16));
						} else {
							list.Add(new Vector2(xx + 16 - 4, yy + 12));
							list.Add(new Vector2(xx + 10 - 4, yy + 16));
						}

						if (Check(level, x - 1, y) || Check(level, x, y + 1)) {
							list.Add(new Vector2(xx + (Check(level, x - 1, y) ? 0 : 4), yy + 16));
						} else {
							list.Add(new Vector2(xx + 4, yy + 12));
							list.Add(new Vector2(xx + 6 + 4, yy + 16));
						}
						
						FixtureFactory.AttachPolygon(new Vertices(list), 1f, body);			
					}
				}
			}
		}

		private bool Check(Level level, int x, int y) {
			return level.IsInside(x, y) && (level.CheckFor(x, y, TileFlags.Solid) || level.CheckFor(x, y, TileFlags.Hole));
		}
	}
}