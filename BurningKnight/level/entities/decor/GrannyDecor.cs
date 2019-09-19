using Microsoft.Xna.Framework;

namespace BurningKnight.level.entities.decor {
	public class GrannyDecor : SolidProp {
		public GrannyDecor() {
			Sprite = "granny";
		}
		
		protected override Rectangle GetCollider() {
			return new Rectangle(15, 8, 30, 7);
		}
	}
}