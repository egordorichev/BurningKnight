using System.Collections.Generic;
using System.IO;
using Aseprite;
using Lens.graphics;
using Lens.graphics.animation;
using Lens.util;
using Lens.util.file;
using Microsoft.Xna.Framework;

namespace Lens.assets {
	public struct Animations {
		public static bool Reload;
		private static Dictionary<string, AnimationData> animations = new Dictionary<string, AnimationData>();
		
		internal static void Load() {
			var animationDir = FileHandle.FromRoot("Animations/");

			if (animationDir.Exists()) {
				foreach (var animation in animationDir.ListFiles()) {
					LoadAnimation(Path.GetFileNameWithoutExtension(animation), animation);
				}
			}
		}

		private static void LoadAnimation(string name, string fullPath) {
			AsepriteFile file;

			if (Assets.LoadOriginalFiles) {
				file = new AsepriteFile(fullPath);
			} else {
				file = Assets.Content.Load<AsepriteFile>($"bin/Animations/{name}");
			}
			
			var animation = new AnimationData();
			
			for (var i = 0; i < file.Layers.Count; i++) {
				var layer = file.Layers[i];
				var list = new List<AnimationFrame>();
				
				for (var j = 0; j < file.Frames.Count; j++) {
					var frame = file.Frames[j];
					var newFrame = new AnimationFrame();
					
					newFrame.Duration = frame.Duration;
					newFrame.Texture = new TextureRegion(file.Texture, new Rectangle(j * file.Width, i * file.Height, file.Width, file.Height));
					newFrame.Bounds = newFrame.Texture.Source;
					
					list.Add(newFrame);
				}
				
				animation.Layers[layer.Name] = list;
			}
			
			foreach (var tag in file.Animations.Values) {
				var newTag = new AnimationTag();
			
				newTag.Direction = (AnimationDirection) tag.Directions;
				newTag.StartFrame = (uint) tag.FirstFrame;
				newTag.EndFrame = (uint) tag.LastFrame;
				
				animation.Tags[tag.Name] = newTag;
			}
			
			foreach (var tag in file.Tags) {
				var newTag = new AnimationTag();
			
				newTag.Direction = (AnimationDirection) tag.LoopDirection;
				newTag.StartFrame = (uint) tag.From;
				newTag.EndFrame = (uint) tag.To;
				
				animation.Tags[tag.Name] = newTag;
			}

			animation.Texture = file.Texture;
			animations[name] = animation;
		}
		
		internal static void Destroy() {
			foreach (var animation in animations.Values) {
				animation.Texture.Dispose();
			}
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