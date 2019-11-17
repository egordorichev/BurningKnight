using BurningKnight.assets.lighting;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity.component;
using BurningKnight.save;
using BurningKnight.ui.editor;
using Lens.util.math;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.level.entities {
	public class Campfire : Prop {
		private float lastFlame;
		
		public override void AddComponents() {
			base.AddComponents();

			Width = 17;
			Height = 11;
			
			AddComponent(new SliceComponent("props", "campfire"));
			AddComponent(new LightComponent(this, 32, new Color(1f, 1f, 0f, 1f)));
			AddComponent(new RectBodyComponent(Width / 4, 0, Width / 2, Height - 5, BodyType.Static));
		}

		public override void Update(float dt) {
			base.Update(dt);
			lastFlame += dt;

			if (lastFlame > 0.1f) {
				lastFlame = 0;

				Area.Add(new FireParticle {
					X = CenterX + Rnd.Float(-4, 4),
					Y = CenterY
				});
			}
		}
	}
}