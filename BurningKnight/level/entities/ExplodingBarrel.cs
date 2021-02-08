using System;
using BurningKnight.assets;
using BurningKnight.assets.achievements;
using BurningKnight.entity;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.state;
using Lens.entity;
using Lens.entity.component.graphics;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.level.entities {
	public class ExplodingBarrel : SolidProp {
		private float tillExplode;
		private Entity trigger;
		
		public ExplodingBarrel() {
			Sprite = "exploding_barrel";
		}

		protected override BodyComponent CreateBody() {
			var collider = GetCollider();
			var body = new RectBodyComponent(collider.X, collider.Y, collider.Width, collider.Height, BodyType.Static);

			//body.Body.LinearDamping = 6f;
			//body.KnockbackModifier = 0.1f;
			//body.Body.Mass = 0.1f;
			
			return body;
		}

		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new ShadowComponent());

			var h = new HealthComponent();
			
			AddComponent(h);
			AddTag(Tags.Bomb);

			h.InitMaxHealth = 1; // 3;
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
				if (!Done) {
					if (e is ExplodedEvent ee && ee.Who != this) {
						trigger = ee.Who;
					}

					PrepareToExplode();
					var h = GetComponent<HealthComponent>();
					h.InvincibilityTimer = h.InvincibilityTimerMax;
				}

				return true;
			} else if (e is HealthModifiedEvent hme) {
				if (TryGetComponent<RoomComponent>(out var room) && room.Room != null && room.Room.Tagged[Tags.Player].Count == 0) {
					return true;
				}

				hme.Amount = -1;
			}
			
			return base.HandleEvent(e);
		}
		
		private bool added;

		public override void Update(float dt) {
			base.Update(dt);

			if (!added) {
				added = true;
				
				var x = (int) Math.Floor(CenterX / 16);
				var y = (int) Math.Floor(CenterY / 16);

				if (Run.Level.IsInside(x, y)) {
					Run.Level.Passable[Run.Level.ToIndex(x, y)] = false;
				}
			}

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

			if (!HasComponent<AudioEmitterComponent>()) {
				AddComponent(new AudioEmitterComponent());
			}
			
			AudioEmitterComponent.Dummy(Area, Center).EmitRandomized("level_tnt");
		}

		protected override GraphicsComponent CreateGraphicsComponent() {
			return new ScalableSliceComponent(CommonAse.Props, Sprite);
		}

		public void Explode() {
			if (Done) {
				return;
			}
			
			Done = true;
			ExplosionMaker.Make(this, 32f);

			var who = trigger;
			var count = 0;

			while (who != null && who is ExplodingBarrel b) {
				who = b.trigger;
				count++;
			}

			if (count >= 2) {
				Achievements.Unlock("bk:boom");
			}
		}

		protected override Rectangle GetCollider() {
			return new Rectangle(0, 0, 16, 15);
		}
	}
}