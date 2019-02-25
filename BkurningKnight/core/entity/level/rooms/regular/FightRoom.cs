using BurningKnight.core.entity.creature.mob;
using BurningKnight.core.entity.creature.mob.hall;
using BurningKnight.core.entity.level.features;
using BurningKnight.core.entity.level.save;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class FightRoom : RegularRoom {
		public override Void Paint(Level Level) {
			base.Paint(Level);

			foreach (LDoor Door in this.Connected.Values()) {
				Door.SetType(LDoor.Type.ENEMY);
			}

			if (Dungeon.Type == Dungeon.Type.INTRO) {
				Point Center = this.GetRandomCell();
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
			return { 0, 0, 1 };
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
