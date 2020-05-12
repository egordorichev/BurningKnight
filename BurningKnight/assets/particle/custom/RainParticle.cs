using System;
using BurningKnight.entity;
using BurningKnight.entity.creature.player;
using BurningKnight.level;
using BurningKnight.util;
using Lens;
using Lens.entity;
using Lens.graphics;
using Lens.util;
using Lens.util.camera;
using Lens.util.math;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace BurningKnight.assets.particle.custom {
	public class RainParticle : Entity {
		private Color color;
		private float target;
		private float size;
		private float speed;
		private bool poofed;

		public bool End;
		public bool Custom;

		private float delay;
		
		public override void Init() {
			base.Init();

			AlwaysActive = true;
			AlwaysVisible = true;
			Depth = Layers.WindFx;

			Reset();

			if (!Custom) {
				Y = Camera.Instance.Y + Rnd.Float(Display.Height + 100);
			} else {
				delay = Rnd.Float(0, 2f);
			}
		}

		private void Reset() {
			if (End) {
				Done = true;
				return;
			}
			
			color = new Color(Rnd.Float(0.3f, 0.5f), Rnd.Float(0.4f, 0.7f), Rnd.Float(0.7f, 0.8f), Rnd.Float(0.5f, 1f));
			X = Rnd.Float(Camera.Instance.X - 150, Camera.Instance.Right + 150);
			Y = Camera.Instance.Y - Rnd.Float(50, 60);
			size = Rnd.Float(20, 50);
			target = Camera.Instance.Y + Rnd.Float(Display.Height + 100);
			speed = Rnd.Float(1f, 1.5f);
			poofed = false;
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (delay > 0) {
				delay -= dt;

				return;
			}

			var s = dt * 300f * speed;
			Position += MathUtils.CreateVector(Weather.RainAngle, s);

			if (Y >= target) {
				if (!poofed) {
					poofed = true;
					var pos = Position + MathUtils.CreateVector(Weather.RainAngle, size);

					if (!Player.InBuilding) {
						var c = Rnd.Int(2, 5);

						for (var i = 0; i < c; i++) {
							var part = new ParticleEntity(Particles.Rain());

							part.Position = pos;
							part.Particle.Velocity = new Vector2(Rnd.Float(-40, 40), Rnd.Float(-30, -50));
							part.Particle.Scale = Rnd.Float(1f, 1.6f);
							Area.Add(part);
							part.Depth = 0;
						}
					}
				}
				
				size -= 0.5f * s * (float) Math.Sin(Weather.RainAngle);

				if (size <= 0) {
					Reset();
					return;
				}
			}

			if (Position.Y > Camera.Instance.Bottom + 20) {
				Reset();
			}
		}

		public override void Render() {
			if (Player.InBuilding) {
				return;
			}
			
			Graphics.Batch.DrawLine(X, Y, X + (float) Math.Cos(Weather.RainAngle) * size, Y + (float) Math.Sin(Weather.RainAngle) * size, color);
		}
	}
}