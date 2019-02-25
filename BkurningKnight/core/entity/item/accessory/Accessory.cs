namespace BurningKnight.core.entity.item.accessory {
	public class Accessory : Item {
		protected void _Init() {
			{
				Useable = false;
			}
		}

		public bool Equipped;

		public override int GetPrice() {
			return 10;
		}

		public Void OnEquip(bool Load) {

		}

		public Void OnUnequip(bool Load) {

		}

		public Accessory() {
			_Init();
		}
	}
}
