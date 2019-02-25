using BurningKnight.core.entity.level.entities;
using BurningKnight.core.entity.level.painters;
using BurningKnight.core.entity.level.save;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class StatueRoom : RegularRoom {
		public override Void Paint(Level Level) {
			base.Paint(Level);

			if (Random.Chance(50)) {
				this.Collumn(this.Left + 2);
				this.Collumn(this.Right - 2);
			} else {
				this.Row(this.Top + 2);
				this.Row(this.Bottom - 2);
			}


			if (Random.Chance(50)) {
				byte F = Terrain.CHASM;

				if (Random.Chance(50)) {
					Painter.FillEllipse(Level, this, 3, F);
				} else {
					Painter.Fill(Level, this, 3, F);
				}


				if (Random.Chance(50)) {
					F = Terrain.WALL;

					if (Random.Chance(50)) {
						Painter.FillEllipse(Level, this, 5, F);
					} else {
						Painter.Fill(Level, this, 5, F);
					}

				} 
			} 
		}

		private Void Collumn(int X) {
			for (int Y = this.Top + 2; Y < this.Bottom - 1; Y += 3) {
				Statue Statue = new Statue();
				Statue.X = X * 16;
				Statue.Y = Y * 16;
				Dungeon.Area.Add(Statue);
				LevelSave.Add(Statue);
			}
		}

		private Void Row(int Y) {
			for (int X = this.Left + 2; X < this.Right - 1; X += 2) {
				Statue Statue = new Statue();
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
