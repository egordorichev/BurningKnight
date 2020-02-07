using System;
using BurningKnight.assets;
using BurningKnight.assets.items;
using BurningKnight.assets.particle;
using BurningKnight.assets.particle.controller;
using BurningKnight.assets.particle.custom;
using BurningKnight.assets.particle.renderer;
using BurningKnight.entity.buff;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.state;
using BurningKnight.util;
using ImGuiNET;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.entity.component;
using Lens.util;
using Lens.util.file;
using Lens.util.math;
using Lens.util.timer;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.component {
	public class HealthComponent : SaveableComponent {
		private float health;
		public byte Phases;

		public bool RenderInvt;
		public float Health => health;
		public int MaxHealthCap = -1;
		public bool AutoKill = true;
		public bool SaveMaxHp;

		public bool HasNoHealth => health <= 0.01f;
		public float Percent => Health / MaxHealth;

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
				
				if (e.Amount > 0) {
					EmitParticles(false);
				}
				
				health = h;

				Send(new PostHealthModifiedEvent {
					Amount = e.Amount,
					Who = Entity,
					From = setter,
					Type = type
				});

				TryToKill(setter);
				return true;
			}

			return false;
		}

		private void TryToKill(Entity e) {
			if (health <= 0.1f && (!Entity.TryGetComponent<HeartsComponent>(out var c) || c.Total == 0) && AutoKill) { 
				Kill(e);
			}
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
			
			if (amount < 0) {
				if (Entity.TryGetComponent<HeartsComponent>(out var hearts)) {
					if (hearts.Total > 0) {
						if (Unhittable || InvincibilityTimer > 0) {
							return false;
						}

						if (hearts.Hurt((int) -Math.Round(amount), setter)) {
							InvincibilityTimer = InvincibilityTimerMax;

							Send(new PostHealthModifiedEvent {
								Amount = amount,
								Who = Entity,
								From = setter,
								Type = type
							});
							
							TryToKill(setter);
							return true;
						}
						
						return false;
					}
				}
			}
			
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
				var nw = value;

				if (MaxHealthCap > -1) {
					nw = Math.Min(MaxHealthCap, nw);
				}

				if (!Send(new MaxHealthModifiedEvent {
					Who = Entity,
					Amount = nw - old
				})) {
					maxHealth = nw;
					health = Math.Min(health, maxHealth);
				}
			}
		}

		public int InitMaxHealth {
			set {
				maxHealth = value;
				health = maxHealth;
			}
		}

		private bool dead;
		public bool Unhittable;
		public float InvincibilityTimer;
		public float InvincibilityTimerMax = 0.5f;

		public bool Dead => dead;

		public void Kill(Entity from) {
			if (Phases > 0) {
				Phases--;
				health = maxHealth;

				Send(new RevivedEvent {
					WhoDamaged = from,
					Who = Entity
				});

				return;
			}
			
			if (dead) {
				Entity.Done = true;	
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
		
		// Needed for loading
		public HealthComponent() {
			
		}

		public void EmitParticles(bool shield) {
			var slice = shield ? "shield" : "heart";

			for (var i = 0; i < 3; i++) {
				Timer.Add(() => {
						var part = new ParticleEntity(new Particle(Controllers.Float, new TexturedParticleRenderer(CommonAse.Particles.GetSlice($"{slice}_{Rnd.Int(1, 4)}"))));
						part.Position = Entity.Center;

						if (Entity.TryGetComponent<ZComponent>(out var z)) {
							part.Position -= new Vector2(0, z.Z);
						}
				
						Entity.Area.Add(part);
				
						part.Particle.Velocity = new Vector2(Rnd.Float(8, 16) * (Rnd.Chance() ? -1 : 1), -Rnd.Float(30, 56));
						part.Particle.Angle = 0;
						part.Particle.Alpha = 0.9f;
						part.Depth = Layers.InGameUi;
					}, i * 0.5f);
			}
		}

		public override bool HandleEvent(Event e) {
			if (e is ItemCheckEvent ev && ev.Item.Type == ItemType.Heart) {
				Send(new ItemAddedEvent {
					Item = ev.Item,
					Who = Entity
				});

				Audio.PlaySfx("item_heart");
				
				ev.Item.Use(Entity);

				Engine.Instance.State.Ui.Add(new ConsumableParticle(ev.Item.Animation != null
					? ev.Item.GetComponent<AnimatedItemGraphicsComponent>().Animation.GetFirstCurrent()
					: ev.Item.Region, (Player) Entity));
				
				ev.Item.Done = true;
				return true;
			} else if (e is ExplodedEvent b && !b.Handled) {
				Items.Unlock("bk:infinite_bomb");
				
				ModifyHealth(Entity is Player ? -2 : -b.Damage, b.Who, DamageType.Explosive);

				var component = Entity.GetAnyComponent<BodyComponent>();
				component?.KnockbackFrom(b.Who, 2);
			}
			
			return base.HandleEvent(e);
		}

		public bool IsFull() {
			return Math.Abs(health - MaxHealth) < 0.01f;
		}

		public bool CanPickup(Item item) {
			if (item.Id.Contains("heart")) {
				return !IsFull();
			}

			if (!Entity.TryGetComponent<HeartsComponent>(out var h)) {
				return false;
			}

			return h.CanHaveMore;
		}

		private bool applied;

		public override void Update(float dt) {
			base.Update(dt);
			InvincibilityTimer = Math.Max(0, InvincibilityTimer - dt);

			if (!applied) {
				applied = true;

				if (Entity.HasComponent<StatsComponent>()) {
					var amount = GetComponent<StatsComponent>().HeartsPayed * 2;

					maxHealth -= amount;
					health = Math.Min(maxHealth, health);
				}
			}
		}
		
		public override void Save(FileWriter stream) {
			base.Save(stream);
			
			stream.WriteFloat(health);
			stream.WriteByte(Phases);

			if (SaveMaxHp) {
				stream.WriteByte((byte) maxHealth);
			}
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			
			health = stream.ReadFloat();
			Phases = stream.ReadByte();

			if (SaveMaxHp) {
				maxHealth = stream.ReadByte();
			}
		}

		public override void RenderDebug() {
			var hp = health;
			
			if (ImGui.InputFloat("Health", ref hp)) {
				health = hp;
			}
			
			ImGui.InputInt("Max health", ref maxHealth);

			var v = (int) Phases;

			if (ImGui.InputInt("Phases", ref v)) {
				Phases = (byte) v;
			}
			
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