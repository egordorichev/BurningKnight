using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.assets.lighting {
	public abstract class Light {
		private bool dirty = true;
		private bool locked;
		
		protected float radius = 32;

		public float Radius {
			get => radius;

			set {
				if (!locked) {
					radius = value;
					dirty = true;
				}
			}
		}

		public Color Color;
		public Vector2 Scale = Vector2.Zero;

		public void Lock() {
			if (locked) {
				return;
			}

			locked = true;
			
			Tween.To(0, Color.A, x => {
				Color.A = (byte) x;
			}, 0.4f).OnEnd = () => Lights.Remove(this, true);
		}

		public void Start(float target) {
			radius = target;

			Tween.To(255, Color.A, x => {
				Color.A = (byte) x; 
			}, 0.4f);
		}
		
		public void Update(float dt) {
			if (dirty) {
				Scale.X = radius / 128f;
				Scale.Y = Scale.X;
			}
		}
		
		public abstract Vector2 GetPosition();
	}
}