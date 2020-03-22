using System;
using System.Collections.Generic;
using System.Linq;
using BurningKnight.assets.items;
using BurningKnight.assets.particle;
using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.drop;
using BurningKnight.entity.creature.mob.boss;
using BurningKnight.entity.creature.mob.prefix;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.entity.projectile;
using BurningKnight.level;
using BurningKnight.level.entities;
using BurningKnight.level.paintings;
using BurningKnight.level.rooms;
using BurningKnight.physics;
using BurningKnight.state;
using BurningKnight.ui.imgui;
using BurningKnight.util;
using ImGuiNET;
using Lens;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.graphics;
using Lens.util;
using Lens.util.camera;
using Lens.util.file;
using Lens.util.math;
using Lens.util.timer;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace BurningKnight.entity.creature.mob {
	public class Mob : Creature, DropModifier {
		public Entity Target;
		public bool HasPrefix => prefix != null;
		public Prefix Prefix => prefix;
		
		protected List<Entity> CollidingToHurt = new List<Entity>();
		protected int TouchDamage = 1;
		protected bool TargetEverywhere;
		
		private Prefix prefix;
		
		public override void AddComponents() {
			base.AddComponents();

			AlwaysActive = true;
			
			AddTag(Tags.Mob);
			AddTag(Tags.MustBeKilled);
			
			SetStats();
			
			AddDrops(new SingleDrop("bk:coin", 0.2f));
			AddDrops(new SingleDrop("bk:bomb", 0.03f));

			var h = GetComponent<HealthComponent>();
			h.InvincibilityTimerMax = 0.3f;
			h.PreventDamageInInvincibility = false;

			if (!(this is Boss)) {
				GetComponent<StateComponent>().Pause++;
			}
		}

		protected virtual void SetStats() {
			
		}

		protected void AddAnimation(string name, string layer = null) {
			AddComponent(new MobAnimationComponent(name, layer));
		}
		
		protected void SetMaxHp(int hp) {
			var health = GetComponent<HealthComponent>();
			health.InitMaxHealth = hp;
		}

		protected virtual void OnTargetChange(Entity target) {
			if (target == null) {
				GetComponent<StateComponent>().PauseOnChange = true;
			} else {
				GetComponent<StateComponent>().PauseOnChange = false;
				GetComponent<StateComponent>().Pause = 0;
			}
		}
		
		private float lastParticle;

		public override void Update(float dt) {
			base.Update(dt);

			if (prefix != null) {
				prefix.Update(dt);

				lastParticle -= dt;

				if (lastParticle <= 0) {
					lastParticle = Rnd.Float(0.05f, 0.3f);

					for (var i = 0; i < Rnd.Int(0, 3); i++) {
						var part = new ParticleEntity(Particles.Scourge());

						part.Position = Center + Rnd.Vector(-4, 4);
						part.Particle.Scale = Rnd.Float(0.5f, 1.2f);
						Area.Add(part);
						part.Depth = 1;
					}
				}
			}

			if (Target == null) {
				FindTarget();
			} else if (Target.Done || Target.GetComponent<RoomComponent>().Room != GetComponent<RoomComponent>().Room ||
			           (Target is Creature c && c.IsFriendly() == IsFriendly()) || 
			           (Target.TryGetComponent<BuffsComponent>(out var b) && b.Has<InvisibleBuff>())) {
				
				Target = null;
				FindTarget();
			}

			if (TouchDamage == 0) {
				return;
			}

			var raging = GetComponent<BuffsComponent>().Has<RageBuff>();
			
			for (var i = CollidingToHurt.Count - 1; i >= 0; i--) {
				var entity = CollidingToHurt[i];

				if (entity.Done) {
					CollidingToHurt.RemoveAt(i);
					continue;
				}

				if ((!(entity is Creature c) || c.IsFriendly() != IsFriendly())) {
					if (entity.GetComponent<HealthComponent>().ModifyHealth(-TouchDamage * (raging ? 2 : 1), this, DamageType.Contact)) {
						OnHit(entity);
					}
				}
			}

			if (GetComponent<RoomComponent>().Room == null) {
				Kill(null);
			}
		}

		protected virtual void OnHit(Entity e) {
			
		}

		public override bool HandleEvent(Event e) {
			if (prefix != null && prefix.HandleEvent(e)) {
				e.Handled = true;
			}
			
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
				var who = de.From;

				if (de.From != null) {
					if (de.From.TryGetComponent<OwnerComponent>(out var o)) {
						who = o.Owner;
					} else if (who is Projectile p) {
						who = p.Owner;
					}
				}
				
				GetComponent<RoomComponent>().Room?.CheckCleared(who);
			} else if (e is HealthModifiedEvent hme && hme.Amount < 0) {				
				if (!rotationApplied) {
					rotationApplied = true;
					var a = GetAnyComponent<AnimationComponent>();
				
					if (a != null) {
						var w = a.Angle;
						a.Angle += 0.5f;

						var t = Tween.To(w, a.Angle, x => a.Angle = x, 0.2f);
						
						t.Delay = 0.2f;
						t.OnEnd = () => {
							rotationApplied = false;
						};
					}
				}
			}
			
			return base.HandleEvent(e);
		}

		protected virtual bool CanHurt(Entity entity) {
			return !(entity is BreakableProp || entity is Painting || entity is Prop);
		}

		protected void FindTarget() {
			List<Entity> targets;

			if (TargetEverywhere) {
				targets = Area.Tagged[IsFriendly() ? Tags.Mob : Tags.Player];
			} else {
				var room = GetComponent<RoomComponent>().Room;

				if (room == null) {
					return;
				}
			
				targets = room.Tagged[IsFriendly() ? Tags.Mob : Tags.Player];
			}
			
			var closestDistance = float.MaxValue;
			var friendly = IsFriendly();
			
			Entity closest = null;
			
			foreach (var target in targets) {
				if (target == this || target is bk.BurningKnight || ((Creature) target).IsFriendly() == friendly || 
				    (target.TryGetComponent<BuffsComponent>(out var b) && b.Has<InvisibleBuff>())) {
					
					continue;
				}
				
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

		private bool rotationApplied;

		public override void AnimateDeath(DiedEvent d) {
			base.AnimateDeath(d);
			CreateGore(d);
		}

		#region Path finding
		protected Vec2 NextPathPoint;
		private int lastStepBack;
		private int prevStepBack;

		private void BuildPath(Vector2 to, bool back = false) {
			var level = Run.Level;
			var fp = level.ToIndex((int) Math.Floor(CenterX / 16f), (int) Math.Floor(Bottom / 16f));
			var tp = level.ToIndex((int) Math.Floor(to.X / 16f), (int) Math.Floor(to.Y / 16f));

			var p = back ? PathFinder.GetStepBack(fp, tp, level.Passable, prevStepBack) : PathFinder.GetStep(fp, tp, level.Passable);

			if (back) {
				prevStepBack = lastStepBack;
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
				var ds = DistanceToFromBottom(point);

				if (ds <= distance) {
					return true;
				}
			} else {
				var ds = DistanceToFromBottom(point);

				if (ds >= distance) {
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
			var dy = NextPathPoint.Y - Bottom;
			var d = (float) Math.Sqrt(dx * dx + dy * dy);

			if (d <= 2f) {
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

		public override void Load(FileReader stream) {
			base.Load(stream);
			var str = stream.ReadString();

			if (str != null) {
				SetPrefix(str);
			}
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteString(prefix?.Id);
		}

		public void GeneratePrefix() {
			if (!Rnd.Chance(Run.Scourge * 10 + 0.5f)) {
				return;
			}

			var all = PrefixRegistry.Defined.Keys.ToArray();
			SetPrefix(all[Rnd.Int(all.Length)]);
		}

		public void SetPrefix(string id) {
			if (!PrefixRegistry.Defined.TryGetValue(id, out var t)) {
				return;
			}

			try {
				var p = (Prefix) Activator.CreateInstance(t);

				prefix = p;
				
				p.Id = id;
				p.Mob = this;
				p.Init();
			} catch (Exception e) {
				Log.Error(e);
				return;
			}
		}

		public override void RenderImDebug() {
			base.RenderImDebug();
			
			ImGui.Text($"Target: {(Target == null ? "null" : Target.GetType().Name)}");

			if (Target != null) {
				if (ImGui.Button("Jump")) {
					WindowManager.Entities = true;
					AreaDebug.ToFocus = Target;
				}
			}
			
			ImGui.Text($"Prefix: {(Prefix == null ? "null" : Prefix.Id)}");
		}

		public override void RenderDebug() {
			base.RenderDebug();

			if (NextPathPoint != null) {
				Graphics.Batch.DrawLine(CenterX, Bottom, NextPathPoint.X, NextPathPoint.Y, Color.Red);
				Graphics.Batch.DrawLine(CenterX, Bottom, Run.Level.FromIndexX(prevStepBack) * 16 + 8, Run.Level.FromIndexY(prevStepBack) * 16 + 8, Color.Blue);
			}
		}

		private static bool RayShouldCollide(Entity entity) {
			return entity is ProjectileLevelBody;
		}

		protected bool CanSeeTarget() {
			if (Target == null) {
				return false;
			}
			
			var min = 1f;
			var found = false;
			
			Physics.World.RayCast((fixture, point, normal, fraction) => {
				if (min > fraction && fixture.Body.UserData is BodyComponent b && RayShouldCollide(b.Entity)) {
					min = fraction;
					found = true;
				}
				
				return min;
			}, Center, Target.Center);

			return !found;
		}
		
		protected void TurnToTarget() {
			if (Target != null) {
				GraphicsComponent.Flipped = Target.CenterX < CenterX;
			}
		}

		protected void PushFromOtherEnemies(float dt, Func<Creature, bool> filter = null) {
			var room = GetComponent<RoomComponent>().Room;
			var body = GetAnyComponent<BodyComponent>();

			if (room == null || body == null) {
				return;
			}
			
			foreach (var m in room.Tagged[Tags.Mob]) {
				if (m == this) {
					continue;
				}
				
				var mob = (Creature) m;
				
				if (filter != null && !filter(mob)) {
					return;
				}

				var dx = DxTo(mob);
				var dy = DyTo(mob);
				var d = MathUtils.Distance(dx, dy);
				var force = dt * 800;
				
				if (d <= 8) {
					var a = MathUtils.Angle(dx, dy) - (float) Math.PI;
					body.Velocity += new Vector2((float) Math.Cos(a) * force, (float) Math.Sin(a) * force);
				}
			}
		}

		protected void PushOthersFromMe(float dt, Func<Creature, bool> filter = null) {
			var room = GetComponent<RoomComponent>().Room;

			if (room == null) {
				return;
			}

			foreach (var m in room.Tagged[Tags.Mob]) {
				if (m == this) {
					continue;
				}

				var mob = (Creature) m;

				if (filter != null && !filter(mob)) {
					return;
				}

				var dx = DxTo(mob);
				var dy = DyTo(mob);
				var d = MathUtils.Distance(dx, dy);
				var force = dt * 800;

				if (d <= 12) {
					var a = MathUtils.Angle(dx, dy) - (float) Math.PI;
					var b = mob.GetAnyComponent<BodyComponent>();

					if (b != null) {
						b.Velocity -= new Vector2((float) Math.Cos(a) * force, (float) Math.Sin(a) * force);
					}
				}
			}
		}
		
		public void ModifyDrops(List<Item> drops) {
			if (Rnd.Chance(Run.Scourge * 0.5f)) {
				var c = Rnd.Int(0, 3);
				
				for (var i = 0; i < c; i++) {
					drops.Add(Items.Create("bk:copper_coin"));
				}
			}
		}
	}
}