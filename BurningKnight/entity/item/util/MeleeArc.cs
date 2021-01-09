﻿using System;
using System.Collections.Generic;
using BurningKnight.assets.lighting;
using BurningKnight.entity.bomb;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using BurningKnight.level;
using BurningKnight.level.biome;
using BurningKnight.physics;
using BurningKnight.state;
using Lens.entity;
using Lens.graphics;
using Lens.util.camera;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.item.util {
	public delegate void ArcHurtCallback(MeleeArc p, Entity e);
	public delegate void ArcDeathCallback(MeleeArc p);

	public class MeleeArc : Entity {
		public static Color ReflectedColor = new Color(0.5f, 1f, 0.5f, 1f);
		
		public float LifeTime = 0.1f;
		public float Damage;
		public Entity Owner;
		public float Angle;
		public string Sound = "item_sword_hit";
		public Color Color = ColorUtils.WhiteColor;
		public bool Mines;
		public float Knockback;

		public ArcHurtCallback OnHurt;
		public ArcDeathCallback OnDeath;

		private float t;
		private Vector2 velocity;
		private List<Entity> hurt = new List<Entity>();

		public override void AddComponents() {
			base.AddComponents();

			const float force = 40f;
			velocity = new Vector2((float) Math.Cos(Angle) * force, (float) Math.Sin(Angle) * force);
			
			AddComponent(new RectBodyComponent(0, -Height / 2f, Width, Height, BodyType.Dynamic, true) {
				Angle = Angle
			});

			AddComponent(new AnimationComponent("sword_trail", null, "idle") {
				Offset = new Vector2(4, 12),
				Scale = new Vector2(Width / 8, Height / 24)
			});
			
			AddComponent(new LightComponent(this, 32f, Color.White));

			GetComponent<AnimationComponent>().OriginY = 12;
			Camera.Instance.Push(Angle - (float) Math.PI, 4f);
		}

		public void AdjustSize() {
			GetComponent<RectBodyComponent>().Resize(0, -Height / 2f, Width, Height);
		}

		public override void Render() {
			var component = GetComponent<AnimationComponent>();
			var region = component.Animation.GetCurrentTexture();

			Graphics.Color = Color;
			Graphics.Render(region, Position, Angle, component.Offset, component.Scale);
			Graphics.Color = ColorUtils.WhiteColor;
		}

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent ev) {
				if (ev.Entity is HalfProjectileLevel bdd) {
					if (Mines) {
						Physics.Fixture.GetAABB(out var hitbox, 0);
						ProjectileLevelBody.Mine(Run.Level, hitbox.Center.X, hitbox.Center.Y);
					}
				} else if (ev.Entity is ProjectileLevelBody bd) {
					if (Mines) {
						Physics.Fixture.GetAABB(out var hitbox, 0);
						ProjectileLevelBody.Mine(Run.Level, hitbox.Center.X, hitbox.Center.Y);
					}
					
					if (Run.Level.Biome is IceBiome) {
						Physics.Fixture.GetAABB(out var hitbox, 0);

						if (bd.Break(hitbox.Center.X, hitbox.Center.Y)) {
							AudioEmitterComponent.Dummy(Area, Center).EmitRandomizedPrefixed("level_snow_break", 3);
						}
					}
				} else if (ev.Entity is Bomb) {
					ev.Entity.GetComponent<RectBodyComponent>().KnockbackFrom(Owner, 1f + Knockback);
				} else if (ev.Entity is Projectile p) {
					if ((p.Owner is Mob) != (Owner is Mob) && ((p.FirstOwner is Mob) != (Owner is Mob))) {
						if (p.HasFlag(ProjectileFlags.Reflectable)) {
							p.Owner = Owner;
							p.Damage *= 2f;

							// fixme: lethal
							// p.Pattern?.Remove(p);

							var b = p.GetAnyComponent<BodyComponent>();
							var d = Math.Max(400, b.Velocity.Length() * 1.8f);
							var a = Owner.AngleTo(p);

							b.Velocity = new Vector2((float) Math.Cos(a) * d, (float) Math.Sin(a) * d);

							if (p.TryGetComponent<LightComponent>(out var l)) {
								l.Light.Color = ReflectedColor;
							}

							p.Color = ProjectileColor.Yellow;

							Camera.Instance.ShakeMax(4f);
							Owner.GetComponent<AudioEmitterComponent>().EmitRandomizedPrefixed("projectile_reflected", 2);
						} else if (p.HasFlag(ProjectileFlags.BreakableByMelee)) {
							p.Break();
						}
					}
				} else if (ev.Entity != Owner && (!(ev.Entity is Player) || !(Owner is Player))) {
					if (ev.Entity.TryGetComponent<HealthComponent>(out var health)) {
						if (!hurt.Contains(ev.Entity)) {
							if (Knockback > 0) {
								ev.Entity.GetAnyComponent<BodyComponent>()?.KnockbackFrom(Owner, Knockback);
							}

							if (health.ModifyHealth(-Damage, Owner)) {
								Owner.GetComponent<AudioEmitterComponent>().EmitRandomizedPrefixed(Sound, 3);
								OnHurt?.Invoke(this, ev.Entity);
							}

							hurt.Add(ev.Entity);
						}
					} else if (ev.Entity is ProjectileLevelBody && !HitWall) {
						HitWall = true;
						Owner.GetComponent<AudioEmitterComponent>().EmitRandomized("item_sword_hit_wall");
					}
				}
			}
			
			return base.HandleEvent(e);
		}

		private bool HitWall;

		public override void Update(float dt) {
			base.Update(dt);

			t += dt;
			Position = Owner.Center + velocity * t;

			if (t >= LifeTime) {
				OnDeath?.Invoke(this);
				Done = true;
			}
		}
		
		public class CreatedEvent : Event {
			public MeleeArc Arc;
			public Entity Owner;
			public Item By;
		}
	}
}