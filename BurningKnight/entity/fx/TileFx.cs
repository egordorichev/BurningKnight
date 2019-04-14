using System;
using Lens.assets;
using Lens.entity;
using Lens.graphics;

namespace BurningKnight.entity.fx {
	public class TileFx : Entity {
		private TextureRegion region;
		private float t;
		
		public override void Init() {
			base.Init();

			Depth = Layers.WallDecor;
			AlwaysActive = true;

			region = Animations.Get("particles").GetSlice("wall");
		}

		public override void Update(float dt) {
			base.Update(dt);
			t += dt * 4f;

			if (t >= 1f) {
				Done = true;
			}
		}

		public override void Render() {
			Graphics.Render(region, Position);
		}
	}
}