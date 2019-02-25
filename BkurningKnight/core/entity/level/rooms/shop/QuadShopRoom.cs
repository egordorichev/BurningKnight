using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.shop {
	public class QuadShopRoom : ShopRoom {
		protected Rect Quad;

		public override Void Paint(Level Level) {
			base.Paint(Level);
			int V = Math.Min(this.GetWidth(), this.GetHeight()) - 4;
			Quad = new Rect(this.Left + (this.GetWidth() - V) / 2, this.Top + (this.GetHeight() - V) / 2, this.Left + (this.GetWidth() + V) / 2, this.Top + (this.GetHeight() + V) / 2);
			Painter.Fill(Level, Quad, Terrain.FLOOR_D);

			if (Random.Chance(70)) {
				if (Random.Chance(50)) {
					if (Random.Chance(50)) {
						Painter.Fill(Level, Quad, 1, Terrain.CHASM);
					} else {
						Painter.FillEllipse(Level, Quad, 1, Terrain.CHASM);
					}

				} else {
					if (Random.Chance(50)) {
						Painter.Fill(Level, Quad, 1, Terrain.RandomFloor());
					} else {
						Painter.FillEllipse(Level, Quad, 1, Terrain.RandomFloor());
					}

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

		protected override Void PlaceItems(List Items) {
			PlaceItem(Items.Get(0), Quad.Left * 16, Quad.Top * 16);
			PlaceItem(Items.Get(1), Quad.Right * 16 - 16, Quad.Top * 16);
			PlaceItem(Items.Get(2), Quad.Left * 16, Quad.Bottom * 16 - 16);
			PlaceItem(Items.Get(3), Quad.Right * 16 - 16, Quad.Bottom * 16 - 16);
		}
	}
}
