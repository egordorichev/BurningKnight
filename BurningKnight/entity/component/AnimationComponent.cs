using System;
using BurningKnight.assets;
using BurningKnight.entity.buff;
using BurningKnight.entity.creature.player;
using BurningKnight.util;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.entity.component.graphics;
using Lens.entity.component.logic;
using Lens.graphics;
using Lens.graphics.animation;
using Lens.util;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.component {
	public class AnimationComponent : GraphicsComponent {
		public Animation Animation;
		public Color Tint = Color.White;
		private string name;
		private ColorSet set;

		public float ShadowOffset;
		public Vector2 Scale = Vector2.One;
		public float Angle;
		public float OriginY;
		public float OriginX;
		public bool AddOrigin = true;
		public bool Centered;

		public bool Flash;
		
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

				OriginX = Animation.GetCurrentTexture().Width / 2f;
				OriginY = Animation.GetCurrentTexture().Height;
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
			var pos = (Centered ? Entity.Center : Entity.Position) + Offset;

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

			var stopShader = !shadow && StartShaders();

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
		
		protected bool StartShaders() {
			if (Flash || (Entity.TryGetComponent<HealthComponent>(out var health) && health.RenderInvt && (health.InvincibilityTimer > health.InvincibilityTimerMax / 2f || health.InvincibilityTimer % 0.1f > 0.05f))) {
				var shader = Shaders.Entity;
				Shaders.Begin(shader);

				shader.Parameters["flash"].SetValue(1f);
				shader.Parameters["flashReplace"].SetValue(1f);
				shader.Parameters["flashColor"].SetValue(ColorUtils.White);

				return true;
			}

			if (Entity.TryGetComponent<BuffsComponent>(out var buffs)) {
				if (buffs.Has<InvincibleBuff>()) {
					var shader = Shaders.Entity;
					Shaders.Begin(shader);

					var t = buffs.Buffs[typeof(InvincibleBuff)].TimeLeft;

					if (t < 2f && t % 0.3f < 0.15f) {
						return false;
					}
					
					shader.Parameters["flash"].SetValue(1f);
					shader.Parameters["flashReplace"].SetValue(1f);
					shader.Parameters["flashColor"].SetValue(ColorUtils.FromHSV(Engine.Time * 180 % 360, 100, 100).ToVector4());

					return true;
				}
				
				if (buffs.Has<FrozenBuff>()) {
					var shader = Shaders.Entity;
					Shaders.Begin(shader);

					shader.Parameters["flash"].SetValue(1f);
					shader.Parameters["flashReplace"].SetValue(0f);
					shader.Parameters["flashColor"].SetValue(FrozenBuff.Color);

					return true;
				}

				if (buffs.Has<BurningBuff>()) {
					var shader = Shaders.Entity;
					Shaders.Begin(shader);

					shader.Parameters["flash"].SetValue(1f);
					shader.Parameters["flashReplace"].SetValue(0f);
					shader.Parameters["flashColor"].SetValue(BurningBuff.Color);

					return true;
				}

				if (buffs.Has<PoisonBuff>()) {
					var shader = Shaders.Entity;
					Shaders.Begin(shader);

					shader.Parameters["flash"].SetValue(1f);
					shader.Parameters["flashReplace"].SetValue(0f);
					shader.Parameters["flashColor"].SetValue(PoisonBuff.Color);

					return true;
				}

				if (buffs.Has<SlowBuff>()) {
					var shader = Shaders.Entity;
					Shaders.Begin(shader);

					shader.Parameters["flash"].SetValue(1f);
					shader.Parameters["flashReplace"].SetValue(0f);
					shader.Parameters["flashColor"].SetValue(SlowBuff.Color);

					return true;
				}

				if (buffs.Has<CharmedBuff>()) {
					var shader = Shaders.Entity;
					Shaders.Begin(shader);

					shader.Parameters["flash"].SetValue(1f);
					shader.Parameters["flashReplace"].SetValue(0f);
					shader.Parameters["flashColor"].SetValue(CharmedBuff.Color);

					return true;
				}
			}

			return false;
		}
		
		protected virtual void CallRender(Vector2 pos, bool shadow) {
			if (Animation == null) {
				return;
			}
			
			var region = Animation.GetCurrentTexture();
			var or = new Vector2(OriginX, shadow ? region.Height - OriginY : OriginY);
			
			Graphics.Render(region, AddOrigin ? pos + or : pos, shadow ^ Flipped ? -Angle : Angle, or, Scale, Graphics.ParseEffect(Flipped, FlippedVerticaly));
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

		public void Animate(Action callback = null) {
			Tween.To(1.8f, Scale.X, x => Scale.X = x, 0.1f);
			Tween.To(0.2f, Scale.Y, x => Scale.Y = x, 0.1f).OnEnd = () => {
				Tween.To(1, Scale.X, x => Scale.X = x, 0.4f);
				Tween.To(1, Scale.Y, x => Scale.Y = x, 0.4f);

				callback?.Invoke();
			};
		}
	}
}