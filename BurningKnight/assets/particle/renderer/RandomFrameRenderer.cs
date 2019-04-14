using System;
using System.Collections.Generic;
using System.Linq;
using BurningKnight.util;
using Lens.assets;
using Lens.graphics;
using Lens.graphics.animation;
using Microsoft.Xna.Framework;

namespace BurningKnight.assets.particle.renderer {
	public class RandomFrameRenderer : ParticleRenderer {
		private List<AnimationFrame> animation;

		public RandomFrameRenderer(string anim) {
			animation = Animations.Get(anim).Layers.First().Value;
		}

		public override void Render(Particle particle) {
			var region = animation[particle.Rnd % animation.Count].Texture;

			Graphics.Color.A = (byte) MathUtils.Clamp(0, 255, particle.Alpha * 255);
			Graphics.Render(region, particle.Position, particle.Angle, region.Center, new Vector2(particle.Scale));
			Graphics.Color.A = 255;
		}
	}
}