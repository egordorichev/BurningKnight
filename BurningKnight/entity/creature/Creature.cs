using System;
using BurningKnight.assets.particle;
using BurningKnight.assets.particle.controller;
using BurningKnight.entity.bomb;
using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.drop;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.npc.dungeon;
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
using Lens;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.graphics;
using Lens.util;
using Lens.util.camera;
using Lens.util.math;
using Microsoft.Xna.Framework;

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
			
			AddDrops(new SingleDrop("bk:heart", 0.02f));
			AddDrops(new SingleDrop("bk:half_heart", 0.005f));
			AddDrops(new SingleDrop("bk:shield", 0.01f));
			AddDrops(new SingleDrop("bk:bomb", 0.01f));
			AddDrops(new SingleDrop("bk:key", 0.01f));
		}

		protected virtual void Become<T>() {
			GetComponent<StateComponent>().Become<T>();
		}
		
		public virtual void Kill(Entity w) {
			GetComponent<HealthComponent>().Kill(w);
		}

		public override bool HandleEvent(Event e) {
			if (e is PostHealthModifiedEvent ev && ev.Amount < 0) {
				if (HasNoHealth(ev)) {
					Kill(ev.From);
				} else if (!ev.PressedForBomb) {
					GetComponent<AudioEmitterComponent>().EmitRandomized(GetHurtSfx());
				}

				if (ev.From != null && !ev.Handled && !ev.PressedForBomb) {
					var b = GetAnyComponent<BodyComponent>();

					if (b != null && ev.Amount < 0) {
						b.KnockbackFrom(ev.From);

						if (Settings.Blood) {
							for (var i = 0; i < 8; i++) {
								var p = Particles.Wrap(new Particle(Controllers.Blood, Particles.BloodRenderer), Area,
									Center + Rnd.Vector(-4, 4));

								var a = ev.From.AngleTo(this);

								p.Particle.Velocity = MathUtils.CreateVector(a + Rnd.Float(-0.5f, 0.5f), Rnd.Float(80, 120));
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
						// GetComponent<BuffsComponent>().Add(BurningBuff.Id);

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
			
			if (d.From is Creature c) {
				d.From.HandleEvent(new KilledEvent {
					Who = this,
					KilledBy = c
				});
			}
			
			return false;
		}

		public virtual void AnimateDeath(DiedEvent d) {
			AudioEmitterComponent.Dummy(Area, Center).EmitRandomized(GetDeadSfx(), sz: 0.2f);

			if (!GetComponent<TileInteractionComponent>().HasNoSupport) {
				GetComponent<DropsComponent>().SpawnDrops();
			}

			Done = true;
			
			for (var i = 0; i < 5; i++) {
				var part = new ParticleEntity(Particles.Ash());

				part.Position = Center;
				part.Particle.Scale = Rnd.Float(1.5f, 2f);
				part.Particle.Velocity = new Vector2(Rnd.Float(20, 30) * (Rnd.Chance() ? -1 : 1), -Rnd.Float(40, 66));
				Run.Level.Area.Add(part);
				part.Depth = 1;
			}

			if (Settings.Blood) {
				for (var i = 0; i < Rnd.Int(2, 8); i++) {
					Area.Add(new SplashParticle {
						Position = Center - new Vector2(2.5f),
						Color = GetBloodColor()
					});
				}
			}
		}

		protected virtual TextureRegion GetDeathFrame() {
			return GetAnyComponent<AnimationComponent>()?.Animation.GetFrame("dead", 0);
		}

		protected virtual void CreateGore(DiedEvent d) {
			Engine.Instance.Freeze = 0.5f;
			Camera.Instance.ShakeMax(5);
			

			if (!Settings.Blood) {
				return;
			}

			var gore = new Gore();
			var r = GetDeathFrame();

			if (r == null) {
				Log.Error($"Failed to find dead frame for {GetType().Name}");
				return;
			}
			
			Area.Add(gore);
			
			gore.Position = Position;
			gore.AddComponent(new ZSliceComponent(r));

			gore.Width = r.Width;
			gore.Height = r.Height;

			var b = new RectBodyComponent(0, 0, gore.Width, gore.Height);
			
			gore.AddComponent(b);
			
			b.Body.LinearDamping = 2f;
			b.Body.Restitution = 1;
			b.Body.Friction = 0;

			if (d != null) {
				var v = MathUtils.CreateVector(d.From is Player ? AngleTo(d.From) - Math.PI : Rnd.AnglePI(), 64);

				b.Body.LinearVelocity = v;

				if (v.X > 0) {
					gore.GetComponent<ZSliceComponent>().Flipped = true;
				}
			}
		}

		protected void AddDrops(params Drop[] drops) {
			GetComponent<DropsComponent>().Add(drops);
		}

		public virtual bool HasNoHealth(HealthModifiedEvent e = null) {
			return GetComponent<HealthComponent>().HasNoHealth || Math.Abs(GetComponent<HealthComponent>().Health - (-e?.Amount ?? 0)) < 0.01f;
		}

		public virtual bool HasNoHealth(PostHealthModifiedEvent e = null) {
			return GetComponent<HealthComponent>().HasNoHealth;
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
			return !((entity is Creature && !(entity is DungeonShopNpc || entity is Mimic)) || (InAir() && (entity is Chasm || entity is Item || entity is Bomb || (!ShouldCollideWithDestroyableInAir() && entity is HalfWall))));
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
			return $"hurt{Rnd.Int(1, 3)}";
		}

		protected virtual string GetDeadSfx() {
			return $"dead{Rnd.Int(1, 4)}";
		}
	}
}