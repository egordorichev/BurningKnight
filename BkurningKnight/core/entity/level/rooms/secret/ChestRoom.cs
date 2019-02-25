using BurningKnight.core.entity.level.entities.chest;
using BurningKnight.core.entity.level.painters;
using BurningKnight.core.entity.level.save;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.secret {
	public class ChestRoom : SecretRoom {
		public override Void Paint(Level Level) {
			base.Paint(Level);
			byte F = Terrain.RandomFloor();
			byte Fl = Random.Chance(30) ? Terrain.WALL : (Random.Chance(50) ? Terrain.CHASM : Terrain.LAVA);
			Painter.Fill(Level, this, Terrain.WALL);

			if (Fl == Terrain.LAVA) {
				F = Random.Chance(40) ? Terrain.WATER : Terrain.DIRT;
			} 

			Painter.Fill(Level, this, 1, Terrain.FLOOR_D);
			Painter.Fill(Level, this, 2, Fl);
			bool El = Random.Chance(50);

			if (El) {
				Painter.FillEllipse(Level, this, 3, F);
			} else {
				Painter.Fill(Level, this, 3, F);
			}


			bool S = false;

			if (Random.Chance(50)) {
				Painter.Set(Level, new Point(this.GetWidth() / 2 + this.Left, this.Top + 2), F);
				S = true;
			} 

			if (Random.Chance(50)) {
				Painter.Set(Level, new Point(this.GetWidth() / 2 + this.Left, this.Bottom - 2), F);
				S = true;
			} 

			if (Random.Chance(50)) {
				Painter.Set(Level, new Point(this.Left + 2, this.GetHeight() / 2 + this.Top), F);
				S = true;
			} 

			if (Random.Chance(50) || !S) {
				Painter.Set(Level, new Point(this.Right - 2, this.GetHeight() / 2 + this.Top), F);
			} 

			if (Random.Chance(50)) {
				byte Flr = Terrain.RandomFloor();

				if (Random.Chance(50)) {
					Painter.Fill(Level, new Rect(this.Left + 1, this.Top + 1, this.Left + 4, this.Top + 4), Flr);
					Painter.Fill(Level, new Rect(this.Right - 3, this.Top + 1, this.Right, this.Top + 4), Flr);
					Painter.Fill(Level, new Rect(this.Left + 1, this.Bottom - 3, this.Left + 4, this.Bottom), Flr);
					Painter.Fill(Level, new Rect(this.Right - 3, this.Bottom - 3, this.Right, this.Bottom), Flr);
				} else {
					Painter.FillEllipse(Level, new Rect(this.Left + 1, this.Top + 1, this.Left + 4, this.Top + 4), Flr);
					Painter.FillEllipse(Level, new Rect(this.Right - 3, this.Top + 1, this.Right, this.Top + 4), Flr);
					Painter.FillEllipse(Level, new Rect(this.Left + 1, this.Bottom - 3, this.Left + 4, this.Bottom), Flr);
					Painter.FillEllipse(Level, new Rect(this.Right - 3, this.Bottom - 3, this.Right, this.Bottom), Flr);
				}

			} 

			Point Center = this.GetCenter();

			if (Random.Chance(Mimic.Chance)) {
				Mimic Chest = new Mimic();
				Chest.X = Center.X * 16;
				Chest.Y = Center.Y * 16;
				Dungeon.Area.Add(Chest);
				LevelSave.Add(Chest);
			} else {
				Chest Chest = Chest.Random();
				Chest.X = Center.X * 16;
				Chest.Y = Center.Y * 16;
				Chest.SetItem(Chest.Generate());
				Dungeon.Area.Add(Chest);
				LevelSave.Add(Chest);
			}


			this.AddEnemies();
		}

		protected override int ValidateWidth(int W) {
			return W % 2 == 0 ? W - 1 : W;
		}

		protected override int ValidateHeight(int H) {
			return H % 2 == 0 ? H - 1 : H;
		}

		public override int GetMinWidth() {
			return 9;
		}

		public override int GetMinHeight() {
			return 9;
		}

		public override int GetMaxWidth() {
			return 12;
		}

		public override int GetMaxHeight() {
			return 12;
		}
	}
}
