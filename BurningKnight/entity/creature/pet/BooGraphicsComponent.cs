using System;
using BurningKnight.entity.component;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.entity.component.graphics;
using Lens.entity.component.logic;
using Lens.graphics;
using Lens.graphics.animation;
using Lens.util;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.pet {
	public class BooGraphicsComponent : GraphicsComponent {
		private Animation animation;
		private bool flipped;
		private double angle;
		private Vector2 scale = Vector2.One;
		
		public BooGraphicsComponent(string anim) {
			animation = Animations.Get(anim).CreateAnimation();
		}

		public override void Update(float dt) {
			base.Update(dt);
			animation.Update(dt);
		}

		public override void Render(bool shadow) {
			var slice = animation.GetCurrentTexture();
			var origin = slice.Center;
			var body = Entity.GetAnyComponent<BodyComponent>();
			var vx = Math.Abs(body.Velocity.X);
			
			if (vx > 0.1f) {
				flipped = vx < 0;
			}

			angle = MathUtils.LerpAngle(angle, Math.Min(2, vx), Engine.Delta * 8);
			Graphics.Render(slice, Entity.Position + origin, (float) angle, origin, flipped ? new Vector2(-scale.X, scale.Y) : scale);
		}

		public override bool HandleEvent(Event e) {
			if (e is StateChangedEvent ev && animation != null) {
				var tag = ev.NewState.Name.ToLower().Replace("state", "");
				
				if (animation.HasTag(tag)) {
					animation.Tag = tag;
				}
			}

			return base.HandleEvent(e);
		}
	}
}