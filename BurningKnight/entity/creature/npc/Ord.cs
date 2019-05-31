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
			
			GetComponent<DialogComponent>().OnNext += OnChoice;
			
			AddComponent(new InteractableComponent(e => {
				GetComponent<AnimationComponent>().Animation.Tag = "turnon";
				GetComponent<DialogComponent>().Start("ord_link");
				
				return true;
			}));
		}

		private void OnAnimationEnd() {
			var anim = GetComponent<AnimationComponent>().Animation;

			if (anim.Tag == "turnon") {
				anim.Tag = "choice";
			}
		}
		
		private void OnChoice(DialogComponent dialog) {
			var anim = GetComponent<AnimationComponent>().Animation;

			if (dialog.Last is CombineDialog c) {
				anim.Tag = c.Last == 0 ? "a" : "b";
			} else {
				anim.Tag = dialog.Current == null ? "turnoff" : "choice";
			}
		}
	}
}