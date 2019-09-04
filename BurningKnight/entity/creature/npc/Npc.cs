using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.ui.dialog;
using Lens;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.util.camera;
using Lens.util.timer;
using Lens.util.tween;

namespace BurningKnight.entity.creature.npc {
	public class Npc : Creature {
		public const int Padding = 4;
		
		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new DialogComponent());
			GetComponent<HealthComponent>().Unhittable = true;
		}

		private bool rotationApplied;

		public override bool HandleEvent(Event e) {
			if (e is HealthModifiedEvent hme && hme.Amount < 0) {
				if (!rotationApplied) {
					rotationApplied = true;
					var a = GetAnyComponent<AnimationComponent>();
				
					if (a != null) {
						var w = a.Angle;
						a.Angle += 0.5f;

						var t = Tween.To(w, a.Angle, x => a.Angle = x, 0.2f);
						
						t.Delay = 0.2f;
						t.OnEnd = () => {
							rotationApplied = false;
						};
					}
				}
			}
			
			return base.HandleEvent(e);
		}
		
		private bool dying;
		
		// Same as in Mob
		public override void AnimateDeath(DiedEvent d) {
			Done = false;
			
			if (dying) {
				return;
			}

			dying = true;

			Engine.Instance.Freeze = 0.5f;
			Camera.Instance.ShakeMax(5);
			
			GetComponent<StateComponent>().Pause++;
			
			Timer.Add(() => {
				// Sets the done flag
				base.AnimateDeath(d);
			}, 0.6f);
		}
	}
}