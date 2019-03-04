using System;
using BurningKnight.entity.creature;
using BurningKnight.entity.events;
using BurningKnight.util;
using Lens.entity;
using Lens.entity.component;

namespace BurningKnight.entity.component {
	public class HealthComponent : Component {
		private int health;
		
		public int Health => health;

		public void SetHealth(int hp, Entity setter) {
			if (hp < health) {
				if (Unhittable || InvincibilityTimer > 0) {
					return;
				}
				
				InvincibilityTimer = InvincibilityTimerMax;
			}

			var old = health;
			health = (int) MathUtils.Clamp(health + hp, 0, maxHealth);

			Send(new HealthModifiedEvent {
				Amount = health - old,
				From = setter
			});
			
			if (health == 0) {
				dead = true;
			}
		}

		public void ModifyHealth(int amount, Entity setter) {
			SetHealth(health + amount, setter);
		}

		private int maxHealth;
		
		public int MaxHealth {
			get => maxHealth;

			set {
				maxHealth = Math.Max(1, value);
				health = Math.Min(maxHealth, Health);
			}
		}

		private bool dead;
		public bool Unhittable;
		public float InvincibilityTimer;
		public float InvincibilityTimerMax = 0.5f;

		public bool Dead => dead;

		public void Kill(Entity from) {
			if (dead) {
				return;
			}

			dead = true;
			health = 0;
			
			HandleEvent(new DiedEvent {
				From = from
			});

			Entity.Done = true;
		}

		public HealthComponent() {
			maxHealth = 1;
			health = MaxHealth;
		}

		public override void Update(float dt) {
			base.Update(dt);
			InvincibilityTimer = Math.Max(0, InvincibilityTimer - dt);
		}
	}
}