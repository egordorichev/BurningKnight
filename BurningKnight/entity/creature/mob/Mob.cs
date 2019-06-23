using System;
using System.Collections.Generic;
using BurningKnight.entity.buff;
using BurningKnight.entity.chest;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using BurningKnight.level.entities;
using BurningKnight.level.paintings;
using BurningKnight.state;
using BurningKnight.util;
using Lens;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.util;
using Lens.util.camera;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob {
	public class Mob : Creature {
		public Entity Target;

		protected List<Entity> CollidingToHurt = new List<Entity>();
		protected int TouchDamage = 1;
		
		public override void AddComponents() {
			base.AddComponents();

			AlwaysActive = true;
			
			AddTag(Tags.Mob);
			AddTag(Tags.MustBeKilled);
			
			SetStats();
			
			GetComponent<DropsComponent>().Add(new SimpleDrop {
				Chance = 0.05f,
				Items = new[] {
					"bk:coin"
				}
			});

			GetComponent<HealthComponent>().InvincibilityTimerMax = 0.2f;
		}

		protected virtual void SetStats() {
			
		}

		protected void AddAnimation(string name, string layer = null) {
			AddComponent(new AnimationComponent(name, layer));
		}
		
		protected void SetMaxHp(int hp) {
			var health = GetComponent<HealthComponent>();
			health.InitMaxHealth = hp;
		}

		protected void Become<T>() {
			GetComponent<StateComponent>().Become<T>();
		}

		protected virtual void OnTargetChange(Entity target) {
			
		}
		
		public override void Update(float dt) {
			base.Update(dt);

			if (Target == null) {
				FindTarget();
			} else if (Target.Done || Target.GetComponent<RoomComponent>().Room != GetComponent<RoomComponent>().Room) {
				Target = null;
				FindTarget();
			}

			if (TouchDamage == 0) {
				return;
			}
			
			for (int i = CollidingToHurt.Count - 1; i >= 0; i--) {
				var entity = CollidingToHurt[i];

				if (entity.Done) {
					CollidingToHurt.RemoveAt(i);
					continue;
				}

				if ((!(entity is Creature c) || c.IsFriendly() != IsFriendly())) {
					entity.GetComponent<HealthComponent>().ModifyHealth(-TouchDamage, this);
				}
			}
		}

		public override bool HandleEvent(Event e) {
			if (e is BuffAddedEvent add && add.Buff is CharmedBuff || e is BuffRemovedEvent del && del.Buff is CharmedBuff) {
				// If old target even was a thing, it was from wrong category
				FindTarget();
			} else if (e is CollisionStartedEvent collisionStart) {
				if (collisionStart.Entity.HasComponent<HealthComponent>() && CanHurt(collisionStart.Entity)) {
					CollidingToHurt.Add(collisionStart.Entity);
				}
			} else if (e is CollisionEndedEvent collisionEnd) {
				if (collisionEnd.Entity.HasComponent<HealthComponent>()) {
					CollidingToHurt.Remove(collisionEnd.Entity);
				}
			} else if (e is DiedEvent de) {
				var r = GetComponent<RoomComponent>().Room;

				if (r != null && !r.Cleared) {
					var found = false;
					
					foreach (var m in r.Tagged[Tags.MustBeKilled]) {
						if (m.GetComponent<HealthComponent>().Health > 0) {
							found = true;
							break;
						}
					}
					
					if (!found) {
						r.Cleared = true;
						var who = de.From;

						if (de.From.TryGetComponent<OwnerComponent>(out var o)) {
							who = o.Owner;
						} else if (who is Projectile p) {
							who = p.Owner;
						}

						who.HandleEvent(new RoomClearedEvent {
							Room = r
						});
					}
				}
			}
			
			return base.HandleEvent(e);
		}

		protected virtual bool CanHurt(Entity entity) {
			return !(entity is BreakableProp || entity is Painting || entity is Chest || entity is Prop);
		}

		protected void FindTarget() {
			var room = GetComponent<RoomComponent>().Room;

			if (room == null) {
				return;
			}
			
			var targets = room.Tagged[IsFriendly() ? Tags.Mob : Tags.Player];
			var closestDistance = float.MaxValue;
			Entity closest = null;
			
			foreach (var target in targets) {
				var d = target.DistanceToSquared(this);

				if (d < closestDistance) {
					closestDistance = d;
					closest = target;
				}
			}

			if (Target != closest) {
				HandleEvent(new MobTargetChange {
					Mob = this,
					New = closest,
					Old = Target 
				});
			}			
			
			// Might be null, thats ok
			Target = closest;
			OnTargetChange(closest);
		}

		public override bool IsFriendly() {
			return GetComponent<BuffsComponent>().Has<CharmedBuff>();
		}

		public virtual float GetWeight() {
			return 1f;
		}

		public virtual bool CanSpawnMultiple() {
			return true;
		}
		
		public virtual bool SpawnsNearWall() {
			return false;
		}

		public virtual float GetSpawnChance() {
			return 1f;
		}

		public override void AnimateDeath() {
			base.AnimateDeath();
			
			Camera.Instance.Shake(4);
			Engine.Instance.Freeze = 0.5f;
		}

		#region Path finding
		protected Vec2 NextPathPoint;
		private int lastStepBack;

		private void BuildPath(Vector2 to, bool back = false) {
			var level = Run.Level;
			var fp = level.ToIndex((int) Math.Floor(CenterX / 16f), (int) Math.Floor(CenterY / 16f));
			var tp = level.ToIndex((int) Math.Floor(to.X / 16f), (int) Math.Floor(to.Y / 16f));

			var p = back ? PathFinder.GetStepBack(fp, tp, level.Passable, lastStepBack) : PathFinder.GetStep(fp, tp, level.Passable);

			if (back) {
				lastStepBack = p;
			}
			
			if (p == -1) {
				return;
			}
			
			NextPathPoint = new Vec2 {
				X = level.FromIndexX(p) * 16 + 8, 
				Y = level.FromIndexY(p) * 16 + 8
			};
		}
		
		public bool MoveTo(Vector2 point, float speed, float distance = 8f, bool back = false) {
			if (!back) {
				var ds = DistanceToSquared(point);

				if (ds <= distance) {
					return true;
				}
			}

			if (NextPathPoint == null) {
				BuildPath(point, back);

				if (NextPathPoint == null) {
					return false;
				}
			}

			var dx = NextPathPoint.X - CenterX;
			var dy = NextPathPoint.Y - CenterY;
			var d = (float) Math.Sqrt(dx * dx + dy * dy);

			if (d <= 1f) {
				NextPathPoint = null;
				return false;
			}

			speed *= Engine.Delta * 60;
			GetAnyComponent<BodyComponent>().Velocity = new Vector2(dx / d * speed, dy / d * speed);

			return false;
		}

		public bool FlyTo(Vector2 point, float speed, float distance = 8f) {
			var dx = DxTo(point);
			var dy = DyTo(point);
			var d = (float) Math.Sqrt(dx * dx + dy * dy);

			if (d <= distance) {
				return true;
			}
			
			GetAnyComponent<BodyComponent>().Velocity = new Vector2(dx / d * speed, dy / d * speed);

			return false;
		}
		#endregion
	}
}