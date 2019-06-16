using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.fx;
using BurningKnight.entity.item;
using BurningKnight.level;
using BurningKnight.physics;
using BurningKnight.save;
using BurningKnight.ui.editor;
using BurningKnight.util;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.graphics;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature {
	public class Creature : SaveableEntity, CollisionFilterEntity, PlaceableEntity {
		public bool Flying;
		
		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new HealthComponent {
				RenderInvt = true,
				AutoKill = false
			});
			
			AddComponent(new StateComponent());
			AddComponent(new RoomComponent());
			AddComponent(new BuffsComponent());
			AddComponent(new ExplodableComponent());
			AddComponent(new DropsComponent());
			AddComponent(new TileInteractionComponent());
			AddComponent(new ShadowComponent(RenderShadow));
			
			AddDrops(new SingleDrop("bk:heart", 0.05f));
		}
		
		public void Kill(Entity w) {
			GetComponent<HealthComponent>().Kill(w);
		}

		public override bool HandleEvent(Event e) {
			if (e is HealthModifiedEvent ev) {
				if (ev.Amount < 0) {
					GetAnyComponent<BodyComponent>()?.KnockbackFrom(ev.From);
					var c = GetBloodColor();
					
					if (Random.Chance(30)) {
						for (var i = 0; i < Random.Int(1, 3); i++) {
							Area.Add(new SplashParticle {
								Position = Center - new Vector2(2.5f),
								Color = c
							});
						}
					}

					Area.Add(new SplashFx {
						Position = Center,
						Color = ColorUtils.Mod(c)
					});
				}
				
				if (HasNoHealth(ev)) {
					Kill(ev.From);
				}
			} else if (e is DiedEvent) {
				HandleDeath();
			} else if (e is LostSupportEvent) {
				if (!(this is Player)) {
					Done = true;
					return true;
				}
			}

			return base.HandleEvent(e);
		}

		protected virtual void HandleDeath() {
			AnimateDeath();
		}

		public virtual void AnimateDeath() {
			GetComponent<DropsComponent>().SpawnDrops();
			Done = true;
				
			AnimationUtil.Poof(Center);
			
			for (var i = 0; i < Random.Int(2, 8); i++) {
				Area.Add(new SplashParticle {
					Position = Center - new Vector2(2.5f),
					Color = GetBloodColor()
				});
			}
		}

		protected void AddDrops(params Drop[] drops) {
			GetComponent<DropsComponent>().Add(drops);
		}

		public virtual bool HasNoHealth(HealthModifiedEvent e = null) {
			return GetComponent<HealthComponent>().Health == (-e?.Amount ?? 0);
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
			return !(entity is Creature || (InAir() && (entity is Chasm || entity is Item || entity is Bomb || (entity is DestroyableLevel && !ShouldCollideWithDestroyableInAir()))));
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
	}
}