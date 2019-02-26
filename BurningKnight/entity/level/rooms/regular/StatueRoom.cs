using BurningKnight.entity.level.entities;
using BurningKnight.entity.level.painters;
using BurningKnight.entity.level.save;
using BurningKnight.util;

namespace BurningKnight.entity.level.rooms.regular {
	public class StatueRoom : RegularRoom {
		public override void Paint(Level Level) {
			base.Paint(Level);

			if (Random.Chance(50)) {
				Collumn(Left + 2);
				Collumn(Right - 2);
			}
			else {
				Row(Top + 2);
				Row(Bottom - 2);
			}


			if (Random.Chance(50)) {
				var F = Terrain.CHASM;

				if (Random.Chance(50))
					Painter.FillEllipse(Level, this, 3, F);
				else
					Painter.Fill(Level, this, 3, F);


				if (Random.Chance(50)) {
					F = Terrain.WALL;

					if (Random.Chance(50))
						Painter.FillEllipse(Level, this, 5, F);
					else
						Painter.Fill(Level, this, 5, F);
				}
			}
		}

		private void Collumn(int X) {
			for (var Y = Top + 2; Y < Bottom - 1; Y += 3) {
				var Statue = new Statue();
				Statue.X = X * 16;
				Statue.Y = Y * 16;
				Dungeon.Area.Add(Statue);
				LevelSave.Add(Statue);
			}
		}

		private void Row(int Y) {
			for (var X = Left + 2; X < Right - 1; X += 2) {
				var Statue = new Statue();
				Statue.X = X * 16;
				Statue.Y = Y * 16;
				Dungeon.Area.Add(Statue);
				LevelSave.Add(Statue);
			}
		}

		protected override int ValidateWidth(int W) {
			return W - W % 2;
		}
	}
}