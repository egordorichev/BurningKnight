using System;
using BurningKnight.entity.component;
using BurningKnight.entity.door;
using BurningKnight.entity.events;
using BurningKnight.level;
using BurningKnight.level.entities;
using BurningKnight.physics;
using Lens.entity;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.fx {
	public class SplashParticle : Entity, CollisionFilterEntity {
		public Color Color;
		private float zv;
		
		public override void AddComponents() {
			base.AddComponents();

			AlwaysActive = true;

			Width = 5;
			Height = 5;
			
			AddComponent(new RandomFrameComponent("splash_particle") {
				Tint = new Color(Color.R, Color.G, Color.B, 0.8f)
			});
			
			AddComponent(new ShadowComponent(RenderShadow));
			AddComponent(new ZComponent());

			var body = new CircleBodyComponent(0, 0, 4);
			AddComponent(body);

			var f = Random.Float(60, 90);
			var a = Random.AnglePI();
			
			body.Body.LinearVelocity = new Vector2((float) Math.Cos(a) * f, (float) Math.Sin(a) * f);
			zv = Random.Float(1, 3);
		}

		public override void Update(float dt) {
			base.Update(dt);

			var c = GetComponent<ZComponent>();
			c.Z += zv * dt * 60;
			zv -= dt * 10;

			if (c.Z <= 0) {
				Remove();
			}
		}

		private void RenderShadow() {
			GraphicsComponent.Render(true);
		}

		public bool ShouldCollide(Entity entity) {
			return (entity is Door || entity is SolidProp || entity is Level || entity is DestroyableLevel);
		}

		private void Remove() {
			Done = true;
				
			Area.Add(new SplashFx {
				Position = Center,
				Color = Color
			});
		}

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent ev && ShouldCollide(ev.Entity)) {
				Remove();
			}
			
			return base.HandleEvent(e);
		}
	}
}