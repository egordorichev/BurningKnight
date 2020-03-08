using BurningKnight.assets.items;
using BurningKnight.entity.item;
using BurningKnight.entity.item.stand;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.special {
	public class HeartRoom : SpecialRoom {
		public override void Paint(Level level) {
			Painter.Fill(level, this, Tiles.Pick(Tile.WallA, Tile.Chasm));

			var c = GetTileCenter();
			var s = (GetWidth() - 1) / 2;

			var f = Tiles.RandomFloor();
			
			Painter.Triangle(level, new Dot(c.X, Bottom - 1), new Dot(Left + 1, c.Y), new Dot(Right, c.Y), f);
			Painter.FillEllipse(level, c.X - s + 1, c.Y - s / 2 - 1, s, s, f);
			Painter.FillEllipse(level, c.X, c.Y - s / 2 - 1, s, s, f);
			
			PaintTunnel(level, Tiles.RandomNewFloor(), GetCenterRect());
			
			var stand = new HealthStand();
			level.Area.Add(stand);
			stand.Center = c * 16 + new Vector2(8);
			
			stand.SetItem(Items.CreateAndAdd(Items.Generate(ItemType.Artifact, data => data.Quality == ItemQuality.Wooden), level.Area), null);
		}

		public override int GetMinWidth() {
			return 11;
		}

		public override int GetMinHeight() {
			return 11;
		}

		protected override int ValidateWidth(int W) {
			return W % 2 == 0 ? W : W - 1;
		}
		
		protected override int ValidateHeight(int H) {
			return H % 2 == 0 ? H : H - 1;
		}

		protected override bool Quad() {
			return true;
		}
	}
}