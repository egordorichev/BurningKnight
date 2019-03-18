using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lens.graphics.animation {
	public delegate void AnimationCallback();
	
	public class Animation {
		public AnimationData Data;
		public AnimationCallback OnEnd;
		
		private AnimationFrame frame;
		
		private uint currentFrame;
		public uint StartFrame { get; private set; }
		public uint EndFrame { get; private set; }
		
		public float SpeedModifier = 1f;
		public bool Paused;
		public bool AutoStop;

		public uint Frame {
			get => currentFrame;
			set => currentFrame = value % (EndFrame - StartFrame + 1);
		}
		
		private string layer;
		private string tag;

		public string Layer {
			get => layer;

			set {
				if (layer == value) {
					return;
				}
				
				layer = value;
				ReadFrame();
			}
		}
		
		public string Tag {
			get => tag;

			set {
				if (tag == value) {
					return;
				}

				currentFrame = 0;
				tag = value;
				Paused = false;
				
				ReadFrame();
			}
		}
		
		private float timer;

		public float Timer {
			get => timer;

			set {
				timer = value;

				if (timer >= frame.Duration) {
					timer = 0;

					if (!AutoStop || currentFrame < EndFrame - StartFrame) {
						Frame++;
					
						if (SkipNextFrame) {
							SkipNextFrame = false;
							Frame++;
						}

						if (AutoStop && currentFrame > EndFrame - StartFrame + 1) {
							currentFrame = EndFrame - StartFrame + 1;
							Paused = true;
						}
					
						ReadFrame();
					} else {
						Paused = true;
					}
				}
			}
		}

		public bool PingGoingForward;
		public bool SkipNextFrame;
		
		public Animation(AnimationData data, string layer = null) {
			Data = data;

			if (layer != null) {
				Layer = layer;
			} else {
				ReadFrame();				
			}
		}

		public void Update(float dt) {
			if (!Paused) {
				var frame = currentFrame;
				Timer += dt * SpeedModifier;
				var newFrame = currentFrame;

				if (frame != newFrame && newFrame == 0) {
					OnEnd?.Invoke();	
				}
			}
		}

		public void Render(Vector2 position, bool flipped = false) {
			Graphics.Render(frame.Texture, position, 0, Vector2.Zero, Vector2.One, flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
		}

		public TextureRegion GetCurrentTexture() {
			return frame.Texture;
		}
		
		public void Reset() {
			Frame = 0;
		}

		private void ReadFrame() {
			var nullableTag = Data.GetTag(tag);

			if (nullableTag == null) {
				return;
			}

			var currentTag = nullableTag.Value;

			StartFrame = currentTag.StartFrame;
			EndFrame = currentTag.EndFrame;
			
			var frame = Data.GetFrame(layer, currentTag.Direction.GetFrameId(this));

			if (frame != null) {
				this.frame = (AnimationFrame) frame;
			}
		}
	}
}