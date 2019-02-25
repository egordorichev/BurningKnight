namespace BurningKnight.core.entity.level.rooms.regular.boss {
	public class DesertBossRoom : RegularRoom {
		public override Void Paint(Level Level) {
			base.Paint(Level);
			bool[] Patch = Patch.Generate(this.GetWidth(), this.GetHeight(), 0.4f, 4);

			for (int X = 1; X < this.GetWidth() - 1; X++) {
				for (int Y = 1; Y < this.GetHeight() - 1; Y++) {
					if (Patch[X + Y * this.GetWidth()]) {
						Level.Set(this.Left + X, this.Top + Y, Terrain.FLOOR_B);
					} 
				}
			}
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
