using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.ui.dialog;
using Lens.entity;
using Lens.util.tween;

namespace BurningKnight.entity.creature.npc {
	public class Npc : Creature {
		public const int Padding = 4;
		
		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new DialogComponent());
			GetComponent<HealthComponent>().Unhittable = true;
			
			AddTag(Tags.Npc);
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
		
		public override void AnimateDeath(DiedEvent d) {
			base.AnimateDeath(d);
			CreateGore(d);
		}
	}
}