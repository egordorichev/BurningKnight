using BurningKnight.entity.level.save;
using BurningKnight.entity.trap;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.regular {
	public class RollingSpikeRoomDef : TrapRoomDef {
		public override void Paint(Level Level) {
			base.Paint(Level);
			var I = 0;
			var Sp = Random.NewInt(2, 4);

			if (Random.Chance(50))
				for (var Y = Top + 2; Y < Bottom - 1; Y += Sp) {
					var Spike = new RollingSpike();
					Spike.X = (I % 2 == 0 ? Left + 2 : Right - 2) * 16;
					Spike.Y = Y * 16;
					var S = 20f;
					Spike.Velocity = new Point(I % 2 == 0 ? S : -S, 0);
					Dungeon.Area.Add(Spike);
					LevelSave.Add(Spike);
					I++;
				}
			else
				for (var X = Left + 2; X < Right - 1; X += Sp) {
					var Spike = new RollingSpike();
					Spike.Y = (I % 2 == 0 ? Top + 2 : Bottom - 2) * 16;
					Spike.X = X * 16;
					var S = 20f;
					Spike.Velocity = new Point(0, I % 2 == 0 ? S : -S);
					Dungeon.Area.Add(Spike);
					LevelSave.Add(Spike);
					I++;
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