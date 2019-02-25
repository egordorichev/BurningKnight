namespace BurningKnight.core.entity.item.key {
	public class BurningKey : Key {
		protected void _Init() {
			{
				AutoPickup = false;
				Sprite = "item-burning_key";
			}
		}

		public override Void OnPickup() {
			base.OnPickup();
		}

		public BurningKey() {
			_Init();
		}
	}
}
