using BurningKnight.assets.lighting;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.entities.decor {
	public class WallTorch : SlicedProp {
		public bool On = true;
		public float XSpread = 0.5f;
		public Vector2? Target;
		
		private FireEmitter emitter;
		
		public override void Init() {
			base.Init();

			Width = 5;
			Height = 7;
			Sprite = "wall_torch";
		}

		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new LightComponent(this, 64f, new Color(1f, 0.8f, 0.2f, 1f)));
		}

		public override void Destroy() {
			base.Destroy();
			emitter.Done = true;
		}

		public override void PostInit() {
			base.PostInit();

			Area.Add(emitter = new FireEmitter {
				Depth = Layers.Wall + 1,
				Position = new Vector2(CenterX, Y + 1),
				Scale = 0.5f
			});
		}
		
		private float lastFlame;

		public override void Update(float dt) {
			base.Update(dt);

			if (!On) {
				return;
			}
			
			lastFlame += dt;

			if (lastFlame > 0.1f) {
				Area.Add(new FireParticle {
					X = CenterX,
					Y = Y + 1,
					Target = Target,
					Depth = Layers.Wall + 1,
					XChange = XSpread
				});

				lastFlame = 0;
			}
		}
	}
}