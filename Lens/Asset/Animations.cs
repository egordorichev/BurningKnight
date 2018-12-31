using System.Collections.Generic;
using Aseprite;
using Lens.Graphics;
using Lens.Graphics.Animation;
using Lens.Util;
using Microsoft.Xna.Framework;

namespace Lens.Asset {
	public struct Animations {
		private static Dictionary<string, AnimationData> animations = new Dictionary<string, AnimationData>();
		
		internal static void Load() {
			var file = Assets.Content.Load<AsepriteFile>("bin/test");
			var animation = new AnimationData();
						
			for (var i = 0; i < file.Layers.Count; i++) {
				var layer = file.Layers[i];
				var list = new List<AnimationFrame>();
				
				for (var j = 0; j < file.Frames.Count; j++) {
					var frame = file.Frames[j];
					var newFrame = new AnimationFrame();
					
					newFrame.Duration = frame.Duration;
					newFrame.Texture = new TextureRegion(file.Texture, new Rectangle(j * file.Width, i * file.Height, file.Width, file.Height));
					
					list.Add(newFrame);
				}
				
				animation.Layers[layer.Name] = list;
			}

			animations["test"] = animation;
		}
		
		internal static void Destroy() {
			
		}

		public static AnimationData Get(string id) {
			AnimationData animation;

			if (animations.TryGetValue(id, out animation)) {
				return animation;
			}
			
			Log.Error($"Animation {id} was not found!");
			return null;
		}
	}
}