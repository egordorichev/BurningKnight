using BurningKnight.core.entity.level.painters;
using BurningKnight.core.entity.level.save;
using BurningKnight.core.entity.trap;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class TurretRoom : TrapRoom {
		public override Void Paint(Level Level) {
			base.Paint(Level);
			Painter.Fill(Level, this, 1, Random.Chance(50) ? Terrain.FLOOR_A : Terrain.FLOOR_B);

			if (Random.Chance(50)) {
				Painter.FillEllipse(Level, this, 1, Random.Chance(50) ? Terrain.FLOOR_A : Terrain.FLOOR_B);
			} 

			bool Wave = Random.Chance(50);
			int S = Random.NewInt(2, 4);
			float A = Random.NewFloat(1, 3);

			if (Random.Chance(50)) {
				bool Left = Random.Chance(50);
				int X = (Left ? (this.Left + 2) : (this.Right - 2)) * 16;
				float I = 0;

				for (int Y = this.Top + 2; Y < this.Bottom - 1; Y += S) {
					Turret Turret = new Turret();
					Turret.X = X;
					Turret.Y = Y * 16;

					if (Wave) {
						Turret.Last = I / A % 3;
						I++;
					} 

					Turret.A = (float) (!Left ? Math.PI : 0);
					Dungeon.Area.Add(Turret);
					LevelSave.Add(Turret);
				}
			} else {
				bool Top = Random.Chance(50);
				int Y = (Top ? (this.Top + 2) : (this.Bottom - 2)) * 16;
				float I = 0;

				for (int X = this.Left + 2; X < this.Right - 1; X += S) {
					Turret Turret = new Turret();
					Turret.X = X * 16;
					Turret.Y = Y;

					if (Wave) {
						Turret.Last = I / A % 3;
						I++;
					} 

					Turret.A = (float) (!Top ? Math.PI * 1.5f : Math.PI / 2);
					Dungeon.Area.Add(Turret);
					LevelSave.Add(Turret);
				}
			}

		}

		public override int GetMinHeight() {
			return 7;
		}

		public override int GetMinWidth() {
			return 7;
		}
	}
}
