using BurningKnight.entity.level.entities.chest;
using BurningKnight.entity.level.rooms.special;
using BurningKnight.entity.level.save;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.treasure {
	public class TreasureRoom : SpecialRoom {
		public bool Weapon;

		protected void PlaceChest(Point Center) {
			if (Random.Chance(Mimic.Chance)) {
				var Chest = new Mimic();
				Chest.X = Center.X * 16;
				Chest.Y = Center.Y * 16;
				Chest.Weapon = Weapon;
				Dungeon.Area.Add(Chest);
				LevelSave.Add(Chest);
			}
			else {
				Chest Chest = Chest.Random();
				Chest.X = Center.X * 16;
				Chest.Y = Center.Y * 16;
				Chest.Weapon = Weapon;
				Chest.SetItem(Chest.Generate());
				Dungeon.Area.Add(Chest);
				LevelSave.Add(Chest);
			}
		}

		protected override int ValidateWidth(int W) {
			return W % 2 == 0 ? W : W + 1;
		}

		protected override int ValidateHeight(int H) {
			return H % 2 == 0 ? H : H + 1;
		}
	}
}