using System;
using System.Linq;
using BurningKnight.entity.room;
using BurningKnight.entity.room.controllable.turret;
using BurningKnight.entity.room.input;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.trap {
	public class CrossTurretPassageRoom : TrapRoom {
		public override void PaintFloor(Level level) {
			
		}

		public override void ModifyRoom(Room room) {
			base.ModifyRoom(room);
			room.AddController("bk:trap");
		}

		public override int GetMinWidth() {
			return 14;
		}

		public override int GetMaxWidth() {
			return 22;
		}
		public override int GetMinHeight() {
			return 14;
		}

		public override int GetMaxHeight() {
			return 22;
		}

		protected override bool Quad() {
			return true;
		}

		private void Line(Level level, Dot a, Dot b) {
			Painter.DrawLine(level, a, b, Tiles.RandomFloor(), true);

			if (Rnd.Chance()) {
				Painter.DrawLine(level, a, b, Tiles.RandomFloor());
			}
		}

		public override void Paint(Level level) {
			base.Paint(level);
			Painter.Fill(level, this, 1, Tile.Chasm);

			var tx = Left + GetWidth() / 2;
			var ty = Top + GetHeight() / 2;

			Line(level, new Dot(tx, Top + 2), new Dot(tx, Bottom - 2));
			Line(level, new Dot(Left + 2, ty), new Dot(Right - 2, ty));

			var a = Rnd.Chance();

			if (a) {
				PlaceButton(level, new Dot(Left + 2, ty));
				PlaceButton(level, new Dot(Right - 2, ty));
			}

			if (!a || Rnd.Chance()) {
				PlaceButton(level, new Dot(tx, Top + 2));
				PlaceButton(level, new Dot(tx, Bottom - 2));
			}

			var s = 2; // Rnd.Int(2, 4);
			var cx = Left + GetWidth() / 2;
			var cy = Top + GetHeight() / 2;

			#region Wave Generator
			var xsmooth = Rnd.Chance(30);
			var xmod = Rnd.Float(s * 8, s * 16);
			var ysmooth = Rnd.Chance(30);
			var ymod = Rnd.Chance(30) ? xmod : Rnd.Float(s * 32, s * 64);

			var fn = new Func<int, int, float>((x, y) => {
				var t = 0f;

				if (!xsmooth) {
					t += (float) Math.Cos(x / xmod);
				}

				if (!ysmooth) {
					t += (float) Math.Sin(y / ymod);
				}
				
				return MathUtils.Mod(t * 3, 3);
			});
			#endregion

			var p = 1;
			
			for (var x = Left + 3; x < Right - 2; x += s) {
				if (x >= cx - p && x <= cx + p) {
					continue;
				}
				
				Place(level, x, Top + 1, 2, fn(x, Top + 1));
			}
			
			for (var x = Left + 3; x < Right - 2; x += s) {
				if (x >= cx - p && x <= cx + p) {
					continue;
				}
				
				Place(level, x, Bottom - 1, 6, fn(x, Bottom - 1));
			}
			
			for (var x = Top + 3; x < Bottom - 2; x += s) {
				if (x >= cy - p && x <= cy + p) {
					continue;
				}
				
				Place(level, Left + 1, x, 0, fn(x, Left + 1));
			}
			
			for (var x = Top + 3; x < Bottom - 2; x += s) {
				if (x >= cy - p && x <= cy + p) {
					continue;
				}
				
				Place(level, Right - 1, x, 4, fn(x, Right - 1));
			}
		}
		
		public override bool CanConnect(RoomDef R, Dot P) {
			if (P.X == Left || P.X == Right) {
				if (Math.Abs(P.Y - (Top + GetHeight() / 2)) > 0) {
					return false;
				}
			} else {
				if (Math.Abs(P.X - (Left + GetWidth() / 2)) > 0) {
					return false;
				}
			}

			return base.CanConnect(R, P);
		}

		private bool Place(Level level, int x, int y, uint a, float offset) {
			foreach (var d in Connected.Values) {
				if ((d.X == x && (d.Y == y + 1 || d.Y == y - 1)) || (d.Y == y && (d.X == x + 1 || d.X == x - 1))) {
					return false;
				}
			}
			
			Painter.Set(level, x, y, Tile.FloorA);

			level.Area.Add(new Turret {
				Position = new Vector2(x, y) * 16,
				StartingAngle = a,
				TimingOffset = offset,
				Speed = 1.5f
			});

			return true;
		}

		private void PlaceButton(Level level, Dot where) {
			Painter.Set(level, where, Tiles.RandomFloor());

			var input = new Button();
			input.Position = where * 16;
			level.Area.Add(input);
		}
	}
}