using BurningKnight.entity.creature.player;

namespace BurningKnight.entity.item.accessory.hat {
	public class Hat : Accessory {
		protected int Defense = 2;
		public string Skin;

		public Hat() {
			_Init();
		}

		protected void _Init() {
			{
				Useable = true;
			}
		}

		public override void Use() {
			base.Use();
			int I;

			for (I = 0;; I < Player.Instance.GetInventory().GetSize() ;
			I++) {
				if (Player.Instance.GetInventory().GetSlot(I) == this) break;
			}

			var Ui = Player.Instance.Ui;
			Item Item = Ui.GetInventory().GetSlot(6);
			Ui.GetInventory().SetSlot(6, Ui.GetInventory().GetSlot(I));
			Ui.GetInventory().SetSlot(I, Item);

			if (Item != null) {
				((Accessory) Item).Equipped = false;
				((Accessory) Item).OnUnequip(false);
			}

			var Ac = (Accessory) Ui.GetInventory().GetSlot(6);
			Ac.OnEquip(false);
			Ac.Equipped = true;
		}

		public override void OnEquip(bool Load) {
			base.OnEquip(Load);

			if (Owner is Player) ((Player) Owner).SetHat(Skin);
		}

		public override void OnUnequip(bool Load) {
			base.OnUnequip(Load);

			if (Owner is Player) ((Player) Owner).SetHat("");
		}
	}
}