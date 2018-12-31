using System.Collections.Generic;
using Aseprite;
using Lens.Graphics;
using Lens.Graphics.Animation;
using Lens.Util;

namespace Lens.Asset {
	public struct Animations {
		private static Dictionary<string, AnimationData> animations = new Dictionary<string, AnimationData>();
		
		internal static void Load() {
			var file = Assets.Content.Load<AsepriteFile>("bin/test");
			var animation = new AnimationData();
						
			foreach (var layer in file.Layers) {
				var list = new List<AnimationFrame>();
				
				Log.Error("'" + layer.Name + "'");
				
				foreach (var frame in file.Frames) {
					var newFrame = new AnimationFrame();
					
					newFrame.Duration = frame.Duration;
					newFrame.Texture = new TextureRegion(file.Texture);
					
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