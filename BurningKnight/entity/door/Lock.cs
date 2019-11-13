using System;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.graphics.animation;
using Lens.util.camera;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.door {
	public class Lock : Entity {
		private bool locked;

		public bool IsLocked => locked;

		public void SetLocked(bool value, Entity entity) {
			if (value == locked || Done) {
				return;
			}

			locked = value;
			
			if (!locked) {
				HandleEvent(new LockOpenedEvent {
					Lock = this,
					Who = entity
				});
				
				GetComponent<StateComponent>().Become<OpeningState>();
				GetComponent<AudioEmitterComponent>().EmitRandomized("unlock");
			} else {
				HandleEvent(new LockClosedEvent {
					Lock = this,
					Who = entity
				});
			}
		}
		
		public bool Move;
		private float t;
		private float shake;
		
		public Lock() {
			Width = 10;
			Height = 20;
			locked = true;
		}
		
		protected virtual bool Interact(Entity entity) {
			if (TryToConsumeKey(entity)) {
				SetLocked(false, entity);
				return true;
			}

			Camera.Instance.Shake(3);
			shake = 1f;
			
			return false;
		}

		protected virtual bool TryToConsumeKey(Entity entity) {
			return false;
		}

		protected virtual bool CanInteract(Entity entity) {
			return true;
		}
		
		public override void AddComponents() {
			base.AddComponents();

			if (Interactable()) {
				AddComponent(new InteractableComponent(Interact) {
					CanInteract = CanInteract
				});
			}
			
			AddComponent(new AudioEmitterComponent());
							
			var state = new StateComponent();
			AddComponent(state);

			state.Become<IdleState>();

			AddTag(Tags.Lock);
		}

		protected virtual AnimationComponent CreateGraphicsComponent() {
			return new AnimationComponent("lock", GetLockPalette());
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (GraphicsComponent == null) {
				// Set here, because of the ui thread
				AddComponent(CreateGraphicsComponent());
			}

			if (!IsLocked) {
				return;
			}
			
			var offset = GetComponent<AnimationComponent>().Offset;

			if (Move) {
				t += dt;
				offset.Y = (float) (Math.Cos(t * 3f) * 1.5f);
			}

			if (shake > 0) {
				shake -= dt;
			} else {
				shake = 0;
			}
							
			offset.X = (float) (Math.Cos(shake * 20f) * shake * 2.5f);
			GetComponent<AnimationComponent>().Offset = offset;
		}

		public override void Render() {
			
		}

		public void RealRender() {
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

		public virtual bool Interactable() {
			return true;
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