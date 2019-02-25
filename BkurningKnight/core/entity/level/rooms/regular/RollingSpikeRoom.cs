using BurningKnight.core.entity.level.save;
using BurningKnight.core.entity.trap;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class RollingSpikeRoom : TrapRoom {
		public override Void Paint(Level Level) {
			base.Paint(Level);
			int I = 0;
			int Sp = Random.NewInt(2, 4);

			if (Random.Chance(50)) {
				for (int Y = this.Top + 2; Y < this.Bottom - 1; Y += Sp) {
					RollingSpike Spike = new RollingSpike();
					Spike.X = ((I % 2 == 0) ? this.Left + 2 : this.Right - 2) * 16;
					Spike.Y = Y * 16;
					float S = 20f;
					Spike.Velocity = new Point(I % 2 == 0 ? S : -S, 0);
					Dungeon.Area.Add(Spike);
					LevelSave.Add(Spike);
					I++;
				}
			} else {
				for (int X = this.Left + 2; X < this.Right - 1; X += Sp) {
					RollingSpike Spike = new RollingSpike();
					Spike.Y = ((I % 2 == 0) ? this.Top + 2 : this.Bottom - 2) * 16;
					Spike.X = X * 16;
					float S = 20f;
					Spike.Velocity = new Point(0, I % 2 == 0 ? S : -S);
					Dungeon.Area.Add(Spike);
					LevelSave.Add(Spike);
					I++;
				}
			}

		}

		public override int GetMinWidth() {
			return 7;
		}

		public override int GetMinHeight() {
			return 7;
		}
	}
}
