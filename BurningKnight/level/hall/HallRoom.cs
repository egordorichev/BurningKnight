using BurningKnight.assets.items;
using BurningKnight.entity.creature.npc;
using BurningKnight.entity.item;
using BurningKnight.level.entities;
using BurningKnight.level.rooms.entrance;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.hall {
	public class HallRoom : ExitRoom {
		public override void Paint(Level level) {
			Painter.Prefab(level, "hall", Left, Top);

			var man = new OldMan();
			level.Area.Add(man);
			man.Center = new Vector2(GetCenter().X, Top + 2.5f);
			
			Entrance entrance;
			
			level.Area.Add(entrance = new Entrance {
				To = -1
			});

			entrance.Center = new Vector2(GetCenter().X, Bottom - 1.5f) * 16;

			PlaceCampfire(level, new Vector2(Left + 7f, Top + 3f) * 16);
			PlaceCampfire(level, new Vector2(Right - 6f, Top + 3f) * 16);

			var weapons = new Item[6];
			weapons[3] = Items.CreateAndAdd("bk:sword", level.Area);
			
			PlaceRow(level, new Vector2(Left + 6f, Top + 5f) * 16, weapons);
			
			var lamps = new Item[6];
			lamps[0] = Items.CreateAndAdd("bk:lamp", level.Area);
			
			PlaceRow(level, new Vector2(Right - 6f, Top + 5f) * 16, lamps);
		}
		
		private void PlaceStand(Level level, Vector2 where, Item item) {
			var stand = new ItemStand();
			level.Area.Add(stand);
			stand.Center = where;

			if (item != null) {
				stand.SetItem(item, null);
			}
		}

		private void PlaceRow(Level level, Vector2 where, Item[] items) {
			var h = 3;

			for (int x = 0; x < 2; x++) {
				for (int y = 0; y < h; y++) {
					PlaceStand(level, where + new Vector2(x, y) * 32, items[x * h + y]);
				}
			}
		}
		
		private void PlaceCampfire(Level level, Vector2 where) {
			var campfire = new Campfire();
			level.Area.Add(campfire);

			campfire.CenterX = where.X;
			campfire.Bottom = where.Y;
		}

		public override void PaintFloor(Level level) {
			
		}

		public override int GetMinWidth() {
			return 21;
		}

		public override int GetMaxWidth() {
			return 22;
		}

		public override int GetMinHeight() {
			return 11;
		}
		
		public override int GetMaxHeight() {
			return 12;
		}
	}
}