namespace BurningKnight.entity.item.key {
	public class BurningKey : Key {
		public BurningKey() {
			_Init();
		}

		protected void _Init() {
			{
				AutoPickup = false;
				Sprite = "item-burning_key";
			}
		}

		public override void OnPickup() {
			base.OnPickup();
		}
	}
}