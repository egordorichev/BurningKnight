using System;
using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using Lens.entity;
using BurningKnight.assets.particle;
using BurningKnight.assets.particle.controller;
using BurningKnight.assets.particle.renderer;
using BurningKnight.entity;
using Lens.graphics;
using Lens.util;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;
using Random = Lens.util.math.Random;

namespace BurningKnight.level.entities {
	public class Gramophone : Prop {
		private TextureRegion top;
		private TextureRegion bottom;
		private float t;
		private float tillNext;
		
		public override void Init() {
			base.Init();

			Width = 16;
			Height = 23;

			top = CommonAse.Props.GetSlice("player_top");
			bottom = CommonAse.Props.GetSlice("player");
		}

		public override void AddComponents() {
			base.AddComponents();
			
			AddTag(Tags.Gramophone);
			
			AddComponent(new RoomComponent());
			AddComponent(new ExplodableComponent());
			AddComponent(new ShadowComponent(RenderWithShadow));
			AddComponent(new RectBodyComponent(2, 14, 12, 4, BodyType.Static));
			AddComponent(new SensorBodyComponent(2, 2, Width - 4, Height - 4, BodyType.Static));

			AddComponent(new HealthComponent {
				InitMaxHealth = 0
			});
		}

		public override void Update(float dt) {
			base.Update(dt);
			
			t += dt;

			if (GetComponent<HealthComponent>().Health == 0) {
				return;
			}
			
			tillNext -= dt;

			if (tillNext <= 0) {
				tillNext = Random.Float(1, 3f);
				
				var part = new ParticleEntity(new Particle(Controllers.Float, new TexturedParticleRenderer(CommonAse.Particles.GetSlice($"note_{Random.Int(1, 3)}"))));
				part.Position = Center;
				Area.Add(part);
				
				part.Particle.Velocity = new Vector2(Random.Float(8, 16) * (Random.Chance() ? -1 : 1), -Random.Float(40, 66));
				part.Particle.Angle = 0;
				part.Depth = Layers.InGameUi;
			}
		}

		public override void Render() {
			RealRender();
		}

		private void RealRender(bool shadow = false) {
			var broken = GetComponent<HealthComponent>().Health == 0;
			
			if (shadow) {
				Graphics.Render(bottom, Position + new Vector2(0, 34), 0, Vector2.Zero, MathUtils.InvertY);

				if (broken) {
					return;
				}
				
				Graphics.Render(top, Position + new Vector2(9, 28), (float) Math.Cos(t) * -0.1f, new Vector2(9, 14),
					new Vector2((float) Math.Cos(t * 2f) * 0.05f + 1f, (float) Math.Sin(t * 2f) * -0.05f - 1f));

				return;
			}
			
			Graphics.Render(bottom, Position + new Vector2(0, 12));

			if (broken) {
				return;
			}

			Graphics.Render(top, Position + new Vector2(9, 14), (float) Math.Cos(t) * 0.1f, new Vector2(9, 14),
				new Vector2((float) Math.Cos(t * 2f) * 0.05f + 1f, (float) Math.Sin(t * 2f) * 0.05f + 1f));
		}

		private void RenderWithShadow() {
			RealRender(true);
		}

		public override bool HandleEvent(Event e) {
			if (e is HealthModifiedEvent) {
				HandleEvent(new GramophoneBrokenEvent {
					Gramophone = this
				});
			}
			
			return base.HandleEvent(e);
		}
	}
}