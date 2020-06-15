using System;
using System.Collections.Generic;
using BurningKnight.assets;
using BurningKnight.assets.particle;
using BurningKnight.assets.particle.controller;
using BurningKnight.assets.particle.renderer;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.boss;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.graphics;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.ui {
	public class HealthBar : Entity {
		private const float ChangeDelay = 1f;
		private static Vector2 barOffset = new Vector2(1, 6);
		
		private TextureRegion frame;
		private TextureRegion fill;
		private TextureRegion damage;
		private TextureRegion phase;
		private TextureRegion phaseB;
		
		private float lastHp;
		private float sinceLastDamage;
		private float lastChange;
		private float lastDamage;
		private bool charge;
		
		private Boss entity;
		private List<float> phases = new List<float>();

		private string name;
		private float nameW;
		
		public HealthBar(Boss owner) {
			entity = owner;
			name = Locale.Get(owner.GetId());
			nameW = Font.Small.MeasureString(name).Width;
		}

		public void AddPhase(float percent) {
			phases.Add(percent);
		}
		
		public override void AddComponents() {
			base.AddComponents();

			AlwaysActive = true;
			AlwaysVisible = true;

			frame = CommonAse.Ui.GetSlice("hb_frame");
			fill = CommonAse.Ui.GetSlice("hb");
			damage = CommonAse.Ui.GetSlice("hb_damage");
			phase = CommonAse.Ui.GetSlice("hb_phase");
			phaseB = CommonAse.Ui.GetSlice("hb_b");
			
			Width = frame.Width;
			Height = frame.Height;

			CenterX = Display.UiWidth / 2f;
			Y = -Height;

			Depth = 32;
		}

		private bool showedUp;
		private bool tweened;

		public override void Update(float dt) {
			base.Update(dt);

			if (charge) {
				sinceLastDamage += dt;

				if (sinceLastDamage >= ChangeDelay) {
					var health = entity.GetComponent<HealthComponent>();
					var h = health.Health;
					var s = (lastChange - h) / health.MaxHealth * fill.Width;

					var p = new ParticleEntity(new Particle(new HealthParticleController(), new HealthParticleRenderer(damage, s)));
					p.Particle.Position = Position + barOffset + new Vector2((float) h / health.MaxHealth * fill.Width, 0);
					p.Position = p.Particle.Position;
					p.AlwaysActive = true;
					p.AlwaysVisible = true;
					Area.Add(p);
					p.Depth = Depth + 1;
					
					Tween.To(h, lastChange, x => lastChange = x, 0.3f);
					charge = false;
				}
			}

			if (entity.Awoken && !showedUp) {
				showedUp = true;
				Tween.To(0, Y, x => Y = x, 0.3f);
			}

			if (showedUp && (!entity.Awoken || (entity.Done && !tweened))) {
				Remove();
			}
		}

		public void Remove() {
			tweened = true;

			if (showedUp) {
				Tween.To(-Height - 14, Y, x => Y = x, 0.3f).OnEnd = () => {
					tweened = false;
				};
			}
		}

		public override void Render() {
			if (!entity.Awoken) {
				return;
			}
			
			Graphics.Render(frame, Position);

			var health = entity.GetComponent<HealthComponent>();
			var region = new TextureRegion(fill.Texture, fill.Source);
			var h = health.Health;
			
			if (h < lastDamage) {
				sinceLastDamage = 0;
				lastDamage = h;
				charge = true;
			} else if (h > lastDamage) {
				lastDamage = h;
			}

			if (h > lastChange) {
				lastChange = h;
			}
			
			Graphics.Render(damage, Position + barOffset, 0, Vector2.Zero, new Vector2(lastChange / health.MaxHealth * region.Width, 1));

			if (lastHp > h) {
				lastHp += Engine.Delta * 10f * (h - lastHp);

				if (Math.Abs(h) < 0.01f && lastHp <= 2f) {
					lastHp = 0;
				}
			} else {
				lastHp = h;
			}

			var w = region.Width;
			region.Source.Width = (int) Math.Ceiling(lastHp / health.MaxHealth * w);
			Graphics.Render(region, Position + barOffset);
			
			foreach (var p in phases) {
				Graphics.Render(health.Health / health.MaxHealth >= p ? phase : phaseB, Position + barOffset + new Vector2(p * (w - 1), 0));
			}

			Graphics.Print(name, Font.Small, new Vector2(CenterX - nameW * 0.5f, Bottom - 3));
		}
	}
}