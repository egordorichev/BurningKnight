using System;
using BurningKnight.assets.lighting;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.events;
using BurningKnight.physics;
using Lens.entity;
using Lens.input;
using Lens.util;
using Lens.util.camera;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity {
	public delegate void BombUpdateCallback(Bomb b, float dt);
	public delegate void BombDeathCallback(Bomb b);
	
	public class Bomb : Entity, CollisionFilterEntity {
		public const float ExplosionTime = 3f;
		private readonly float explosionTime; 

		public BombUpdateCallback Controller;
		public BombDeathCallback OnDeath;

		public Bomb Parent;
		public Entity Owner;

		public float Scale;
		public float T;
		
		public Bomb(Entity owner, float time = ExplosionTime, Bomb parent = null) {
			explosionTime = time + Rnd.Float(-0.1f, 1f);
			
			Parent = parent;
			Owner = owner;

			Scale = parent?.Scale * 0.7f ?? 1;
		}
		
		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new BombGraphicsComponent("items", "bomb"));
			AddComponent(new ShadowComponent(RenderShadow));
			AddComponent(new ExplodableComponent());
			AddComponent(new RoomComponent());
			
			AddComponent(new LightComponent(this, 32f * Scale, new Color(1f, 0.3f, 0.3f, 1f)));
			
			Width = 10 * Scale;
			Height = 13 * Scale;
			AlwaysActive = true;
			
			AddComponent(new RectBodyComponent(0, 0, Width, Height));
			AddComponent(new ExplodeComponent {
				Radius = 32,
				Timer = explosionTime
			});
			
			AddComponent(new AudioEmitterComponent());
			GetComponent<AudioEmitterComponent>().EmitRandomized("bomb_placed");
		}

		private void RenderShadow() {
			GraphicsComponent.Render(true);
		}

		public void MoveToMouse() {
			VelocityTo(AngleTo(Input.Mouse.GamePosition));
		}

		public void VelocityTo(float angle, float force = 100f) {
			var component = GetComponent<RectBodyComponent>();
			var vec = new Vector2((float) Math.Cos(angle) * force, (float) Math.Sin(angle) * force);
			
			Position += vec * 0.05f;
			
			component.Body.LinearDamping = 5;
			component.Velocity = vec;
		}

		public bool ShouldCollide(Entity entity) {
			return !(entity is Mob);
		}

		private bool sent;

		public override void Update(float dt) {
			base.Update(dt);

			T += dt;

			if (!sent) {
				sent = true;
				
				// Not placed in init, so that room component had a chance to guess the room				
				Owner?.HandleEvent(new BombPlacedEvent {
					Bomb = this,
					Owner = Owner
				});
			}
			
			Controller?.Invoke(this, dt);
		}
	}
}