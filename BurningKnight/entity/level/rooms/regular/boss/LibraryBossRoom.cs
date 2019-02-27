namespace BurningKnight.entity.level.rooms.regular.boss {
	public class LibraryBossRoomDef : RegularRoomDef {
		public override void Paint(Level Level) {
			base.Paint(Level);
			bool[] Patch = Patch.Generate(GetWidth(), GetHeight(), 0.4f, 4);

			for (var X = 1; X < GetWidth() - 1; X++)
			for (var Y = 1; Y < GetHeight() - 1; Y++)
				if (Patch[X + Y * GetWidth()])
					Level.Set(Left + X, Top + Y, Terrain.FLOOR_B);
		}

		public override int GetMinWidth() {
			return 25;
		}

		public override int GetMaxWidth() {
			return 26;
		}

		public override int GetMinHeight() {
			return 25;
		}

		public override int GetMaxHeight() {
			return 26;
		}
	}
}