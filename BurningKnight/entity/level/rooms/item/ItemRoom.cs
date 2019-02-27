using BurningKnight.entity.item;
using BurningKnight.entity.item.weapon.sword;
using BurningKnight.entity.level.entities;
using BurningKnight.entity.level.rooms.regular;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.item {
	public class ItemRoomDef : RegularRoomDef {
		protected void PlaceItem(Point Point) {
			Item Item = null;

			if (Item == null) {
				if (Dungeon.Level.ItemsToSpawn.Size() == 0)
					Item = new Sword();
				else
					Item = Dungeon.Level.ItemsToSpawn.Get(Random.NewInt(Dungeon.Level.ItemsToSpawn.Size()));
			}
			else {
				Dungeon.Level.ItemsToSpawn.Remove(Item);
			}


			var Slab = new Slab();
			Slab.X = Point.X * 16;
			Slab.Y = Point.Y * 16 - 8;
			Dungeon.Area.Add(Slab.Add());
			var Holder = new ItemHolder(Item);
			Holder.GetItem().Generate();
			Holder.X = Point.X * 16 + (16 - Holder.W) / 2;
			Holder.Y = Point.Y * 16 + (16 - Holder.H) / 2 - 8;
			Dungeon.Area.Add(Holder.Add());
		}

		protected override int ValidateWidth(int W) {
			return W % 2 == 0 ? W : W + 1;
		}

		protected override int ValidateHeight(int H) {
			return H % 2 == 0 ? H : H + 1;
		}
	}
}