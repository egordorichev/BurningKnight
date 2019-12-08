using System;
using System.Collections.Generic;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util;
using Lens.util.math;

namespace BurningKnight.level.rooms.regular {
	public class JungleRoom : RegularRoom {
		public static int DeathCondition = 4;
		public static int BirthCondition = 4;
		
		public override void PaintFloor(Level level) {
			
		}

		private void Pass(Action<int, int> callback) {
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
			
			Painter.Fill(level, this, Tile.WallA);
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
							Painter.Set(level, x + xx, y + yy, Rnd.Chance() ? Tile.WallA : Tile.FloorA);
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

			fill(w / 2, h / 2);

			if (queue.Count == 0) {
				Log.Error("Filled center in the jungle room!");
				Paint(level);
				return;
			}

			while (queue.Count > 0) {
				var d = queue.Dequeue();
				fill(d.X, d.Y);
			}

			if (m < w * h / 4) {
				Log.Error($"Too lil floor space, regenerating the jungle room ({m / (w * h)}%)");
				Paint(level);
				return;
			}

			Pass((x, y) => {
				if (x > Left && x < Right && y > Top && y < Bottom) {
					Painter.Set(level, x, y, array[x - Left, y - Top] == 3 ? Tile.FloorA : Tile.WallA);
				}
			});
		}

		public override int GetMinWidth() {
			return (10 + 2) * 3;
		}

		public override int GetMinHeight() {
			return (8 + 2) * 3;
		}

		public override int GetMaxWidth() {
			return (18 + 6) * 3;
		}

		public override int GetMaxHeight() {
			return (12 + 2) * 3;
		}
	}
}