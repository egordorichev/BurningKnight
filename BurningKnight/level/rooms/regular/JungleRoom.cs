using System;
using System.Collections.Generic;
using System.Linq;
using BurningKnight.level.tile;
using BurningKnight.util;
using BurningKnight.util.geometry;
using Lens.util;
using Lens.util.math;

namespace BurningKnight.level.rooms.regular {
	public class JungleRoom : RegularRoom {
		public static int DeathCondition = 4;
		public static int BirthCondition = 4;

		protected Tile Floor = Tile.FloorA;
		protected Tile Floor2 = Tile.FloorB;
		protected Tile Wall = Tile.WallA;

		public override void PaintFloor(Level level) {
			
		}

		protected void Pass(Action<int, int> callback) {
			for (var y = Top; y < Bottom; y++) {
				for (var x = Left; x < Right; x++) {
					callback(x, y);
				}
			}
		}

		public override bool CanConnect(RoomDef R, Dot P) {
			if (P.X == Left || P.X == Right) {
				if (P.Y - Top < 3 || Bottom - P.Y < 3) {
					return false;
				}
			} else {
				if (P.X - Left < 3 || Right - P.X < 3) {
					return false;
				}
			}
			
			return base.CanConnect(R, P);
		}

		public override void Paint(Level level) {
			var w = GetWidth();
			var h = GetHeight();
			
			var old = Painter.Clip;
			Painter.Clip = null;
			Painter.Fill(level, this, Wall);
			Painter.Clip = old;
			
			Painter.FillEllipse(level, this, 3, Tile.FloorA);
			Painter.FillEllipse(level, this, (int) (Math.Min(w, h) / 2f - 2), Tile.FloorD);

			var r = GetCenterRect();
			
			PaintTunnel(level, Tile.FloorA, r, true);
			PaintTunnel(level, Tile.FloorD, r);

			Pass((x, y) => {
				var t = level.Get(x, y);
				
				if (t != Tile.FloorD || Rnd.Chance(30)) {
					if (t == Tile.WallA && Rnd.Chance(10)) {
						Painter.Set(level, x, y, Tile.FloorA);
					}
					
					return;
				}

				var sz = Rnd.Int(2, 4);

				for (var xx = -sz; xx <= sz; xx++) {
					for (var yy = -sz; yy <= sz; yy++) {
						if (level.Get(x + xx, y + yy) != Tile.FloorD) {
							Painter.Set(level, x + xx, y + yy, Rnd.Chance(30) ? Tile.WallA : Tile.FloorA);
						}
					}
				}
			});

			var array = new byte[w, h];
			var oldArray = new byte[w, h];
			
			Pass((x, y) => {
				var t = level.Get(x, y);
				
				if (t == Tile.FloorA && Rnd.Float() > 0.55f - (float) (Math.Cos(x / 16f) * 0.02f + Math.Cos(y / 16f) * 0.1f)) {
					t = Tile.WallA;
					Painter.Set(level, x, y, t);
				}

				var v = t == Tile.FloorA ? 0 : 1;

				if (t == Tile.FloorD) {
					v = 2;
				}

				array[x - Left, y - Top] = (byte) v;
				oldArray[x - Left, y - Top] = (byte) v;
			});

			for (var i = 0; i < 5; i++) {
				for (var x = 0; x < w; x++) {
					for (var y = 0; y < h; y++) {
						var c = oldArray[x, y];
						var n = 0;

						foreach (var d in MathUtils.AllDirections) {
							var xx = x + (int) d.X;
							var yy = y + (int) d.Y;

							if (xx < 0 || xx >= w || yy < 0 || yy >= h) {
								n++;
								continue;
							}

							if (oldArray[xx, yy] == 1) {
								n++;
							}
						}

						if (c != 2) {
							if (n < DeathCondition) {
								c = 0;
							} else if (n > BirthCondition) {
								c = 1;
							}
						}

						array[x, y] = c;
					}
				}

				for (var x = 0; x < w; x++) {
					for (var y = 0; y < h; y++) {
						oldArray[x, y] = array[x, y];
					}
				}
			}

			Action<int, int> fill = null;
			var queue = new Queue<Dot>();
			var m = 0;

			fill = (x, y) => {
				if (x < 0 || y < 0 || x >= w || y >= h) {
					return;
				}
				
				var v = array[x, y];
				
				if (v == 1 || v == 3) {
					return;
				}

				m++;
				array[x, y] = 3;

				queue.Enqueue(new Dot(x - 1, y));
				queue.Enqueue(new Dot(x + 1, y));
				queue.Enqueue(new Dot(x, y - 1));
				queue.Enqueue(new Dot(x, y + 1));
			};

			var painted = false;
			
			foreach (var dr in Connected.Values) {
				var st = new Dot(dr.X, dr.Y);

				if (dr.X == Left) {
					st = new Dot(dr.X + 1, dr.Y);
				} else if (dr.X == Right) {
					st = new Dot(dr.X - 1, dr.Y);
				} else if (dr.Y == Top) {
					st = new Dot(dr.X, dr.Y + 1);
				} else if (dr.Y == Bottom) {
					st = new Dot(dr.X, dr.Y - 1);
				}

				fill(st.X - Left, st.Y - Top);

				if (queue.Count != 0) {
					painted = true;
					break;
				}
			}

			if (!painted) {
				Log.Error("Did not spot a spot in the jungle room!");
				Paint(level);
				return;
			}

			while (queue.Count > 0) {
				var d = queue.Dequeue();
				fill(d.X, d.Y);
			}

			var start = 0;

			var ww = GetWidth();
			var hh = GetHeight();
			
			Func<int, int, int> toIndex = (x, y) => (x - Left) + (y - Top) * ww;

			foreach (var d in Connected.Values) {
				if (d.X == Left) {
					start = toIndex(d.X + 1, d.Y);
				} else if (d.X == Right) {
					start = toIndex(d.X - 1, d.Y);
				} else if (d.Y == Top) {
					start = toIndex(d.X, d.Y + 1);
				} else if (d.Y == Bottom) {
					start = toIndex(d.X, d.Y - 1);
				}
			}
			
			Pass((x, y) => {
				if (x > Left && x < Right && y > Top && y < Bottom) {
					Painter.Set(level, x, y, array[x - Left, y - Top] == 3 ? Floor : Wall);
				}
			});

			var patch = new bool[ww * hh];

			for (var y = 0; y < hh; y++) {
				for (var x = 0; x < ww; x++) {
					patch[y * ww + x] = !level.Get(Left + x, Top + y).IsPassable();
				}
			}

			PathFinder.SetMapSize(ww, hh);
			PathFinder.BuildDistanceMap(start, BArray.Not(patch, null));
			var valid = true;

			for (var i = 0; i < patch.Length; i++) {
				if (!patch[i] && PathFinder.Distance[i] == Int32.MaxValue) {
					valid = false;
					break;
				}
			}
			
			// level.Set(Left + start % ww, Top + start / ww, Tile.Cobweb);
			PathFinder.SetMapSize(level.Width, level.Height);

			if (!valid) {
				Log.Error("Failed to build path");
				Paint(level);
				return;
			}
			 
			patch = Patch.GenerateWithNoise(w, h, Rnd.Float(10000), 0.25f, 0.1f);

			if (Floor2 != Floor) {
				Pass((x, y) => {
					if (level.Get(x, y) == Floor && patch[(x - Left) + (y - Top) * w]) {
						Painter.Set(level, x, y, Floor2);
					}
				});
			}
		}

		public override int GetMinWidth() {
			return (10 + 2) * 2;
		}

		public override int GetMinHeight() {
			return (8 + 2) * 2;
		}

		public override int GetMaxWidth() {
			return (18 + 6) * 2;
		}

		public override int GetMaxHeight() {
			return (12 + 2) * 2;
		}

		public override float GetWeightModifier() {
			return 0.5f;
		}
	}
}