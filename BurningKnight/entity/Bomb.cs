using System;
using BurningKnight.entity.component;
using Lens.entity;
using Lens.input;
using Lens.util;
using Lens.util.camera;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity {
	public class Bomb : Entity {
		public const float ExplosionTime = 3f;

		private readonly float explosionTime; 

		public Bomb(float time = ExplosionTime) {
			explosionTime = time;
		}
		
		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new BombGraphicsComponent("items", "bomb"));

			Width = 10;
			Height = 13;
			AlwaysActive = true;
			
			AddComponent(new RectBodyComponent(0, 0, Width, Height));
			AddComponent(new ExplodeComponent {
				Radius = 32,
				Timer =  explosionTime
			});
		}

		public void MoveToMouse() {
			var component = GetComponent<RectBodyComponent>();
			var angle = AngleTo(Input.Mouse.GamePosition);
			var force = 100f;
			var vec = new Vector2((float) Math.Cos(angle) * force, (float) Math.Sin(angle) * force);

			Position += vec * 0.05f;
			
			component.Body.LinearDamping = 5;
			component.Velocity = vec;
		}
	}
}