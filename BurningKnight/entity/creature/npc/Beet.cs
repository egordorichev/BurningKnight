using BurningKnight.entity.component;
using BurningKnight.ui.dialog;
using Lens.entity;
using Lens.entity.component.logic;

namespace BurningKnight.entity.creature.npc {
	public class Beet : Npc {
		public override void AddComponents() {
			base.AddComponents();

			Width = 18;
			Height = 19;

			AddComponent(new AnimationComponent("beet"));
			AddComponent(new RectBodyComponent(0, 0, Width, Height));
			AddComponent(new InteractableComponent(Interact));
			
			GetComponent<StateComponent>().Become<IdleState>();
		}

		private bool Interact(Entity e) {
			var state = GetComponent<StateComponent>();

			if (state.StateInstance is IdleState) {
				state.Become<PopState>();
			} 

			return true;
		}
		
		#region Beet States
		public class IdleState : CreatureState<Beet> {
			
		}
		
		public class PopState : CreatureState<Beet> {
			public override void Init() {
				base.Init();
				Self.GetComponent<AnimationComponent>().SetAutoStop(true);
			}

			public override void Destroy() {
				base.Destroy();
				Self.GetComponent<AnimationComponent>().SetAutoStop(false);
			}

			public override void Update(float dt) {
				base.Update(dt);
				
				if (Self.GetComponent<AnimationComponent>().Animation.Paused) {
					Self.GetComponent<StateComponent>().Become<PoppedState>();
				}
			}
		}
		
		public class HideState : CreatureState<Beet> {
			public override void Init() {
				base.Init();
				Self.GetComponent<AnimationComponent>().SetAutoStop(true);
			}

			public override void Destroy() {
				base.Destroy();
				Self.GetComponent<AnimationComponent>().SetAutoStop(false);
			}

			public override void Update(float dt) {
				base.Update(dt);
				
				if (Self.GetComponent<AnimationComponent>().Animation.Paused) {
					Self.GetComponent<StateComponent>().Become<IdleState>();
				}
			}
		}
		
		public class PoppedState : CreatureState<Beet> {
			public override void Init() {
				base.Init();
				Self.GetComponent<DialogComponent>().Start("beet_0");
			}
		}
		#endregion
	}
}