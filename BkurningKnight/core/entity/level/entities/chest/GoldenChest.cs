using BurningKnight.core.entity.creature.fx;
using BurningKnight.core.entity.item;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.level.entities.chest {
	public class GoldenChest : Chest {
		public static Animation Animation = Animation.Make("chest", "-golden");
		private static AnimationData Closed = Animation.Get("idle");
		private static AnimationData Open = Animation.Get("opening");
		private static AnimationData Openend = Animation.Get("open");

		protected override Animation GetAnim() {
			return Animation;
		}

		public override Item Generate() {
			return Chest.Generate(ItemRegistry.Quality.GOLDEN, Weapon);
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

		public override Void Open() {
			base.Open();

			if (Random.Chance(70)) {
				for (int I = 0; I < Random.NewInt(2, 5); I++) {
					HeartFx Fx = new HeartFx();
					Fx.X = this.X + (this.W - Fx.W) / 2;
					Fx.Y = this.Y + (this.H - Fx.H) / 2;
					Dungeon.Area.Add(Fx);
					Fx.RandomVelocity();
				}
			} 
		}
	}
}
