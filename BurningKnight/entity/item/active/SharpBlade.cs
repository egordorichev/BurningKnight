using BurningKnight.entity.creature.player;

namespace BurningKnight.entity.item.active {
	public class SharpBlade : ActiveItem {
		public override void Use() {
			if (Delay > 0) return;

			base.Use();
			Player.Instance.ModifyHp(-1, null);
		}
	}
}