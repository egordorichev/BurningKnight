using System;
using BurningKnight.assets.items;
using BurningKnight.assets.lighting;
using BurningKnight.assets.particle;
using BurningKnight.assets.particle.controller;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.boss;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.level;
using BurningKnight.level.entities;
using BurningKnight.level.tile;
using BurningKnight.state;
using BurningKnight.ui;
using BurningKnight.ui.dialog;
using BurningKnight.util;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.graphics;
using Lens.util.camera;
using Lens.util.timer;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.creature.bk {
	public class BurningKnight : Boss {
		private BossPatternSet<BurningKnight> set;
	
		public override void AddComponents() {
			base.AddComponents();
			
			AddTag(Tags.BurningKnight);

			Width = 22;
			Height = 27;
			Flying = true;

			var b = new RectBodyComponent(8, 8, Width - 8, Height - 8) {
				KnockbackModifier = 0
			};

			AddComponent(b);
			b.Body.LinearDamping = 4;
			
			AddComponent(new AnimationComponent("old_burning_knight"));

			var health = GetComponent<HealthComponent>();
			health.InitMaxHealth = 48 + (Run.Depth - 1) * 20;
			
			GetComponent<StateComponent>().Become<IdleState>();
			AddComponent(new OrbitGiverComponent());
			
			AddComponent(new DialogComponent());
			AddComponent(new LightComponent(this, 32, new Color(1f, 0.2f, 0.1f, 0.5f)));
		}

		protected override void OnTargetChange(Entity target) {
			if (!Awoken && target != null) {
				GetComponent<DialogComponent>().StartAndClose("burning_knight_0", 3);
				Audio.PlayMusic("Rogue");
				
				Timer.Add(() => {
					// SelectAttack();
				}, 3f);
			}

			base.OnTargetChange(target);
		}

		private float lastPart;
		private bool added;

		public override void Update(float dt) {
			base.Update(dt);

			if (!added) {
				added = true;
				
				var e = new Entity();
				Area.Add(e);
				e.Center = GetComponent<RoomComponent>().Room.Center;
				e.AddComponent(new LightComponent(e, 256f, new Color(1f, 0.9f, 0.5f, 1f)));
			}
			
			if (died) {
				lastPart -= dt;

				if (lastPart <= 0) {
					lastPart = 0.03f;
					var p = new ParticleEntity(new Particle(Controllers.BkDeath, Particles.BkDeathRenderer));
					Area.Add(p);
					p.Particle.Position = Center;
				}
				
				deathTimer += dt;
				lastExplosion -= dt;
				
				if (lastExplosion <= 0) {
					lastExplosion = 0.3f;
					AnimationUtil.Explosion(Center + new Vector2(Random.Float(-16, 16), Random.Float(-16, 16)));
					Camera.Instance.Shake(10);
					Audio.PlaySfx("explosion");
				}

				if (deathTimer > 1f) {
					Engine.Instance.FlashColor = new Color(1f, 1f, 1f, (deathTimer - 1) * 0.5f);
					Engine.Instance.Flash = 0.3f;
				}

				if (deathTimer >= 3f) {
					HandleEvent(new DefeatedEvent {
						BurningKnight = this
					});
					
					Engine.Instance.FlashColor = ColorUtils.WhiteColor;
					Done = true;
					PlaceRewards();
					HandleEvent(new BurningKnightDefeatedEvent());
					
					Timer.Add(() => {
						((InGameState) Engine.Instance.State).ResetFollowing();
					}, 0.5f);
				}
			}
		}

		public override void SelectAttack() {
			if (set == null) {
				set = BurningKnightAttackRegistry.PatternSetRegistry.Generate(Run.Level.Biome.Id);
			}
			
			base.SelectAttack();
			GetComponent<StateComponent>().PushState(BurningKnightAttackRegistry.GetNext(set));
		}

		private bool died;
		private float deathTimer;
		private float lastExplosion;
		
		public override bool HandleEvent(Event e) {
			if (e is DiedEvent) {
				if (!died) {
					died = true;

					HealthBar.Remove();

					Camera.Instance.Targets.Clear();
					Camera.Instance.Follow(this, 1f);
					Become<DefeatedState>();

					Audio.Stop();
				}
				
				Done = false;
				e.Handled = true;
			}
			
			return base.HandleEvent(e);
		}
		
		private void PlaceRewards() {
			var exit = new Exit();
			Area.Add(exit);
				
			exit.To = Run.Depth + 1;

			var x = (int) Math.Floor(CenterX / 16);
			var y = (int) Math.Floor(CenterY / 16);
			var p = new Vector2(x * 16 + 8, y * 16 + 8);
			
			exit.Center = p;

			Painter.Fill(Run.Level, x - 1, y - 1, 3, 3, Tiles.RandomFloor());
			Painter.Fill(Run.Level, x - 1, y - 3, 3, 3, Tiles.RandomFloor());

			var stand = new BossStand();
			Area.Add(stand);
			stand.Center = p - new Vector2(0, 32f);
			stand.SetItem(Items.CreateAndAdd(Items.Generate(ItemPool.Boss), Area), null);
			
			Run.Level.TileUp();
			Run.Level.CreateBody();
		}

		private class DefeatedState : SmartState<BurningKnight> {
			
		}

		public class DefeatedEvent : Event {
			public BurningKnight BurningKnight;
		}
	}
}