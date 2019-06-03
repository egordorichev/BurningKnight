using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.ui.dialog;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.util;
using Lens.util.math;

namespace BurningKnight.entity.creature.npc {
	public class Beet : Npc {
		private Entity interactingWith;

		static Beet() {
			Dialogs.RegisterCallback("beet_4", d => {
				var a = ((AnswerDialog) d).Answer;
				Log.Error(a);
				Random.Seed = a;
			});
		}
	
		public override void AddComponents() {
			base.AddComponents();

			Width = 18;
			Height = 19;

			AddComponent(new AnimationComponent("beet"));
			AddComponent(new RectBodyComponent(-Padding, -Padding, Width + Padding * 2, Height + Padding * 2));
			AddComponent(new InteractableComponent(Interact));
			
			GetComponent<StateComponent>().Become<IdleState>();

			GetComponent<DialogComponent>().OnNext += (c) => {
				if (c.Current == null) {
					GetComponent<StateComponent>().Become<HideState>();
				}
			};
		}

		private bool Interact(Entity e) {
			var state = GetComponent<StateComponent>();

			if (state.StateInstance is IdleState) {
				interactingWith = e;
				state.Become<PopState>();
			} else {
				GetComponent<DialogComponent>().Start("beet_0", e);
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
				
				Self.GetComponent<DialogComponent>().Start("beet_0", Self.interactingWith);
				Self.interactingWith = null;
			}
		}
		#endregion
	}
}