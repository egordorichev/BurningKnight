using Microsoft.Xna.Framework;

namespace Lens.Graphics.Animation {
	public class Animation {
		public AnimationData Data;
		
		private AnimationFrame frame;
		private uint currentFrame;
		public uint StartFrame { get; private set; }
		public uint EndFrame { get; private set; }

		public uint Frame {
			get { return currentFrame; }

			set {
				currentFrame = value % (EndFrame - StartFrame + 1);
			}
		}
		
		private string layer;
		private string tag;

		public string Layer {
			get { return layer; }
			
			set {
				layer = value;
				ReadFrame();
			}
		}
		
		public string Tag {
			get { return tag; }
			
			set {
				tag = value;
				ReadFrame();
			}
		}
		
		private float timer;

		public float Timer {
			get { return timer; }

			set {
				timer = value;

				if (timer >= frame.Duration) {
					timer = 0;
					Frame++;

					if (SkipNextFrame) {
						SkipNextFrame = false;
						Frame++;
					}
					
					ReadFrame();
				}
			}
		}

		public bool PingGoingForward;
		public bool SkipNextFrame;
		
		public Animation(AnimationData data) {
			Data = data;
			ReadFrame();
		}

		public void Update(float dt) {
			Timer += dt;
		}

		public void Render(Vector2 position) {
			Gr.Render(frame.Texture, position);
		}
		
		public void Reset() {
			Frame = 0;
		}

		private void ReadFrame() {
			var nullableTag = Data.GetTag(this.tag);

			if (nullableTag == null) {
				return;
			}

			var tag = nullableTag.Value;

			StartFrame = tag.StartFrame;
			EndFrame = tag.EndFrame;
			
			var frame = Data.GetFrame(layer, tag.Direction.GetFrameId(this));

			if (frame != null) {
				this.frame = (AnimationFrame) frame;
			}
		}
	}
}