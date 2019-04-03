using System;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.util;
using Lens.entity;
using Lens.entity.component;
using Lens.util.file;

namespace BurningKnight.entity.component {
	public class HealthComponent : SaveableComponent {
		private int health;

		public bool RenderInvt;
		public int Health => health;

		public void SetHealth(int hp, Entity setter) {			
			if (hp < health) {
				if (Unhittable || InvincibilityTimer > 0) {
					return;
				}				
			}
			
			var old = health;
			var h = (int) MathUtils.Clamp(0, maxHealth, hp);

			if (!Send(new HealthModifiedEvent {
				Amount = h - old,
				From = setter
			})) {
				if (old > h) {
					InvincibilityTimer = InvincibilityTimerMax;
				}
				
				health = h;
			}
		}

		public void ModifyHealth(int amount, Entity setter) {
			if (amount < 0) {
				if (Entity.TryGetComponent<HeartsComponent>(out var hearts)) {
					if (hearts.Total > 0) {
						if (Unhittable || InvincibilityTimer > 0) {
							return;
						}
				
						hearts.Hurt(-amount, setter);
						return;
					}
				}
			}
			
			SetHealth(health + amount, setter);
		}

		private int maxHealth;

		public int MaxHealth {
			get => maxHealth;

			set {
				var old = maxHealth;
				maxHealth = Math.Max(1, value);

				if (old == health) {
					health = maxHealth;
				} else {
					health = Math.Min(maxHealth, Health);
				}
			}
		}

		public int InitMaxHealth {
			set {
				maxHealth = Math.Max(1, value);
				health = maxHealth;
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

			if (!Send(new DiedEvent {
				From = from,
				Who = Entity
			})) {
				dead = true;
				health = 0;
				Entity.Done = true;	
			}
		}

		public HealthComponent() {
			maxHealth = 2;
			health = MaxHealth;
		}

		public override bool HandleEvent(Event e) {
			if (e is ItemCheckEvent ev && ev.Item.Type == ItemType.Heart) {
				ev.Item.Use(Entity);
				ev.Item.Done = true;
				return true;
			} else if (e is ExplodedEvent b) {
				ModifyHealth(-2, b.Who);
			}
			
			return base.HandleEvent(e);
		}

		public bool IsFull() {
			return health == MaxHealth;
		}

		public override void Update(float dt) {
			base.Update(dt);
			InvincibilityTimer = Math.Max(0, InvincibilityTimer - dt);
		}
		
		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteInt32(health);
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			health = stream.ReadInt32();
		}
	}
}