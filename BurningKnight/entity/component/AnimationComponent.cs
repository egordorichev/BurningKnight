using BurningKnight.assets;
using BurningKnight.util;
using Lens.assets;
using Lens.entity;
using Lens.entity.component.graphics;
using Lens.entity.component.logic;
using Lens.graphics;
using Lens.graphics.animation;
using Lens.util;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.component {
	public class AnimationComponent : GraphicsComponent {
		public Animation Animation;
		public Color Tint = Color.White;
		private string name;
		private ColorSet set;

		public float ShadowOffset;
		
		public AnimationComponent(string animationName, string layer = null, string tag = null) {
			name = animationName;
			ReloadAnimation(layer, tag);
		}
		
		public AnimationComponent(string animationName, ColorSet set) {
			name = animationName;
			this.set = set;
			
			ReloadAnimation();
		}

		public void SetAutoStop(bool stop) {
			Animation.AutoStop = stop;
		}
		
		private void ReloadAnimation(string layer = null, string tag = null) {
			var data = set == null ? Animations.Get(name) : Animations.GetColored(name, set);

			if (data != null) {
				Animation = data.CreateAnimation(layer);

				if (tag != null) {
					Animation.Tag = tag;
				}
			}			
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Animations.Reload) {
				ReloadAnimation();
			}
			
			Animation?.Update(dt);
		}

		public override void Render(bool shadow) {
			var pos = Entity.Position + Offset;

			if (shadow) {
				FlippedVerticaly = !FlippedVerticaly;
				pos.Y += Animation.GetCurrentTexture().Height - ShadowOffset * 2;
			}
			
			if (Entity.TryGetComponent<InteractableComponent>(out var component) && component.OutlineAlpha > 0.05f) {
				var shader = Shaders.Entity;
				Shaders.Begin(shader);

				shader.Parameters["flash"].SetValue(component.OutlineAlpha);
				shader.Parameters["flashReplace"].SetValue(1f);
				shader.Parameters["flashColor"].SetValue(ColorUtils.White);

				foreach (var d in MathUtils.Directions) {
					CallRender(pos + d, shadow);
				}
				
				Shaders.End();
			}

			var stopShader = false;

			if (Entity.TryGetComponent<HealthComponent>(out var health) && health.RenderInvt) {
				var i = health.InvincibilityTimer;

				if (i > health.InvincibilityTimerMax / 2f || i % 0.1f > 0.05f) {
					var shader = Shaders.Entity;
					Shaders.Begin(shader);

					shader.Parameters["flash"].SetValue(1f);
					shader.Parameters["flashReplace"].SetValue(1f);
					shader.Parameters["flashColor"].SetValue(ColorUtils.White);
					
					stopShader = true;
				}
			}

			Graphics.Color = Tint;
			CallRender(pos, shadow);
			Graphics.Color = ColorUtils.WhiteColor;

			if (stopShader) {
				Shaders.End();
			}

			if (shadow) {
				FlippedVerticaly = !FlippedVerticaly;
			}
		}
		
		protected virtual void CallRender(Vector2 pos, bool shadow) {
			Animation?.Render(pos, Flipped, FlippedVerticaly);
		}

		public override bool HandleEvent(Event e) {
			if (e is StateChangedEvent ev && Animation != null) {
				var tag = ev.NewState.Name.ToLower().Replace("state", "");
				
				if (Animation.HasTag(tag)) {
					Animation.Tag = tag;
				}
			}

			return base.HandleEvent(e);
		}
	}
}