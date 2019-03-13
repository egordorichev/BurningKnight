using System.Collections.Generic;
using System.IO;
using Aseprite;
using Lens.graphics;
using Lens.graphics.animation;
using Lens.util;
using Lens.util.file;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

			for (var i = 0; i < file.Slices.Count; i++) {
				var slice = file.Slices[i];
				animation.Slices[slice.Name] = new TextureRegion(file.Texture, new Rectangle(slice.OriginX, slice.OriginY, slice.Width, slice.Height));
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
			if (animations.TryGetValue(id, out var animation)) {
				return animation;
			}
			
			Log.Error($"Animation {id} was not found!");
			return null;
		}

		public static Animation Create(string id, string layer = null) {
			return new Animation(Get(id), layer);
		}

		public static AnimationData GetColored(string id, ColorSet set) {
			var fullId = $"{id}_{set.Id}";
			
			if (animations.TryGetValue(fullId, out var animation)) {
				return animation;
			}
			
			animation = Get(id);

			if (animation == null) {
				return null;
			}
			
			var data = new AnimationData();
			var w = animation.Texture.Width;
			var h = animation.Texture.Height;
			var texture = new Texture2D(Engine.GraphicsDevice, w, h);
			var tdata = new Color[w * h];
			
			animation.Texture.GetData(tdata);
			var pixelData = new Color[w * h];
			
			for (int y = 0; y < h; y++) {
				for (int x = 0; x < w; x++) {
					var i = x + y * w;
					var color = tdata[i];

					for (int c = 0; c < set.From.Length; c++) {
						if (ColorUtils.Compare(set.From[c], color, 4)) {
							color = set.To[c];
						}
					}
					
					pixelData[i] = color;
				}
			}
			
			texture.SetData(pixelData);

			foreach (var l in animation.Layers) {
				var list = new List<AnimationFrame>();
				
				foreach (var f in l.Value) {
					list.Add(new AnimationFrame {
						Texture = new TextureRegion(texture, f.Bounds),
						Duration = f.Duration,
						Bounds = f.Bounds
					});	
				}

				data.Layers[l.Key] = list;
			}

			foreach (var s in animation.Slices) {
				data.Slices[s.Key] = new TextureRegion(texture, s.Value.Source);
			}

			foreach (var t in animation.Tags) {
				data.Tags[t.Key] = t.Value;
			}
			
			data.Texture = texture;
			animations[fullId] = data;
			
			return data;
		}
		
		public static Animation CreateColored(string id, ColorSet set, string layer = null) {
			return new Animation(GetColored(id, set), layer);
		}
	}
}