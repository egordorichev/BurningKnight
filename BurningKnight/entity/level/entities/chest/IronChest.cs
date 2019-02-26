using BurningKnight.entity.item;
using BurningKnight.util;

namespace BurningKnight.entity.level.entities.chest {
	public class IronChest : Chest {
		public static Animation Animation = Animation.Make("chest", "-iron");
		private static AnimationData Closed = Animation.Get("idle");
		private static AnimationData Open = Animation.Get("opening");
		private static AnimationData Openend = Animation.Get("open");

		protected override Animation GetAnim() {
			return Animation;
		}

		public override Item Generate() {
			return Generate(ItemRegistry.Quality.Iron, Weapon);
		}

		protected override AnimationData GetClosedAnim() {
			return Closed;
		}

		protected override AnimationData GetOpenAnim() {
			return Open;
		}

		protected override AnimationData GetOpenedAnim() {
			return Openend;
		}
	}
}