using System.Collections.Generic;
using BurningKnight.assets.items;
using BurningKnight.entity.item;
using BurningKnight.level.rooms.special;
using BurningKnight.level.tile;
using Lens.util;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.shop {
	public class ShopRoom : LockedRoom {
		public override void Paint(Level level) {
			Painter.Fill(level, this, 2, Tile.FloorD);

			var stands = ValidateStands(GenerateStands());

			foreach (var s in stands) {
				var stand = new ShopStand();
				level.Area.Add(stand);

				stand.Center = new Vector2(s.X * 16 + 8, s.Y * 16 + 8);
				stand.SetItem(Items.CreateAndAdd(Items.Generate(ItemPool.Shop), level.Area), null);
			}
		}

		protected List<Point> ValidateStands(List<Point> stands) {
			var list = new List<Point>();

			for (var i = 0; i < stands.Count; i++) {
				var found = false;
				var p = stands[i];
				
				for (var j = i + 1; j < stands.Count; j++) {
					var s = stands[j];

					// Check if 2 stands are on the same tile
					if (s.X == p.X && s.Y == p.Y) {
						found = true;
						break;
					}

					// Check if 2 stands are too close
					foreach (var d in MathUtils.Directions) {
						if (s.X + (int) d.X == p.X && s.Y + (int) d.Y == p.Y) {
							found = true;
							break;
						}
					}
				}

				if (!found) {
					list.Add(p);
				}
			}
			
			return list;
		}

		protected virtual List<Point> GenerateStands() {
			var list = new List<Point>();

			var sides = new bool[4];
			var set = false;

			for (var i = 0; i < 4; i++) {
				if (Random.Chance()) {
					set = true;
					sides[i] = true;
				}
			}

			if (!set) {
				sides[Random.Int(4)] = true;
			}

			for (var x = Left + (Random.Chance() ? 2 : 3); x < Right - 2; x += 2) {
				if (sides[0]) {
					list.Add(new Point(x, Top + 2));
				}

				if (sides[1]) {
					list.Add(new Point(x, Bottom - 2));
				}
			}
			
			for (var y = Top + (Random.Chance() ? 2 : 3); y < Bottom - 2; y += 2) {
				if (sides[2]) {
					list.Add(new Point(Left + 2, y));
				}

				if (sides[3]) {
					list.Add(new Point(Right - 2, y));
				}
			}

			return list;
		}
	}
}