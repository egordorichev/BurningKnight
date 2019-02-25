using BurningKnight.entity.item.accessory.equippable;

namespace BurningKnight.entity.item {
	public class Compass : Equippable {
		public Compass() {
			_Init();
		}

		protected void _Init() {
			{
				Description = Locale.Get("compass_desc");
				Name = Locale.Get("compass");
				Sprite = "item-compass";
			}
		}

		public override void OnEquip(bool Load) {
			base.OnEquip(Load);
			Owner.SeePath = true;
		}

		public override void OnUnequip(bool Load) {
			base.OnUnequip(Load);
			Owner.SeePath = false;
		}
	}
}