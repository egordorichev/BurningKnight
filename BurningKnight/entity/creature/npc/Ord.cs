using BurningKnight.entity.component;
using BurningKnight.ui.dialog;

namespace BurningKnight.entity.creature.npc {
	public class Ord : Npc {
		public override void AddComponents() {
			base.AddComponents();
			
			Width = 16;
			Height = 20;

			var anim = new AnimationComponent("ord");
			AddComponent(anim);

			anim.Animation.AutoStop = true;
			anim.Animation.OnEnd += OnAnimationEnd;

			AddComponent(new RectBodyComponent(-4, -4, Width + 8, Height + 8));
			
			AddComponent(new InteractableComponent(e => {
				GetComponent<AnimationComponent>().Animation.Tag = "turnon";
				return true;
			}));
		}

		private void OnAnimationEnd() {
			var anim = GetComponent<AnimationComponent>().Animation;

			if (anim.Tag == "turnon") {
				anim.Tag = "choice";
			}
		}
	}
}