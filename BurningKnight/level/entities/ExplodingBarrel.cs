using BurningKnight.assets;
using BurningKnight.entity;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using Lens.entity;
using Lens.entity.component.graphics;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.level.entities {
	public class ExplodingBarrel : SolidProp {
		private float tillExplode;
		
		public ExplodingBarrel() {
			Sprite = "exploding_barrel";
		}

		public override void AddComponents() {
			base.AddComponents();
			
			Width = 11;
			Height = 17;
			
			AddComponent(new ShadowComponent());

			var h = new HealthComponent();
			
			AddComponent(h);
			
			h.InitMaxHealth = 3;
			h.RenderInvt = true;
			
			AddComponent(new ExplodableComponent());
		}

		public override void PostInit() {
			base.PostInit();
			var s = GetComponent<ScalableSliceComponent>();

			s.Origin.Y = s.Sprite.Height;
			s.ShadowZ = -6;
		}

		public override bool HandleEvent(Event e) {
			if (e is ExplodedEvent || e is DiedEvent) {
				PrepareToExplode();
				var h = GetComponent<HealthComponent>();
				h.InvincibilityTimer = h.InvincibilityTimerMax;
				return true;
			} else if (e is HealthModifiedEvent hme) {
				hme.Amount = -1;
			}
			
			return base.HandleEvent(e);
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (tillExplode > 0) {
				tillExplode -= dt;

				if (tillExplode <= 0) {
					Explode();
				}
			}
		}

		private void PrepareToExplode() {
			tillExplode = 0.5f;

			var a = GetComponent<ScalableSliceComponent>();
			
			Tween.To(1.4f, a.Scale.X, x => a.Scale.X = x, 0.4f);
			Tween.To(0.7f, a.Scale.Y, x => a.Scale.Y = x, 0.4f);
		}

		protected override GraphicsComponent CreateGraphicsComponent() {
			return new ScalableSliceComponent(CommonAse.Props, Sprite);
		}

		private void Explode() {
			if (Done) {
				return;
			}
			
			Done = true;
			ExplosionMaker.Make(this, 32f);
		}

		protected override Rectangle GetCollider() {
			return new Rectangle(0, 4, 11, 13);
		}
	}
}