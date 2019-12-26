using System;
using System.Collections.Generic;
using BurningKnight.entity.room;
using BurningKnight.entity.room.controllable.turret;
using BurningKnight.entity.room.input;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.trap {
	public class TurretTrapRoom : TrapRoom {
		public override void PaintFloor(Level level) {
			
		}

		public override void ModifyRoom(Room room) {
			base.ModifyRoom(room);
			room.AddController("bk:trap");
		}

		public override void Paint(Level level) {
			Painter.Fill(level, this, 1, Tile.Chasm);
			Painter.Fill(level, this, 2, Tiles.RandomFloor());

			// todo: the shift in timing
			
			var s = Rnd.Int(2, 4);
			var f = Tiles.RandomNewFloor();
			var ft = Rnd.Chance() ? f : Tiles.RandomNewFloor();
			var spots = new List<Dot>();
			var p = Rnd.Int(0, 2);
			var xm = Rnd.Int(0, s);
			var ym = Rnd.Int(0, s);
			
			for (var x = Left + 2 + xm; x < Right - 2; x += s) {
				Painter.DrawLine(level, new Dot(x, Top + 1), new Dot(x, Bottom - 1), f);

				if (Place(level, x, Top + 1, 2)) {
					spots.Add(new Dot(x, Top + 2 + p));
				}	
			}
			
			for (var x = Left + 2 + xm; x < Right - 2; x += s) {
				if (Place(level, x, Bottom - 1, 6)) {
					spots.Add(new Dot(x, Bottom - 2 - p));
				}		
			}
			
			for (var y = Top + 2 + ym; y < Bottom - 2; y += s) {
				Painter.DrawLine(level, new Dot(Left + 1, y), new Dot(Right - 1, y), ft);
				
				if (Place(level, Left + 1, y, 0)) {
					spots.Add(new Dot(Left + 2 + p, y));
				}	
			}
			
			for (var y = Top + 2 + ym; y < Bottom - 2; y += s) {
				if (Place(level, Right - 1, y, 4)) {
					spots.Add(new Dot(Right - 2 - p, y));
				}	
			}

			if (Connected.Count > 1 && Rnd.Chance(20)) {
				return;
			}

			if (Rnd.Chance()) {
				PlaceButton(level, GetTileCenter());
			} else {
				for (var i = 0; i < Rnd.Int(1, Math.Min(spots.Count, 4)); i++) {
					var j = Rnd.Int(spots.Count);

					PlaceButton(level, spots[j]);
					spots.RemoveAt(j);
				}
			}
		}

		private void PlaceButton(Level level, Dot where) {
			Painter.Set(level, where, Tiles.RandomFloor());

			var input = new Button();
			input.Position = where * 16;
			level.Area.Add(input);
		}
		
		public override bool CanConnect(RoomDef R, Dot P) {
			if (P.X == Left || P.X == Right) {
				if (P.Y == Top + 1 || P.Y == Bottom - 1) {
					return false;
				}
			} else 	if (P.X == Left + 1 || P.X == Right - 1) {
				return false;
			}

			return base.CanConnect(R, P);
		}

		private bool Place(Level level, int x, int y, uint a) {
			foreach (var d in Connected.Values) {
				if ((d.X == x && (d.Y == y + 1 || d.Y == y - 1)) || (d.Y == y && (d.X == x + 1 || d.X == x - 1))) {
					return false;
				}
			}
			
			Painter.Set(level, x, y, Tile.FloorA);

			level.Area.Add(new Turret {
				Position = new Vector2(x, y) * 16,
				StartingAngle = a
			});

			return true;
		}
	}
}