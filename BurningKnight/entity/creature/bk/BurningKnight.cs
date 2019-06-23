using System;
using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.boss;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.level;
using BurningKnight.level.entities;
using BurningKnight.level.tile;
using BurningKnight.state;
using BurningKnight.ui;
using BurningKnight.util;
using Lens;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.util.camera;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.creature.bk {
	public class BurningKnight : Boss {
		private HealthBar healthBar;
		private BossPatternSet<BurningKnight> set;
	
		public override void AddComponents() {
			base.AddComponents();
			
			AddTag(Tags.BurningKnight);

			Width = 42;
			Height = 42;
			Flying = true;

			var b = new RectBodyComponent(16, 16, Width - 32, Height - 32) {
				KnockbackModifier = 0
			};

			AddComponent(b);
			b.Body.LinearDamping = 4;
			
			// FIXME: TMP sprite and size, obv
			AddComponent(new AnimationComponent("burning_knight"));

			var health = GetComponent<HealthComponent>();
			health.InitMaxHealth = 48 + (Run.Depth - 1) * 20;
			
			GetComponent<StateComponent>().Become<IdleState>();
			
			AddComponent(new OrbitGiverComponent());
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (died) {
				deathTimer += dt;
				lastExplosion -= dt;

				if (lastExplosion <= 0) {
					lastExplosion = 0.3f;
					AnimationUtil.Explosion(Center + new Vector2(Random.Float(-16, 16), Random.Float(-16, 16)));
					Camera.Instance.Shake(3);
				}

				if (deathTimer >= 3f) {
					Done = true;
					((InGameState) Engine.Instance.State).ResetFollowing();
					PlaceRewards();
				}
			}
			
			if (set == null) {
				set = BurningKnightAttackRegistry.PatternSetRegistry.Generate(Run.Level.Biome.Id);
			}

			if (healthBar == null) {
				healthBar = new HealthBar(this);
				Engine.Instance.State.Ui.Add(healthBar);
			}
		}

		public override void SelectAttack() {
			base.SelectAttack();
			GetComponent<StateComponent>().PushState(BurningKnightAttackRegistry.GetNext(set));
		}

		private bool died;
		private float deathTimer;
		private float lastExplosion;
		
		public override bool HandleEvent(Event e) {
			if (e is DiedEvent && !died) {
				died = true;
				Done = false;

				e.Handled = true;

				Camera.Instance.Targets.Clear();
				Camera.Instance.Follow(this, 1f);
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
	}
}