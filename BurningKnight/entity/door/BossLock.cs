using BurningKnight.entity.creature.bk;
using BurningKnight.level.entities.decor;
using BurningKnight.level.rooms;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.util;

namespace BurningKnight.entity.door {
	public class BossLock : IronLock {
		private bool triggered;

		public override void Init() {
			base.Init();
			
			Subscribe<SpawnTrigger.TriggeredEvent>();
			Subscribe<creature.bk.BurningKnight.DefeatedEvent>();
			Subscribe<BurningStatue.BrokenEvent>();
		}

		public override bool HandleEvent(Event e) {
			if (e is SpawnTrigger.TriggeredEvent) {
				triggered = true;
			} else if (e is creature.bk.BurningKnight.DefeatedEvent || e is BurningStatue.BrokenEvent) {
				triggered = false;
			}
			
			return base.HandleEvent(e);
		}

		protected override void UpdateState() {
			var shouldLock = triggered;

			if (!shouldLock) {
				foreach (var r in rooms) {
					if (r.Type != RoomType.Connection && r.Tagged[Tags.Player].Count > 0 &&
					    r.Tagged[Tags.MustBeKilled].Count > 0) {
						
						shouldLock = true;
						break;
					}
				}
			}

			if (shouldLock && !IsLocked) {
				SetLocked(true, null);
				GetComponent<StateComponent>().Become<ClosingState>();
			} else if (!shouldLock && IsLocked) {
				SetLocked(false, null);
				GetComponent<StateComponent>().Become<OpeningState>();
			}
		}
	}
}