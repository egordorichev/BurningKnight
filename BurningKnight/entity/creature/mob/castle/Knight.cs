using Lens.entity;
using Lens.entity.component.logic;

namespace BurningKnight.entity.creature.mob.castle {
	public class Knight : Mob {
		protected override void SetStats() {
			base.SetStats();
			
			AddAnimation("knight");
			SetMaxHp(10);
			
			Become<IdleState>();
		}

		protected override void OnTargetChange(Entity target) {
			if (target == null) {
				Become<IdleState>();
			} else {
				Become<ChaseState>();
			}
		}

		#region Knight States
		public class IdleState : EntityState {
			
		}

		public class ChaseState : EntityState {
			
		}
		#endregion
	}
}