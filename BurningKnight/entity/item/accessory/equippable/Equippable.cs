using BurningKnight.entity.creature.player;

namespace BurningKnight.entity.item.accessory.equippable {
	public class Equippable : Accessory {
		public Player Owner = Player.Instance;

		public override void OnEquip(bool Load) {
			Owner = Player.Instance;
			base.OnEquip(Load);
		}

		public void SetOwner(Player Owner) {
			base.SetOwner(Owner);
			this.Owner = Owner;
		}

		public override StringBuilder BuildInfo() {
			StringBuilder Builder = base.BuildInfo();
			Builder.Append("\n[green]Equippable[gray]");

			return Builder;
		}

		public override bool IsUseable() {
			return true;
		}

		public override bool CanBeUsed() {
			return false;
		}

		public override void Use() {
			base.Use();
			Audio.PlaySfx("menu/select");

			for (var I = 0; I < Player.Instance.GetInventory().GetSize(); I++)
				if (Player.Instance.GetInventory().GetSlot(I) == this) {
					Player.Instance.GetInventory().SetSlot(I, null);

					break;
				}

			for (var I = 7; I < 11; I++)
				if (Player.Instance.GetInventory().GetSlot(I) == null) {
					Player.Instance.GetInventory().SetSlot(I, this);
					OnEquip(false);

					return;
				}
		}
	}
}