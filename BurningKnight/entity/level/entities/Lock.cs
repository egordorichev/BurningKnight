using BurningKnight.entity.component;
using Lens.entity;
using Lens.entity.component.graphics;
using Lens.entity.component.logic;
using Lens.graphics.animation;

namespace BurningKnight.entity.level.entities {
	public class Lock : Entity {
		public bool IsLocked { get; protected set; }
		
		public Lock() {
			Width = 10;
			Height = 15;
			IsLocked = true;
		}
		
		protected virtual bool Interact(Entity entity) {
			if (TryToConsumeKey(entity)) {
				GetComponent<StateComponent>().Become<OpeningState>();
				IsLocked = false;
				
				return true;
			}

			return false;
		}

		protected virtual bool TryToConsumeKey(Entity entity) {
			return false;
		}
		
		public override void AddComponents() {
			base.AddComponents();

			AddComponent(new InteractableComponent(Interact));			
			SetGraphicsComponent(new AnimationComponent("lock", GetLockPalette()));
			
			var state = new StateComponent();
			AddComponent(state);

			state.Become<IdleState>();
		}

		public override void Render() {
			if (!(GetComponent<StateComponent>().StateInstance is OpenState)) {
				base.Render();
			}
		}

		protected virtual bool Disposable() {
			return true;
		}

		protected virtual ColorSet GetLockPalette() {
			return null;
		}
		
		#region Lock States
		public class IdleState : EntityState {
			
		}
		
		public class OpenState : EntityState {
			
		}
		
		public class OpeningState : EntityState {
			public override void Init() {
				base.Init();
				Self.GetComponent<AnimationComponent>().SetAutoStop(true);
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (Self.GetComponent<AnimationComponent>().Animation.Paused) {
					if (((Lock) Self).Disposable()) {
						Self.Done = true;
					} else {
						Become<OpenState>();
					}
				}
			}

			public override void Destroy() {
				base.Destroy();
				Self.GetComponent<AnimationComponent>().SetAutoStop(false);
			}
		}

		public class ClosingState : EntityState {
			public override void Init() {
				base.Init();
				Self.GetComponent<AnimationComponent>().SetAutoStop(true);
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (Self.GetComponent<AnimationComponent>().Animation.Paused) {
					Become<IdleState>();
				}
			}

			public override void Destroy() {
				base.Destroy();
				Self.GetComponent<AnimationComponent>().SetAutoStop(false);
			}
		}
		#endregion
	}
}