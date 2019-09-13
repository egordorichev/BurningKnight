using System;
using BurningKnight.assets.particle;
using BurningKnight.assets.particle.controller;
using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.fx;
using BurningKnight.entity.item;
using BurningKnight.level;
using BurningKnight.level.tile;
using BurningKnight.physics;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.ui.editor;
using BurningKnight.util;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.graphics;
using Lens.util;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.creature {
	public class Creature : SaveableEntity, CollisionFilterEntity, PlaceableEntity {
		public bool Flying;
		
		public override void AddComponents() {
			base.AddComponents();

			AddComponent(new BuffsComponent());

			AddComponent(new HealthComponent {
				RenderInvt = true,
				AutoKill = false
			});
			
			AddComponent(new StateComponent());
			AddComponent(new RoomComponent());
			AddComponent(new ExplodableComponent());
			AddComponent(new DropsComponent());
			AddComponent(new TileInteractionComponent());
			AddComponent(new SupportableComponent());
			AddComponent(new ShadowComponent(RenderShadow));
			AddComponent(new AudioEmitterComponent());
			
			AddDrops(new SingleDrop("bk:heart", 0.05f));
		}

		protected void Become<T>() {
			GetComponent<StateComponent>().Become<T>();
		}
		
		public void Kill(Entity w) {
			GetComponent<HealthComponent>().Kill(w);
		}

		public override bool HandleEvent(Event e) {
			if (e is PostHealthModifiedEvent ev && ev.Amount < 0) {
				if (HasNoHealth(ev)) {
					Kill(ev.From);
				} else {
					GetComponent<AudioEmitterComponent>().EmitRandomized(GetHurtSfx());
				}

				if (ev.From != null && !ev.Handled) {
					var b = GetAnyComponent<BodyComponent>();

					if (b != null && ev.Amount < 0) {
						b.KnockbackFrom(ev.From);

						if (Settings.Blood) {
							for (var i = 0; i < 8; i++) {
								var p = Particles.Wrap(new Particle(Controllers.Blood, Particles.BloodRenderer), Area,
									Center + Random.Vector(-4, 4));

								var a = ev.From.AngleTo(this);

								p.Particle.Velocity = MathUtils.CreateVector(a + Random.Float(-0.5f, 0.5f), Random.Float(80, 120));
								p.Particle.Velocity.Y -= 8f;
								p.Particle.Velocity += b.Velocity * 0.5f;
							}
						}
					}
				}
			} else if (e is DiedEvent d) {
				if (!e.Handled) {
					try {
						if (HandleDeath(d)) {
							return true;
						}
					} catch (Exception ex) {
						Log.Error(ex);
					}
				}
			} else if (e is LostSupportEvent) {
				if (!(this is Player)) {
					Done = true;
					return true;
				}
			} else if (e is TileCollisionStartEvent tce) {
				if (tce.Tile == Tile.Lava) {
					if (GetComponent<HealthComponent>().ModifyHealth(-1, Run.Level)) {
						GetComponent<BuffsComponent>().Add(BurningBuff.Id);

						var set = false;
						var center = Center;
						var count = 0;

						GetComponent<TileInteractionComponent>().ApplyForAllTouching((i, x, y) => {
							if (Run.Level.Get(x, y, true) == Tile.Lava) {
								var v = new Vector2(x * 16, y * 16);
								count++;

								if (!set) {
									set = true;
									center = v;
								} else {
									center += v;
								}
							}
						});

						GetAnyComponent<BodyComponent>()?.KnockbackFrom(center / count, 1.5f);
					}
				}
			}

			return base.HandleEvent(e);
		}

		protected virtual bool HandleDeath(DiedEvent d) {
			AnimateDeath(d);
			return false;
		}

		public virtual void AnimateDeath(DiedEvent d) {
			GetComponent<AudioEmitterComponent>().EmitRandomized(GetDeadSfx(), 1, false);
			GetComponent<DropsComponent>().SpawnDrops();
			Done = true;
			
			for (var i = 0; i < 5; i++) {
				var part = new ParticleEntity(Particles.Dust());

				part.Position = Center;
				part.Particle.Scale = Random.Float(0.4f, 0.8f);
				Run.Level.Area.Add(part);
				part.Depth = 1;
			}

			if (Settings.Blood) {
				for (var i = 0; i < Random.Int(2, 8); i++) {
					Area.Add(new SplashParticle {
						Position = Center - new Vector2(2.5f),
						Color = GetBloodColor()
					});
				}
			}
		}

		protected void AddDrops(params Drop[] drops) {
			GetComponent<DropsComponent>().Add(drops);
		}

		public virtual bool HasNoHealth(HealthModifiedEvent e = null) {
			return GetComponent<HealthComponent>().Health == 0 || GetComponent<HealthComponent>().Health == (-e?.Amount ?? 0);
		}

		public virtual bool HasNoHealth(PostHealthModifiedEvent e = null) {
			return GetComponent<HealthComponent>().Health == 0;
		}
		
		public virtual bool InAir() {
			return Flying;
		}

		public virtual bool IgnoresProjectiles() {
			return false;
		}

		public virtual bool ShouldCollideWithDestroyableInAir() {
			return false;
		}

		public virtual bool ShouldCollide(Entity entity) {
			return !(entity is Creature || (InAir() && ((entity is Chasm) || entity is Item || entity is Bomb || (entity is DestroyableLevel && !ShouldCollideWithDestroyableInAir()))));
		}

		public virtual bool IsFriendly() {
			return true;
		}

		protected virtual void RenderShadow() {
			GraphicsComponent?.Render(true);
		}

		protected virtual Color GetBloodColor() {
			return Color.Red;
		}

		protected virtual string GetHurtSfx() {
			return $"hurt{Random.Int(1, 3)}";
		}

		protected virtual string GetDeadSfx() {
			return $"dead{Random.Int(1, 4)}";
		}
	}
}