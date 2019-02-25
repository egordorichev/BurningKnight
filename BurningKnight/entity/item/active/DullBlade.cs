using BurningKnight.entity.creature.player;

namespace BurningKnight.entity.item.active {
	public class DullBlade : Item {
		public override void Use() {
			if (Delay > 0) return;

			base.Use();
			Player.DullDamage = true;
			Player.Instance.SetInvt(Player.Instance.InvTime);
			Player.Instance.OnHurt(-1, null);
		}
	}
}