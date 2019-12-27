using BurningKnight.entity.events;
using Lens.entity;

namespace BurningKnight.entity.buff {
	public class InvincibleBuff : Buff {
		public const string Id = "bk:invincible";
		
		public InvincibleBuff() : base(Id) {
			Duration = 10;
		}

		public override void HandleEvent(Event e) {
			if (e is HealthModifiedEvent hme) {
				hme.Handled = true;
			}
			
			base.HandleEvent(e);
		}

		public override string GetIcon() {
			return "star";
		}
	}
}