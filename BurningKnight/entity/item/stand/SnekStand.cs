namespace BurningKnight.entity.item.stand {
	public class SnekStand : ShopStand {
		protected override string GetSprite() {
			return "snek_stand";
		}

		public override ItemPool GetPool() {
			return ItemPool.Snek;
		}
	}
}