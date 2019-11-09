using System;
using BurningKnight.assets.items;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.state;
using BurningKnight.util;
using ImGuiNET;
using Lens;
using Lens.entity;
using Lens.entity.component;
using Lens.util;
using Lens.util.file;

namespace BurningKnight.entity.component {
	public class HealthComponent : SaveableComponent {
		private float health;

		public bool RenderInvt;
		public float Health => health;
		public int MaxHealthCap = -1;
		public bool AutoKill = true;

		public bool SetHealth(float hp, Entity setter, bool mod = true, DamageType type = DamageType.Regular) {
			if (Math.Abs(hp - health) < 0.01f) {
				return false;
			}
			
			if (hp < health) {
				if (Unhittable || InvincibilityTimer > 0) {
					return false;
				}				
			}
			
			var old = health;
			var h = MathUtils.Clamp(0, maxHealth, hp);

			var e = new HealthModifiedEvent {
				Amount = h - old,
				Who = Entity,
				From = setter,
				Type = type
			};
			
			if (!Send(e)) {
				h = old + e.Amount;

				if (old > h) {
					InvincibilityTimer = mod ? InvincibilityTimerMax : 0.1f;
				}
				
				health = h;

				Send(new PostHealthModifiedEvent {
					Amount = e.Amount,
					Who = Entity,
					From = setter,
					Type = type
				});

				if (health <= 0.01f && AutoKill) {
					Kill(setter);
				}

				return true;
			}

			return false;
		}

		public bool ModifyHealth(float amount, Entity setter, DamageType type = DamageType.Regular) {
			if (amount < 0 && Entity is Player && (Run.Depth != -2 && Run.Depth < 1)) {
				if (Unhittable || InvincibilityTimer > 0 || Health <= 0.01f) {
					return false;
				}

				Send(new HealthModifiedEvent {
					Amount = 0,
					Who = Entity,
					From = null
				});
				
				InvincibilityTimer = InvincibilityTimerMax;
				return false;
			}
			
			/*if (amount < 0) {
				if (Entity.TryGetComponent<HeartsComponent>(out var hearts)) {
					if (hearts.Total > 0) {
						if (Unhittable || InvincibilityTimer > 0) {
							return false;
						}
				
						hearts.Hurt(-amount, setter);
						return true;
					}
				}
			}*/
			
			return SetHealth(health + (amount), setter, true, type);
		}

		private int maxHealth;

		public int MaxHealth {
			get => maxHealth;
			set {
				if (maxHealth == value) {
					return;
				}

				var old = maxHealth;
				var nw = Math.Max(1, value);

				if (MaxHealthCap > -1) {
					nw = Math.Min(MaxHealthCap, nw);
				}

				if (!Send(new MaxHealthModifiedEvent {
					Who = Entity,
					Amount = nw - old
				})) {
					maxHealth = nw;
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
			
			health = 0;

			if (!Send(new DiedEvent {
				From = from,
				Who = Entity
			})) {
				dead = true;
				Entity.Done = true;	
			}
		}

		public HealthComponent() {
			maxHealth = 2;
			health = MaxHealth;
		}

		public override bool HandleEvent(Event e) {
			if (e is ItemCheckEvent ev && ev.Item.Type == ItemType.Heart) {
				Send(new ItemAddedEvent {
					Item = ev.Item,
					Who = Entity
				});
				
				ev.Item.Use(Entity);
				ev.Item.Done = true;
				
				Engine.Instance.State.Ui.Add(new ConsumableParticle(ev.Item.Animation != null
						? ev.Item.GetComponent<AnimatedItemGraphicsComponent>().Animation.GetFirstCurrent()
						: ev.Item.Region, (Player) Entity));
				
				return true;
			} else if (e is ExplodedEvent b && !b.Handled) {
				Items.Unlock("bk:infinite_bomb");
				
				ModifyHealth(Entity is Player ? -2 : -32, b.Who, DamageType.Explosive);

				var component = Entity.GetAnyComponent<BodyComponent>();
				component?.KnockbackFrom(b.Who, 2);
			}
			
			return base.HandleEvent(e);
		}

		public bool IsFull() {
			return Math.Abs(health - MaxHealth) < 0.01f;
		}

		public override void Update(float dt) {
			base.Update(dt);
			InvincibilityTimer = Math.Max(0, InvincibilityTimer - dt);
		}
		
		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteFloat(health);
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			health = stream.ReadFloat();
		}

		public override void RenderDebug() {
			var hp = health;
			
			if (ImGui.InputFloat("Health", ref hp)) {
				health = hp;
			}
			
			ImGui.InputInt("Max health", ref maxHealth);
			ImGui.Checkbox("Unhittable", ref Unhittable);

			if (ImGui.Button("Heal")) {
				ModifyHealth(maxHealth - health, null);
			}
			
			ImGui.SameLine();
			
			if (ImGui.Button("Hurt")) {
				ModifyHealth(-1, null);
			}
			
			ImGui.SameLine();
			
			if (ImGui.Button("Kill")) {
				ModifyHealth(-health, null);
			}
		}
	}
}