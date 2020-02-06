using BurningKnight.assets.items;
using BurningKnight.entity.item;
using BurningKnight.entity.item.stand;
using BurningKnight.level.tile;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.secret {
	public class SecretScourgeRoom : SecretRoom {
		public override void PaintFloor(Level level) {
			
		}

		public override void Paint(Level level) {
			Painter.Fill(level, this, 1, Tile.EvilFloor);

			var stand = new ScourgedStand();
			level.Area.Add(stand);
			stand.Center = GetCenter() * 16 + new Vector2(8);

			stand.SetItem(Items.CreateAndAdd(Scourge.GenerateItemId(), level.Area), stand);
		}

		protected override bool Quad() {
			return Rnd.Chance();
		}

		public override int GetMinWidth() {
			return 6;
		}

		public override int GetMinHeight() {
			return 6;
		}

		public override int GetMaxWidth() {
			return 11;
		}

		public override int GetMaxHeight() {
			return 11;
		}
	}
}