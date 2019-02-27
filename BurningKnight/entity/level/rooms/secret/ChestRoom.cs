using BurningKnight.entity.level.entities.chest;
using BurningKnight.entity.level.painters;
using BurningKnight.entity.level.save;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.secret {
	public class ChestRoomDef : SecretRoomDef {
		public override void Paint(Level Level) {
			base.Paint(Level);
			var F = Terrain.RandomFloor();
			var Fl = Random.Chance(30) ? Terrain.WALL : (Random.Chance(50) ? Terrain.CHASM : Terrain.LAVA);
			Painter.Fill(Level, this, Terrain.WALL);

			if (Fl == Terrain.LAVA) F = Random.Chance(40) ? Terrain.WATER : Terrain.DIRT;

			Painter.Fill(Level, this, 1, Terrain.FLOOR_D);
			Painter.Fill(Level, this, 2, Fl);
			var El = Random.Chance(50);

			if (El)
				Painter.FillEllipse(Level, this, 3, F);
			else
				Painter.Fill(Level, this, 3, F);


			var S = false;

			if (Random.Chance(50)) {
				Painter.Set(Level, new Point(GetWidth() / 2 + Left, Top + 2), F);
				S = true;
			}

			if (Random.Chance(50)) {
				Painter.Set(Level, new Point(GetWidth() / 2 + Left, Bottom - 2), F);
				S = true;
			}

			if (Random.Chance(50)) {
				Painter.Set(Level, new Point(Left + 2, GetHeight() / 2 + Top), F);
				S = true;
			}

			if (Random.Chance(50) || !S) Painter.Set(Level, new Point(Right - 2, GetHeight() / 2 + Top), F);

			if (Random.Chance(50)) {
				var Flr = Terrain.RandomFloor();

				if (Random.Chance(50)) {
					Painter.Fill(Level, new Rect(Left + 1, Top + 1, Left + 4, Top + 4), Flr);
					Painter.Fill(Level, new Rect(Right - 3, Top + 1, Right, Top + 4), Flr);
					Painter.Fill(Level, new Rect(Left + 1, Bottom - 3, Left + 4, Bottom), Flr);
					Painter.Fill(Level, new Rect(Right - 3, Bottom - 3, Right, Bottom), Flr);
				}
				else {
					Painter.FillEllipse(Level, new Rect(Left + 1, Top + 1, Left + 4, Top + 4), Flr);
					Painter.FillEllipse(Level, new Rect(Right - 3, Top + 1, Right, Top + 4), Flr);
					Painter.FillEllipse(Level, new Rect(Left + 1, Bottom - 3, Left + 4, Bottom), Flr);
					Painter.FillEllipse(Level, new Rect(Right - 3, Bottom - 3, Right, Bottom), Flr);
				}
			}

			var Center = GetCenter();

			if (Random.Chance(Mimic.Chance)) {
				var Chest = new Mimic();
				Chest.X = Center.X * 16;
				Chest.Y = Center.Y * 16;
				Dungeon.Area.Add(Chest);
				LevelSave.Add(Chest);
			}
			else {
				Chest Chest = Chest.Random();
				Chest.X = Center.X * 16;
				Chest.Y = Center.Y * 16;
				Chest.SetItem(Chest.Generate());
				Dungeon.Area.Add(Chest);
				LevelSave.Add(Chest);
			}


			AddEnemies();
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