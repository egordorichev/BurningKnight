namespace BurningKnight.entity.item.key {
	public class Key : Item {
		public Key() {
			_Init();
		}

		protected void _Init() {
			{
				AutoPickup = true;
				Stackable = true;
				Useable = false;
			}
		}

		public override void OnPickup() {
			base.OnPickup();
			Audio.PlaySfx("key");
		}

		public override void Render() {
		}
	}
}