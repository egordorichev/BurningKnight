using BurningKnight.ui.editor;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.entities {
	public class Tombstone : SolidProp, PlaceableEntity {
		public Tombstone() {
			Width = 12;
			Sprite = "tombstone";
		}

		protected override Rectangle GetCollider() {
			return new Rectangle(2, 0, (int) Width - 4, (int) Height - 8);
		}
	}
}