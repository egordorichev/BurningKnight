using System;
using System.Collections.Generic;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.room;
using BurningKnight.level;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.boss.rooms {
	public class DmPadsRoom : DmRoom {
		private Dot magePos;
		private Dot playerPos;
		
		public override void PlaceMage(Room room, DM mage) {
			mage.BottomCenter = magePos * 16 + new Vector2(8);
		}

		public override void PlacePlayer(Room room, Player player) {
			player.BottomCenter = playerPos * 16 + new Vector2(8);
		}

		public override void PaintFloor(Level level) {
			
		}
		
		public override void Paint(Level level, Room room) {
			Painter.Fill(level, this, Tile.WallA);
			Painter.Fill(level, this, 1, Tile.Chasm);

			var gap = 2;
			
			var xcollumn = Rnd.Int(2, 5);
			var ycollumn = Rnd.Chance(20) ? Rnd.Int(2, 5) : xcollumn;
			var w = GetWidth();
			var h = GetHeight();
			var gx = (gap + xcollumn);
			var gy = (gap + ycollumn);

			var xcount = (int) Math.Ceiling((w - (float) xcollumn) / gx);
			var ycount = (int) Math.Ceiling((h - (float) ycollumn) / gy);

			var xw = xcount * gx - xcollumn;
			var yw = ycount * gy - ycollumn;

			var xo = (int) Math.Min(Math.Floor((w - xw) / 2f), 2);
			var yo = (int) Math.Min(Math.Floor((h - yw) / 2f), 2);

			var spots = new List<Dot>();
			
			for (var x = 0; x < xcount; x++) {
				for (var y = 0; y < ycount; y++) {
					spots.Add(new Dot(Left + xo + x * gx, Top + yo + y * gy));
				}
			}

			var i = Rnd.Int(spots.Count);
			magePos = spots[i] + new Dot(1);
			
			Painter.Fill(level, new Rect(magePos.X - 1, magePos.Y - 1).Resize(Math.Max(3, xcollumn), Math.Max(3, ycollumn)), Tiles.RandomFloor());
			Painter.Set(level, magePos.X - 1, magePos.Y - 1, Tile.WallA);
			Painter.Set(level, magePos.X + 1, magePos.Y - 1, Tile.WallA);
			Painter.Set(level, magePos.X - 1, magePos.Y + 1, Tile.WallA);
			Painter.Set(level, magePos.X + 1, magePos.Y + 1, Tile.WallA);
			
			spots.RemoveAt(i);
			
			i = Rnd.Int(spots.Count);
			playerPos = spots[i] + new Dot(1);
			
			Painter.Fill(level, new Rect(playerPos.X - 1, playerPos.Y - 1).Resize(xcollumn, ycollumn), Tiles.RandomFloor());
			spots.RemoveAt(i);

			foreach (var s in spots) {
				Painter.Fill(level, new Rect(s.X, s.Y).Resize(xcollumn, ycollumn), Rnd.Chance(5) ? Tiles.RandomFloor() : Tile.SensingSpikeTmp);
			}
		}
		
		public override int GetMinWidth() {
			return 20;
		}

		public override int GetMinHeight() {
			return 20;
		}

		public override int GetMaxWidth() {
			return 28;
		}

		public override int GetMaxHeight() {
			return 28;
		}
	}
}