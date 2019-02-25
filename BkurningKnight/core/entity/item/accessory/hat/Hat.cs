using BurningKnight.core.entity.creature.inventory;
using BurningKnight.core.entity.creature.player;

namespace BurningKnight.core.entity.item.accessory.hat {
	public class Hat : Accessory {
		protected void _Init() {
			{
				Useable = true;
			}
		}

		protected int Defense = 2;
		public string Skin;

		public override Void Use() {
			base.Use();
			int I;

			for (I = 0;
; I < Player.Instance.GetInventory().GetSize(); I++) {
				if (Player.Instance.GetInventory().GetSlot(I) == this) {
					break;
				} 
			}

			UiInventory Ui = Player.Instance.Ui;
			Item Item = Ui.GetInventory().GetSlot(6);
			Ui.GetInventory().SetSlot(6, Ui.GetInventory().GetSlot(I));
			Ui.GetInventory().SetSlot(I, Item);

			if (Item != null) {
				((Accessory) Item).Equipped = false;
				((Accessory) Item).OnUnequip(false);
			} 

			Accessory Ac = ((Accessory) Ui.GetInventory().GetSlot(6));
			Ac.OnEquip(false);
			Ac.Equipped = true;
		}

		public override Void OnEquip(bool Load) {
			base.OnEquip(Load);

			if (this.Owner is Player) {
				((Player) this.Owner).SetHat(this.Skin);
			} 
		}

		public override Void OnUnequip(bool Load) {
			base.OnUnequip(Load);

			if (this.Owner is Player) {
				((Player) this.Owner).SetHat("");
			} 
		}

		public Hat() {
			_Init();
		}
	}
}
