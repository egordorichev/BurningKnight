using BurningKnight.core.assets;

namespace BurningKnight.core.entity.item.key {
	public class Key : Item {
		protected void _Init() {
			{
				AutoPickup = true;
				Stackable = true;
				Useable = false;
			}
		}

		public override Void OnPickup() {
			base.OnPickup();
			Audio.PlaySfx("key");
		}

		public override Void Render() {

		}

		public Key() {
			_Init();
		}
	}
}
