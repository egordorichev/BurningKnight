using BurningKnight.core.entity.creature.fx;
using BurningKnight.core.entity.item;
using BurningKnight.core.entity.item.key;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.level.entities.chest {
	public class WoodenChest : Chest {
		public static Animation Animation = Animation.Make("chest", "-wooden");
		private static AnimationData Closed = Animation.Get("idle");
		private static AnimationData Open = Animation.Get("opening");
		private static AnimationData Opened = Animation.Get("open");

		protected override Animation GetAnim() {
			return Animation;
		}

		public override Item Generate() {
			return Chest.Generate(ItemRegistry.Quality.WOODEN, Weapon);
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

		public override Void Open() {
			base.Open();

			if (Dungeon.Depth == -3) {
				return;
			} 

			if (Random.Chance(50)) {
				HeartFx Fx = new HeartFx();
				Fx.X = this.X + (this.W - Fx.W) / 2;
				Fx.Y = this.Y + (this.H - Fx.H) / 2;
				Dungeon.Area.Add(Fx);
				Fx.RandomVelocity();
			} 

			if (Random.Chance(10)) {
				ItemHolder Fx = new ItemHolder(new KeyC());
				Fx.X = this.X + (this.W - Fx.W) / 2;
				Fx.Y = this.Y + (this.H - Fx.H) / 2;
				Dungeon.Area.Add(Fx);
			} 

			if (Random.Chance(10)) {
				ItemHolder Fx = new ItemHolder(new Gold());
				Fx.GetItem().Generate();
				Fx.X = this.X + (this.W - Fx.W) / 2;
				Fx.Y = this.Y + (this.H - Fx.H) / 2;
				Dungeon.Area.Add(Fx);
			} 
		}
	}
}
