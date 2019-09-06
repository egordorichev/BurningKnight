using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.state;
using BurningKnight.ui;
using Lens;
using Lens.entity;
using Lens.util.camera;
using Lens.util.timer;

namespace BurningKnight.entity.creature.mob.boss {
	public class Boss : Mob {
		public bool Awoken;
		protected HealthBar HealthBar;

		public override void Update(float dt) {
			base.Update(dt);
			
			if (HealthBar == null) {
				HealthBar = new HealthBar(this);
				Engine.Instance.State.Ui.Add(HealthBar);
			}
		}

		public override void PostInit() {
			base.PostInit();
			Become<IdleState>();
		}

		protected override void OnTargetChange(Entity target) {
			if (target == null) {
				Become<IdleState>();
			} else {
				Awoken = true;
				
				/*Camera.Instance.Targets.Clear();
				
				Timer.Add(() => {
					((InGameState) Engine.Instance.State).ResetFollowing();
					
					Timer.Add(() => {
						SelectAttack();
					}, 1f);
				}, 3f);*/
			}
			
			base.OnTargetChange(target);
		}

		public virtual void SelectAttack() {
			
		}

		public class IdleState : SmartState<Boss> {
			
		}
	}
}