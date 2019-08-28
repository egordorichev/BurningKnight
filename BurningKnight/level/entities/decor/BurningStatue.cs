using BurningKnight.assets.lighting;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity.component;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.entities.decor {
	public class BurningStatue : SolidProp {
		public override void Init() {
			base.Init();

			Width = 14;
			Height = 28;
			Sprite = "statue_a";
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
				Position = new Vector2(X + 11, Y + 15),
				Scale = 0.8f
			});
			
			Area.Add(new FireEmitter {
				Depth = Depth + 1,
				Position = new Vector2(X + 4, Y + 15),
				Scale = 0.8f
			});
		}

		private float lastFlame;

		public override void Update(float dt) {
			base.Update(dt);
			lastFlame += dt;

			if (lastFlame > 0.3f) {
				Area.Add(new FireParticle {
					X = X + 11,
					Y = Y + 15,
					XChange = 0.1f
				});
				
				Area.Add(new FireParticle {
					X = X + 4,
					Y = Y + 15,
					XChange = 0.1f
				});

				lastFlame = 0;
			}
		}

		protected override Rectangle GetCollider() {
			return new Rectangle(2, 8, 10, 14);
		}
	}
}