using Lens.entity;

namespace BurningKnight.entity.creature.mob.boss {
	public class Boss : Mob {
		public override void PostInit() {
			base.PostInit();
			Become<IdleState>();
		}

		protected override void OnTargetChange(Entity target) {
			if (target == null) {
				Become<IdleState>();
			} else {
				
			}
			
			base.OnTargetChange(target);
		}

		public class IdleState : CreatureState<Boss> {
			
		}
	}
}