using BurningKnight.assets.lighting;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity.component;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.entities.decor {
	public class Torch : SolidProp {
		public override void Init() {
			base.Init();

			Width = 10;
			Sprite = "torch";
		}

		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new LightComponent(this, 32f, new Color(1f, 0.8f, 0.3f, 1f)));
			AddComponent(new ShadowComponent());
		}

		public override void PostInit() {
			base.PostInit();
			
			Area.Add(new FireEmitter {
				Depth = Depth + 1,
				Position = new Vector2(CenterX, Y + 3),
				Scale = 0.5f
			});
		}

		private float lastFlame;

		public override void Update(float dt) {
			base.Update(dt);
			lastFlame += dt;

			if (lastFlame > 0.1f) {
				Area.Add(new FireParticle {
					X = CenterX,
					Y = Y + 2
				});

				lastFlame = 0;
			}
		}

		protected override Rectangle GetCollider() {
			return new Rectangle(2, 5, 6, 5);
		}
	}
}