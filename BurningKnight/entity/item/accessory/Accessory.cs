namespace BurningKnight.entity.item.accessory {
	public class Accessory : Item {
		public bool Equipped;

		public Accessory() {
			_Init();
		}

		protected void _Init() {
			{
				Useable = false;
			}
		}

		public override int GetPrice() {
			return 10;
		}

		public void OnEquip(bool Load) {
		}

		public void OnUnequip(bool Load) {
		}
	}
}