using BurningKnight.core.assets;
using BurningKnight.core.entity.item.accessory.equippable;

namespace BurningKnight.core.entity.item {
	public class Compass : Equippable {
		protected void _Init() {
			{
				Description = Locale.Get("compass_desc");
				Name = Locale.Get("compass");
				Sprite = "item-compass";
			}
		}

		public override Void OnEquip(bool Load) {
			base.OnEquip(Load);
			this.Owner.SeePath = true;
		}

		public override Void OnUnequip(bool Load) {
			base.OnUnequip(Load);
			this.Owner.SeePath = false;
		}

		public Compass() {
			_Init();
		}
	}
}
