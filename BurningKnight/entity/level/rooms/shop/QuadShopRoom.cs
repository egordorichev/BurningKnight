using BurningKnight.entity.level.painters;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.shop {
	public class QuadShopRoom : ShopRoom {
		protected Rect Quad;

		public override void Paint(Level Level) {
			base.Paint(Level);
			var V = Math.Min(GetWidth(), GetHeight()) - 4;
			Quad = new Rect(Left + (GetWidth() - V) / 2, Top + (GetHeight() - V) / 2, Left + (GetWidth() + V) / 2, Top + (GetHeight() + V) / 2);
			Painter.Fill(Level, Quad, Terrain.FLOOR_D);

			if (Random.Chance(70)) {
				if (Random.Chance(50)) {
					if (Random.Chance(50))
						Painter.Fill(Level, Quad, 1, Terrain.CHASM);
					else
						Painter.FillEllipse(Level, Quad, 1, Terrain.CHASM);
				}
				else {
					if (Random.Chance(50))
						Painter.Fill(Level, Quad, 1, Terrain.RandomFloor());
					else
						Painter.FillEllipse(Level, Quad, 1, Terrain.RandomFloor());
				}
			}

			PlaceItems();
		}

		public override int GetMinWidth() {
			return 8;
		}

		public override int GetMaxWidth() {
			return 12;
		}

		public override int GetMinHeight() {
			return 8;
		}

		public override int GetMaxHeight() {
			return 12;
		}

		protected override int GetItemCount() {
			return 4;
		}

		protected override bool Quad() {
			return true;
		}

		protected override void PlaceItems(List Items) {
			PlaceItem(Items.Get(0), Quad.Left * 16, Quad.Top * 16);
			PlaceItem(Items.Get(1), Quad.Right * 16 - 16, Quad.Top * 16);
			PlaceItem(Items.Get(2), Quad.Left * 16, Quad.Bottom * 16 - 16);
			PlaceItem(Items.Get(3), Quad.Right * 16 - 16, Quad.Bottom * 16 - 16);
		}
	}
}