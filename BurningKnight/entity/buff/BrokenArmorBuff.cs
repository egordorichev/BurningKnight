using BurningKnight.entity.events;
using Lens.entity;

namespace BurningKnight.entity.buff {
	public class BrokenArmorBuff : Buff {
		public const string Id = "bk:broken_armor";
		
		public BrokenArmorBuff() : base(Id) {
			Duration = 300;
		}

		public override void HandleEvent(Event e) {
			if (e is HealthModifiedEvent hme) {
				if (hme.Amount < 0) {
					hme.Amount *= 2;
				}
			}
			
			base.HandleEvent(e);
		}
	}
}