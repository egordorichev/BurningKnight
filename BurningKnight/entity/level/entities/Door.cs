using System.Collections.Generic;
using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.events;
using BurningKnight.save;
using Lens.entity;
using Lens.entity.component.graphics;
using Lens.entity.component.logic;
using Lens.util.file;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.level.entities {
	public class Door : SaveableEntity {
		private const float W = 16;
		private const float H = 8;
		private const float CloseTimer = 1f;
		
		public bool FacingSide;

		private List<Entity> colliding = new List<Entity>();
		private float lastCollisionTimer;
		
		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new StateComponent());
			GetComponent<StateComponent>().Become<ClosedState>();
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			FacingSide = stream.ReadBoolean();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteBoolean(FacingSide);
		}

		public override void PostInit() {
			base.PostInit();

			SetGraphicsComponent(new AnimationComponent(FacingSide ? "side_door" : "regular_door") {
				Offset = new Vector2(-2, 0)
			});
			
			AddComponent(new RectBodyComponent(0, 0, FacingSide ? H : W, FacingSide ? W : H, BodyType.Static, true));
		}

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent start) {
				if (start.Entity is Creature) {
					colliding.Add(start.Entity);

					if (colliding.Count == 1) {
						GetComponent<StateComponent>().Become<OpeningState>();	
					}
				}
			} else if (e is CollisionEndedEvent end) {
				if (end.Entity is Creature) {
					colliding.Remove(end.Entity);
					
					if (colliding.Count == 0) {
						lastCollisionTimer = CloseTimer;
					}
				}
			}
			
			return base.HandleEvent(e);
		}

		public override void Update(float dt) {
			base.Update(dt);
			var state = GetComponent<StateComponent>();
			
			if (state.StateInstance is OpenState && colliding.Count == 0) {
				lastCollisionTimer -= dt;

				if (lastCollisionTimer <= 0) {
					state.Become<ClosingState>();
				}
			}
		}

		public class ClosedState : EntityState {
			
		}

		public class ClosingState : EntityState {
			public override void Update(float dt) {
				base.Update(dt);

				if (T >= 0.05f && Self.GetComponent<AnimationComponent>().Animation.Frame == 0) {
					Self.GetComponent<StateComponent>().Become<ClosedState>();
				}
			}
		}

		public class OpenState : EntityState {
			
		}

		public class OpeningState : EntityState {
			public override void Update(float dt) {
				base.Update(dt);
				
				if (T >= 0.05f && Self.GetComponent<AnimationComponent>().Animation.Frame == 0) {
					Self.GetComponent<StateComponent>().Become<OpenState>();
				}
			}
		}
	}
}