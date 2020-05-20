using System;
using BurningKnight.assets;
using BurningKnight.entity;
using BurningKnight.entity.component;
using Lens;
using Lens.entity;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.entities {
	public class Claw : Entity {
		private static Vector2 origin = new Vector2(15f / 2, 1);
		private static Vector2 rightOrigin = new Vector2(-0.5f, 1);
		private static Vector2 topOrigin = new Vector2(7 / 2f, 32);

		private TextureRegion leftClaw;
		private TextureRegion rightClaw;
		private TextureRegion top;
		private float angle;
		
		public override void AddComponents() {
			base.AddComponents();

			Width = 15;
			Depth = Layers.FlyingMob;

			var body = new SensorBodyComponent(-origin.X, -origin.Y, 15, 14);
			AddComponent(body);

			body.Body.LinearDamping = 10;

			var animation = CommonAse.Props;

			leftClaw = animation.GetSlice("left_claw");
			rightClaw = animation.GetSlice("right_claw");
			top = animation.GetSlice("claw_hand");
			
		}

		public override void Update(float dt) {
			base.Update(dt);
			angle = (float) Math.Sin(Engine.Time) - 1f;
		}

		public override void Render() {
			Graphics.Render(leftClaw, Position, angle, origin);
			Graphics.Render(rightClaw, Position, -angle, rightOrigin);
			Graphics.Render(top, Position, 0, topOrigin);
		}
	}
}