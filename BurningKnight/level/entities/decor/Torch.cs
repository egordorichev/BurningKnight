using BurningKnight.assets;
using BurningKnight.assets.lighting;
using BurningKnight.assets.particle;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity.component;
using BurningKnight.util;
using Lens.graphics;
using Lens.util.file;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.entities.decor {
	public class Torch : SolidProp {
		public bool On = true;
		public float XSpread = 1f;
		public Vector2? Target;
		
		private bool broken;
		private FireEmitter emitter;
		
		public override void Init() {
			base.Init();

			Width = 10;
			Sprite = "torch";
		}

		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new LightComponent(this, 64f, new Color(1f, 0.8f, 0.3f, 1f)));
			AddComponent(new ShadowComponent());
			AddComponent(new RoomComponent());
			
			AddTag(Tags.Torch);
		}

		public override void Destroy() {
			base.Destroy();

			if (emitter != null) {
				emitter.Done = true;
			}
		}

		public override void PostInit() {
			base.PostInit();

			if (!broken) {
				Area.Add(emitter = new FireEmitter {
					Depth = Depth + 1,
					Position = new Vector2(CenterX, Y + 3),
					Scale = 0.5f
				});
			}

			UpdateSprite();
		}

		private void UpdateSprite() {
			if (broken) {
				var s = GetComponent<SliceComponent>();

				s.Sprite = CommonAse.Props.GetSlice("broken_torch");
				s.Offset = new Vector2(0, 5);
			}
		}

		public void Break() {
			if (broken) {
				return;
			}

			On = false;
			broken = true;

			if (emitter != null) {
				emitter.Done = true;
			}
			
			AnimationUtil.Poof(Center);
			Particles.BreakSprite(Area, GetComponent<SliceComponent>().Sprite, Position);
			
			UpdateSprite();
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
					Y = Y + 2,
					Target = Target,
					XChange = XSpread
				});

				lastFlame = 0;
			}
		}

		protected override Rectangle GetCollider() {
			return new Rectangle(2, 5, 6, 5);
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			broken = stream.ReadBoolean();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteBoolean(broken);
		}
	}
}