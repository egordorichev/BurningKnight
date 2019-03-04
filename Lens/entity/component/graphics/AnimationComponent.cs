using Lens.assets;
using Lens.entity.component.logic;
using Lens.graphics.animation;
using Lens.util;

namespace Lens.entity.component.graphics {
	public class AnimationComponent : GraphicsComponent {
		public Animation Animation;
		private string name;
		
		public AnimationComponent(string animationName, string layer = null) {
			name = animationName;
			ReloadAnimation(layer);
		}

		private void ReloadAnimation(string layer = null) {
			var data = Animations.Get(name);

			if (data != null) {
				Animation = data.CreateAnimation(layer);
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

		public override void HandleEvent(Event e) {
			base.HandleEvent(e);

			if (e is StateChangedEvent ev && Animation != null) {
				Animation.Tag = ev.NewState.Name.ToLower().Replace("state", "");
			}
		}
	}
}