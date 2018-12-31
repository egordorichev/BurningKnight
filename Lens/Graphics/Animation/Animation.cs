using Lens.Util;
using Microsoft.Xna.Framework;

namespace Lens.Graphics.Animation {
	public class Animation {
		public AnimationData Data;
		public string layer = "Layer 1";
		
		private AnimationFrame frame;
		private uint currentFrame;
		public uint StartFrame;
		public uint EndFrame;

		
		public uint Frame {
			get { return currentFrame; }

			set {
				currentFrame = value % (EndFrame - StartFrame + 1);
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
					ReadFrame();
				}
			}
		}

		public AnimationDirection Direction;
		
		public Animation(AnimationData data) {
			Data = data;
			ReadFrame();
		}

		public void Update(float dt) {
			Timer += dt;
		}

		public void Render(Vector2 position) {
			frame.Texture.Source = new Rectangle(0, 0, 32, 16);
			frame.Bounds = frame.Texture.Source;
			Gr.Render(frame.Texture, position);
		}
		
		public void Reset() {
			Frame = 0;
		}

		private void ReadFrame() {
			var frame = Data.GetFrame(layer, Direction.GetFrameId(this));

			if (frame != null) {
				this.frame = (AnimationFrame) frame;
			}
		}
	}
}