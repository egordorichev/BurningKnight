using BurningKnight.entity.level.painters;
using BurningKnight.entity.level.save;
using BurningKnight.entity.trap;
using BurningKnight.util;

namespace BurningKnight.entity.level.rooms.regular {
	public class TurretRoomDef : TrapRoomDef {
		public override void Paint(Level Level) {
			base.Paint(Level);
			Painter.Fill(Level, this, 1, Random.Chance(50) ? Terrain.FLOOR_A : Terrain.FLOOR_B);

			if (Random.Chance(50)) Painter.FillEllipse(Level, this, 1, Random.Chance(50) ? Terrain.FLOOR_A : Terrain.FLOOR_B);

			var Wave = Random.Chance(50);
			var S = Random.NewInt(2, 4);
			var A = Random.NewFloat(1, 3);

			if (Random.Chance(50)) {
				var Left = Random.Chance(50);
				var X = (Left ? this.Left + 2 : Right - 2) * 16;
				float I = 0;

				for (var Y = Top + 2; Y < Bottom - 1; Y += S) {
					var Turret = new Turret();
					Turret.X = X;
					Turret.Y = Y * 16;

					if (Wave) {
						Turret.Last = I / A % 3;
						I++;
					}

					Turret.A = !Left ? Math.PI : 0;
					Dungeon.Area.Add(Turret);
					LevelSave.Add(Turret);
				}
			}
			else {
				var Top = Random.Chance(50);
				var Y = (Top ? this.Top + 2 : Bottom - 2) * 16;
				float I = 0;

				for (var X = Left + 2; X < Right - 1; X += S) {
					var Turret = new Turret();
					Turret.X = X * 16;
					Turret.Y = Y;

					if (Wave) {
						Turret.Last = I / A % 3;
						I++;
					}

					Turret.A = !Top ? Math.PI * 1.5f : Math.PI / 2;
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