using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.state;
using Lens;
using Lens.entity;
using Lens.util.camera;
using Lens.util.timer;

namespace BurningKnight.entity.creature.mob.boss {
	public class Boss : Mob {
		public bool Awoken;
		
		public override void PostInit() {
			base.PostInit();
			Become<IdleState>();
		}

		protected override void OnTargetChange(Entity target) {
			if (target == null) {
				Become<IdleState>();
			} else {
				Awoken = true;

				/*if (true) { // fix
					SelectAttack();
					Camera.Instance.Follow(this, 0.6f);
					return;
				}*/
				
				Camera.Instance.Targets.Clear();
				Camera.Instance.Follow(this, 1f);
				
				Timer.Add(() => {
					((InGameState) Engine.Instance.State).ResetFollowing();
					Camera.Instance.Follow(this, 0.6f);
					
					Timer.Add(() => {
						SelectAttack();
					}, 1f);
				}, 3f);
			}
			
			base.OnTargetChange(target);
		}

		public virtual void SelectAttack() {
			
		}

		public class IdleState : CreatureState<Boss> {
			
		}
	}
}