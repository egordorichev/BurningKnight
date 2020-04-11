using System.Collections.Generic;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.door;
using BurningKnight.entity.events;
using BurningKnight.level;
using BurningKnight.level.entities;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.util;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.pet {
	public class TheEye : RoomBasedPet {
		private List<Entity> Colliding = new List<Entity>();

		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new AnimationComponent("eye") {
				ShadowOffset = -2
			});
			

			Width = 25;
			Height = 19;
			Depth = Layers.FlyingMob;

			AddComponent(new ShadowComponent());
			AddComponent(new RectBodyComponent(0, 0, Width, Height, BodyType.Dynamic, false));
			
			AddComponent(new StateComponent());
			GetComponent<StateComponent>().Become<IdleState>();
			
			GetComponent<AnimationComponent>().Animate();
			
			var body = GetComponent<BodyComponent>().Body;

			body.Restitution = 1;
			body.LinearDamping = 10;
			Flying = true;
		}

		protected override void OnJump() {
			base.OnJump();
			GetComponent<StateComponent>().Become<IdleState>();
		}
		
		private float t;
		
		public override void Update(float dt) {
			base.Update(dt);
			t += dt;

			if (t >= 0.2f) {
				t = 0;

				if (GetComponent<StateComponent>().StateInstance is AttackState) {
					foreach (var c in Colliding) {
						c.GetComponent<HealthComponent>().ModifyHealth(-3, this);
					}
				}
			}
		}

		public override bool ShouldCollide(Entity entity) {
			if (entity is Chasm || entity is HalfWall || entity is SolidProp) {
				return false;
			}

			if ((entity is Door || entity is Level) && GetComponent<StateComponent>().StateInstance is IdleState) {
				return false;
			}
			
			return base.ShouldCollide(entity);
		}

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent cse) {
				if (cse.Entity is Level) {
					if (GetComponent<StateComponent>().StateInstance is AttackState) {
						Become<IdleState>();
					}
				} else if (cse.Entity.HasTag(Tags.MustBeKilled)) {
					Colliding.Add(cse.Entity);

					if (GetComponent<StateComponent>().StateInstance is AttackState) {
						cse.Entity.GetComponent<HealthComponent>().ModifyHealth(-3, this);
					}
				}
			} else if (e is CollisionEndedEvent cee) {
				if (cee.Entity.HasTag(Tags.MustBeKilled)) {
					Colliding.Remove(cee.Entity);
				}
			}
			
			return base.HandleEvent(e);
		}

		private Entity target;
		
		#region Eye States
		private class IdleState : SmartState<TheEye> {
			public override void Init() {
				base.Init();
				Self.Owner.GetComponent<FollowerComponent>().AddFollower(Self);
			}

			public override void Destroy() {
				base.Destroy();
				Self.GetComponent<FollowerComponent>().Remove();
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (T <= 1f) {
					return;
				}
				
				var r = Self.Owner.GetComponent<RoomComponent>().Room;

				if (r == null || r.Tagged[Tags.MustBeKilled].Count == 0) {
					return;
				}

				Self.target = r.FindClosest(Self.Center, Tags.MustBeKilled);

				if (Self.target != null) {
					Become<AttackState>();
				}
			}
		}
		
		private class AttackState : SmartState<TheEye> {
			public override void Init() {
				base.Init();
				Self.GetComponent<RectBodyComponent>().Velocity = MathUtils.CreateVector(Self.AngleTo(Self.target), 600);
			}

			public override void Destroy() {
				base.Destroy();
				Self.GetComponent<RectBodyComponent>().Velocity = Vector2.Zero;
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (Self.GetComponent<RectBodyComponent>().Velocity.LengthSquared() <= 2048) {
					Become<IdleState>();
				}
			}
		}
		#endregion
	}
}