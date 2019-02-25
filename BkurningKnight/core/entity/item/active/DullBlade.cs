using BurningKnight.core.entity.creature.player;

namespace BurningKnight.core.entity.item.active {
	public class DullBlade : Item {
		public override Void Use() {
			if (Delay > 0) {
				return;
			} 

			base.Use();
			Player.DullDamage = true;
			Player.Instance.SetInvt(Player.Instance.InvTime);
			Player.Instance.OnHurt(-1, null);
		}
	}
}
