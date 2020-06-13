using System;
using BurningKnight.assets.items;
using BurningKnight.entity.item;
using BurningKnight.entity.item.stand;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.regular {
	public class ItemTrollRoom : RegularRoom {
		public override void Paint(Level level) {
			var w = GetWidth();
			var h = GetHeight();
			var s = Math.Min(w, h);

			s = Math.Max(5, Rnd.Int(5, s - 5));

			var center = GetTileCenter();
			var stand = new ItemStand();
			
			level.Area.Add(stand);
			stand.Center = center * 16 + new Vector2(8, 8);
			stand.SetItem(Items.CreateAndAdd(Items.Generate(ItemPool.Treasure), level.Area), null);

			var c = (int) Math.Floor(s / 2f);
			var t = Rnd.Chance() ? Tile.Rock : Tile.MetalBlock;

			for (var x = center.X - c; x <= center.X + c; x++) {
				for (var y = center.Y - c; y <= center.Y + c; y++) {
					if (Math.Abs(x - center.X) + Math.Abs(y - center.Y) <= c && (center.X != x || center.Y != y)) {
						Painter.Set(level, new Dot(x, y), t);
					}
				}
			}
			
			Painter.Rect(level, this, 1, Tiles.RandomFloor());
		}

		public override bool ShouldSpawnMobs() {
			return false;
		}

		protected override int ValidateWidth(int W) {
			return W % 2 == 0 ? W : W - 1;
		}

		protected override int ValidateHeight(int H) {
			return H % 2 == 0 ? H : H - 1;
		}
	}
}