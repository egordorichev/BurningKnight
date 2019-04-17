using System;
using System.Collections.Generic;
using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.level.entities;
using BurningKnight.level.paintings;
using BurningKnight.state;
using BurningKnight.util;
using Lens;
using Lens.entity;
using Lens.entity.component.logic;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

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

			for (int i = CollidingToHurt.Count - 1; i >= 0; i--) {
				var entity = CollidingToHurt[i];

				if (entity.Done) {
					CollidingToHurt.RemoveAt(i);
					continue;
				}

				if (TouchDamage > 0 && (!(entity is Creature c) || c.IsFriendly() != IsFriendly())) {
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
			}
			
			return base.HandleEvent(e);
		}

		protected virtual bool CanHurt(Entity entity) {
			return !(entity is BreakableProp || entity is Painting);
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
				var d = target.DistanceTo(this);

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

		public virtual bool SpawnsNearWall() {
			return false;
		}
		
		#region Path finding
		private Vec2 nextPathPoint;

		private void BuildPath(Vector2 to) {
			var level = Run.Level;
			var fp = level.ToIndex((int) Math.Round(CenterX / 16f), (int) Math.Round(CenterY / 16f));
			var tp = level.ToIndex((int) Math.Round(to.X / 16f), (int) Math.Round(to.Y / 16f));

			var p = PathFinder.GetStep(fp, tp, level.Passable);

			if (p == -1) {
				return;
			}
			
			nextPathPoint = new Vec2 {
				X = level.FromIndexX(p) * 16 + 8, 
				Y = level.FromIndexY(p) * 16 + 8
			};
		}
		
		public bool MoveTo(Vector2 point, float speed, float distance = 8f) {
			var ds = DistanceTo(point);
			
			if (ds <= distance) {
				return true;
			}
			
			if (nextPathPoint == null) {
				BuildPath(point);

				if (nextPathPoint == null) {
					return false;
				}
			}

			var dx = nextPathPoint.X - CenterX;
			var dy = nextPathPoint.Y - CenterY;
			var d = (float) Math.Sqrt(dx * dx + dy * dy);

			if (d <= 1f) {
				nextPathPoint = null;
				return false;
			}

			speed *= Engine.Delta * 60;
			GetAnyComponent<BodyComponent>().Velocity = new Vector2(dx / d * speed, dy / d * speed);

			return false;
		}
		#endregion
	}
}