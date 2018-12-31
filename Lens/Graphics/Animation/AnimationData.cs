using System.Collections.Generic;

namespace Lens.Graphics.Animation {
	public class AnimationData {
		public Dictionary<string, List<AnimationFrame>> Layers = new Dictionary<string, List<AnimationFrame>>();
		public TextureRegion Texture;
				
		public AnimationFrame? GetFrame(string layer, uint id) {			
			List<AnimationFrame> frames;

			if (!Layers.TryGetValue(layer, out frames)) {
				return null;
			}

			if (frames.Count < id) {
				return null;
			}

			return frames[(int) id];
		}

		public Animation CreateAnimation() {
			var animation = new Animation(this);

			animation.StartFrame = 0;
			animation.EndFrame = 1;
			animation.Direction = AnimationDirection.Forward;
			
			return animation;
		}
	}
}