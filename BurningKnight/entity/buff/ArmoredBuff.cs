using BurningKnight.entity.component;
using BurningKnight.entity.events;
using Lens.entity;

namespace BurningKnight.entity.buff {
	public class ArmoredBuff : Buff {
		public const string Id = "bk:armored";
		
		public ArmoredBuff() : base(Id) {
			Duration = 10;
		}

		public override void Init() {
			base.Init();
			Entity.GetComponent<BuffsComponent>().Remove<BrokenArmorBuff>();
		}

		public override void HandleEvent(Event e) {
			if (e is HealthModifiedEvent hme) {
				if (hme.Amount < 0) {
					hme.Amount /= 2;
				}
			}
			
			base.HandleEvent(e);
		}

		public override string GetIcon() {
			return "armor";
		}
	}
}