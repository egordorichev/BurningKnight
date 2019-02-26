using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.mob.hall;
using BurningKnight.entity.level.features;
using BurningKnight.entity.level.save;

namespace BurningKnight.entity.level.rooms.regular {
	public class FightRoom : RegularRoom {
		public override void Paint(Level Level) {
			base.Paint(Level);

			foreach (LDoor Door in Connected.Values()) Door.SetType(LDoor.Type.ENEMY);

			if (Dungeon.Type == Dungeon.Type.INTRO) {
				var Center = GetRandomCell();
				Mob Mob = new Knight();
				Mob.X = Center.X * 16;
				Mob.Y = Center.Y * 16;
				Mob.Generate();
				Dungeon.Area.Add(Mob);
				LevelSave.Add(Mob);
				Mob.ModifyHp(-4, null);
			}
		}

		protected float GetSizeChance() {
			return {
				0, 0, 1
			}
			;
		}

		public override int GetMinWidth() {
			return 10;
		}

		public override int GetMinHeight() {
			return 10;
		}

		public override int GetMaxWidth() {
			return 16;
		}

		public override int GetMaxHeight() {
			return 16;
		}
	}
}