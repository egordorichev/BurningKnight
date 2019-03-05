using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace Lens.graphics.animation {
	public class AnimationData {
		public Dictionary<string, List<AnimationFrame>> Layers = new Dictionary<string, List<AnimationFrame>>();
		public Dictionary<string, AnimationTag> Tags = new Dictionary<string, AnimationTag>();
		public Dictionary<string, TextureRegion> Slices = new Dictionary<string, TextureRegion>();
		public Texture2D Texture;
		
		public AnimationTag? GetTag(string tagName) {
			AnimationTag tag;

			if (tagName == null) {
				tag = Tags.FirstOrDefault().Value;
			} else if (!Tags.TryGetValue(tagName, out tag)) {
				return null;
			}

			return tag;
		}

		public TextureRegion GetSlice(string name) {
			if (Slices.TryGetValue(name, out var region)) {
				return region;
			}

			return null;
		}
		
		public AnimationFrame? GetFrame(string layer, uint id) {
			List<AnimationFrame> frames;

			if (layer == null) {
				frames = Layers.FirstOrDefault().Value;
			} else if (!Layers.TryGetValue(layer, out frames)) {
				return null;
			}

			if (frames.Count < id) {
				return null;
			}

			return frames[(int) id];
		}

		public Animation CreateAnimation(string layer = null) {
			return new Animation(this, layer);
		}
	}
}