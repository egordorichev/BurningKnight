namespace BurningKnight.level.rooms.boss {
	public class BossRoom : RoomDef {
		public override int GetMinWidth() {
			return 18 + 5;
		}

		public override int GetMinHeight() {
			return 18 + 5;
		}

		public override int GetMaxWidth() {
			return 30;
		}

		public override int GetMaxHeight() {
			return 30;
		}

		public override int GetMaxConnections(Connection Side) {
			return 1;
		}

		public override int GetMinConnections(Connection Side) {
			if (Side == Connection.All) return 1;
			return 0;
		}

		public override void Paint(Level level) {
			base.Paint(level);
			Place(level);
		}

		protected virtual void Place(Level level) {
			var bk = new entity.creature.bk.BurningKnight();
			level.Area.Add(bk);

			bk.Center = GetCenter() * 16;
		}
	}
}