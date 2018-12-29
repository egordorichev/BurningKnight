using System.Collections.Generic;
using Lens.Graphics.Animation;
using Lens.Util;

namespace Lens.Asset {
	public struct Animations {
		private static Dictionary<string, Animation> animations = new Dictionary<string, Animation>();
		
		internal static void Load() {
			
		}
		
		internal static void Destroy() {
			
		}

		public static Animation Get(string id) {
			Animation animation;

			if (animations.TryGetValue(id, out animation)) {
				return animation;
			}
			
			Log.Error($"Animation {id} was not found!");
			return null;
		}
	}
}