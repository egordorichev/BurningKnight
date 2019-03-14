using BurningKnight.assets;
using BurningKnight.util;
using Lens.assets;
using Lens.entity;
using Lens.entity.component.graphics;
using Lens.entity.component.logic;
using Lens.graphics;
using Lens.graphics.animation;
using MonoGame.Extended.Sprites;

namespace BurningKnight.entity.component {
	public class AnimationComponent : GraphicsComponent {
		public Animation Animation;
		private string name;
		private ColorSet set;
		
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

		public override void Render() {
			var pos = Entity.Position + Offset;
			
			if (Entity.TryGetComponent<InteractableComponent>(out var component) && component.OutlineAlpha > 0.05f) {
				var shader = Shaders.Entity;
				Shaders.Begin(shader);

				shader.Parameters["flash"].SetValue(component.OutlineAlpha);
				shader.Parameters["flashReplace"].SetValue(1f);
				shader.Parameters["flashColor"].SetValue(ColorUtils.White);

				foreach (var d in MathUtils.EntityDirections) {
					Animation?.Render(pos + d, Flipped);
				}
				
				Shaders.End();
			}
			
			Animation?.Render(pos, Flipped);
		}

		public override bool HandleEvent(Event e) {
			if (e is StateChangedEvent ev && Animation != null) {
				Animation.Tag = ev.NewState.Name.ToLower().Replace("state", "");
			}

			return base.HandleEvent(e);
		}
	}
}