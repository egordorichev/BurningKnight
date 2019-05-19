using System;
using System.Collections.Generic;
using BurningKnight.level;
using BurningKnight.level.tile;
using Lens.input;
using Lens.util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BurningKnight.ui.editor.command {
	public class FillCommand : Command {
		public Tile Tile;
		public Tile Start;
		public int X;
		public int Y;
		public bool Dir8;

		private Tile before;
		private Tile beforeLiquid;
		private List<Point> where = new List<Point>();

		private void Set(Level level, int x, int y, bool liquid) {
			if (!level.IsInside(x, y)) {
				return;
			}
			
			var tile = level.Get(x, y, liquid);

			if (tile != Start) {
				return;
			}
			
			where.Add(new Point(x, y));
			level.Set(x, y, Tile);
			level.UpdateTile(x, y);
			
			Set(level, x - 1, y, liquid);
			Set(level, x + 1, y, liquid);
			Set(level, x, y - 1, liquid);
			Set(level, x, y + 1, liquid);

			if (Dir8) {
				Set(level, x - 1, y - 1, liquid);
				Set(level, x + 1, y - 1, liquid);
				Set(level, x + 1, y + 1, liquid);
				Set(level, x - 1, y + 1, liquid);
			}
		}
		
		public void Do(Level level) {
			try {
				var i = level.ToIndex(X, Y);
				
				Dir8 = Input.Keyboard.IsDown(Keys.LeftShift);
				Start = level.Liquid[i] == 0 ? level.Get(i) : level.Get(i, true);
				
				Set(level, X, Y, Start.Matches(TileFlags.LiquidLayer));
			} catch (Exception e) {
				Log.Error(e);
				Log.Error("Failed to fill");
			}
		}

		public void Undo(Level level) {
			foreach (var p in where) {
				level.Set(p.X, p.Y, Start);
				level.UpdateTile(p.X, p.Y);
			}
			
			where.Clear();
		}
	}
}