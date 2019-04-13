using System.Collections.Generic;
using System.Linq;
using Lens.assets;
using Lens.util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lens.graphics.animation {
	public class AnimationData {
		public Dictionary<string, List<AnimationFrame>> Layers = new Dictionary<string, List<AnimationFrame>>();
		public Dictionary<string, AnimationTag> Tags = new Dictionary<string, AnimationTag>();
		public Dictionary<string, TextureRegion> Slices = new Dictionary<string, TextureRegion>();
		public Texture2D Texture;

		public AnimationData Recolor(Color[] from, Color[] to) {
			if (from.Length != to.Length) {
				return null;
			}
			
			AnimationData data = new AnimationData();
			
			return data;
		}
		
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
			
			Log.Warning($"Unable to find slice {name}");
			return Textures.Missing;
		}
		
		public AnimationFrame? GetFrame(string layer, uint id) {
			List<AnimationFrame> frames;

			if (layer == null) {
				frames = Layers.FirstOrDefault().Value;
			} else if (!Layers.TryGetValue(layer, out frames)) {
				return null;
			}

			if (frames.Count < id) {
				Log.Warning($"Unable to find frame {layer}:{id}");
				return null;
			}

			return frames[(int) id];
		}

		public Animation CreateAnimation(string layer = null) {
			return new Animation(this, layer);
		}
	}
}