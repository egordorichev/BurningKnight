using Lens.assets;
using Lens.entity.component.logic;
using Lens.graphics.animation;
using Lens.util;

namespace Lens.entity.component.graphics {
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
			Animation?.Render(Entity.Position + Offset, Flipped);
		}

		public override bool HandleEvent(Event e) {
			if (e is StateChangedEvent ev && Animation != null) {
				Animation.Tag = ev.NewState.Name.ToLower().Replace("state", "");
			}

			return base.HandleEvent(e);
		}
	}
}