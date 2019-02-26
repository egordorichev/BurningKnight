using BurningKnight.entity.creature.fx;
using BurningKnight.entity.item;
using BurningKnight.util;

namespace BurningKnight.entity.level.entities.chest {
	public class GoldenChest : Chest {
		public static Animation Animation = Animation.Make("chest", "-golden");
		private static AnimationData Closed = Animation.Get("idle");
		private static AnimationData Open = Animation.Get("opening");
		private static AnimationData Openend = Animation.Get("open");

		protected override Animation GetAnim() {
			return Animation;
		}

		public override Item Generate() {
			return Generate(ItemRegistry.Quality.Golden, Weapon);
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

		public override void Open() {
			base.Open();

			if (Random.Chance(70))
				for (var I = 0; I < Random.NewInt(2, 5); I++) {
					var Fx = new HeartFx();
					Fx.X = this.X + (W - Fx.W) / 2;
					Fx.Y = this.Y + (H - Fx.H) / 2;
					Dungeon.Area.Add(Fx);
					Fx.RandomVelocity();
				}
		}
	}
}