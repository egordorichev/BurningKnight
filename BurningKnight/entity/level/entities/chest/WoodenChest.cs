using BurningKnight.entity.creature.fx;
using BurningKnight.entity.item;
using BurningKnight.entity.item.key;
using BurningKnight.util;

namespace BurningKnight.entity.level.entities.chest {
	public class WoodenChest : Chest {
		public static Animation Animation = Animation.Make("chest", "-wooden");
		private static AnimationData Closed = Animation.Get("idle");
		private static AnimationData Open = Animation.Get("opening");
		private static AnimationData Opened = Animation.Get("open");

		protected override Animation GetAnim() {
			return Animation;
		}

		public override Item Generate() {
			return Generate(ItemRegistry.Quality.Wooden, Weapon);
		}

		protected override AnimationData GetClosedAnim() {
			return Closed;
		}

		protected override AnimationData GetOpenAnim() {
			return Open;
		}

		protected override AnimationData GetOpenedAnim() {
			return Opened;
		}

		public override void Open() {
			base.Open();

			if (Dungeon.Depth == -3) return;

			if (Random.Chance(50)) {
				var Fx = new HeartFx();
				Fx.X = this.X + (W - Fx.W) / 2;
				Fx.Y = this.Y + (H - Fx.H) / 2;
				Dungeon.Area.Add(Fx);
				Fx.RandomVelocity();
			}

			if (Random.Chance(10)) {
				var Fx = new ItemHolder(new KeyC());
				Fx.X = this.X + (W - Fx.W) / 2;
				Fx.Y = this.Y + (H - Fx.H) / 2;
				Dungeon.Area.Add(Fx);
			}

			if (Random.Chance(10)) {
				var Fx = new ItemHolder(new Gold());
				Fx.GetItem().Generate();
				Fx.X = this.X + (W - Fx.W) / 2;
				Fx.Y = this.Y + (H - Fx.H) / 2;
				Dungeon.Area.Add(Fx);
			}
		}
	}
}