using BurningKnight.entity.level.rooms.special;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.level.rooms.treasure {
	public class TreasureRoom : SpecialRoom {
		protected void PlaceChest(Level level, Vector2 Center) {
			/*if (Random.Chance(30)) {
				var Chest = new Mimic();
				Chest.X = Center.X * 16;
				Chest.Y = Center.Y * 16;
				Chest.Weapon = Weapon;
				Dungeon.Area.Add(Chest);
				LevelSave.Add(Chest);
			}
			else {*/
				/*Chest Chest = Chest.Random();
				Chest.X = Center.X * 16;
				Chest.Y = Center.Y * 16;
				Chest.Weapon = Weapon;
				Chest.SetItem(Chest.Generate());
				Level.Area.Add(Chest);*/
			// }
		}

		protected override int ValidateWidth(int W) {
			return W % 2 == 0 ? W : W + 1;
		}

		protected override int ValidateHeight(int H) {
			return H % 2 == 0 ? H : H + 1;
		}
	}
}