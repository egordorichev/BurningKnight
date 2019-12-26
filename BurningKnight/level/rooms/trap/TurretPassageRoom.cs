using System;
using System.Collections.Generic;
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
	public class TurretPassageRoom : TrapRoom {
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

		public override void Paint(Level level) {
			Painter.Fill(level, this, 1, Tile.Chasm);
			Painter.DrawLine(level, new Dot(Left + 1, Top + 1), new Dot(Left + 1, Bottom - 1), Tiles.RandomFloor(), true);
			Painter.DrawLine(level, new Dot(Right - 1, Top + 1), new Dot(Right - 1, Bottom - 1), Tiles.RandomFloor(), true);

			var ty = Rnd.Int(Top + 2, Bottom - 2);

			if (Rnd.Chance()) {
				ty = Top + GetHeight() / 2;
			}

			for (var i = 0; i < 2; i++) {
				if (Rnd.Chance()) {
					var tty = Rnd.Int(Top + 2, Bottom - 2);
					Painter.DrawLine(level, new Dot(Left + 1, tty), new Dot(Right - 1, tty), Tile.FloorD);
				}
			}

			Painter.DrawLine(level, new Dot(Left + 1, ty), new Dot(Right - 1, ty), Tiles.RandomFloor(), true);

			if (Connected.Count == 1 || true) {
				ty = Rnd.Int(Top + 2, Bottom - 2);

				if (Connected.Values.First().X == Left) {
					PlaceButton(level, new Dot(Right - 1, ty));
				} else {
					PlaceButton(level, new Dot(Left + 1, ty));
				}
			}
			
			var s = Rnd.Int(2, 4);
			var xm = Rnd.Int(0, s);

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
			
			for (var x = Left + 2 + xm; x < Right - 2; x += s) {
				Place(level, x, Top + 1, 2, fn(x, Top + 1));
			}
			
			for (var x = Left + 2 + xm; x < Right - 2; x += s) {
				Place(level, x, Bottom - 1, 6, fn(x, Bottom - 1));
			}
		}
		
		public override bool CanConnect(RoomDef R, Dot P) {
			if (P.X == Left || P.X == Right) {
				if (P.Y == Top + 1 || P.Y == Bottom - 1) {
					return false;
				}
			} else 	if (P.Y == Top || P.Y == Bottom) {
				return false;
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
				TimingOffset = offset
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