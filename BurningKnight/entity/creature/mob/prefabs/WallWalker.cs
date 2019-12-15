using System;
using System.Collections.Generic;
using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.castle;
using BurningKnight.entity.door;
using BurningKnight.entity.events;
using BurningKnight.level.entities;
using BurningKnight.level.rooms;
using BurningKnight.level.tile;
using BurningKnight.state;
using BurningKnight.util;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.util;
using Lens.util.file;
using Lens.util.math;
using Microsoft.Xna.Framework;
using SharpDX.Direct3D11;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.mob.prefabs {
	public class WallWalker : Mob {
		protected internal Direction Direction;
		public bool Left;
		public float T;
		public bool Inited;

		public override bool InAir() {
			return true;
		}

		protected override void SetStats() {
			base.SetStats();
			
			Width = 16;
			Height = 16;
			Left = Rnd.Chance();
			
			GetComponent<BuffsComponent>().AddImmunity<FrozenBuff>();
		}

		private bool locked;
		
		private void LockToWall() {
			var dirs = new List<Direction>();
			
			var x = (int) Math.Round(X / 16f);
			var y = (int) Math.Round((Y + 8) / 16f);
			var level = Run.Level;

			if (level.Get(x + 1, y).IsWall()) {
				dirs.Add(Direction.Left);
			}
			
			if (level.Get(x - 1, y).IsWall()) {
				dirs.Add(Direction.Right);
			}
			
			if (level.Get(x, y + 1).IsWall()) {
				dirs.Add(Direction.Up);
			}
			
			if (level.Get(x, y - 1).IsWall()) {
				dirs.Add(Direction.Down);
			}

			if (dirs.Count == 0) {
				Log.Error("No walls to lock to!");
				Done = true;
				return;
			}

			CenterX = x * 16 + 8;
			CenterY = y * 16;
			
			Direction = dirs[Rnd.Int(dirs.Count)];
			var angle = Direction.ToAngle();
			
			var v = Direction == Direction.Up || Direction == Direction.Down;
			var body = new RectBodyComponent(Direction == Direction.Left ? 8 : 0, Direction == Direction.Up ? 8 : 0, v ? 16 : 8, v ? 8 : 16, BodyType.Dynamic, true);

			AddComponent(body);
			body.KnockbackModifier = 0;
			
			GetComponent<WallAnimationComponent>().WallAngle = angle;
			GetComponent<StateComponent>().State = GetIdleState();
		}

		public override void Save(FileWriter stream) {
			var y = Y;
				
			X = (int) Math.Floor(X / 16f) * 16;
			Y = (int) Math.Floor(y / 16f) * 16 + 8;
			
			base.Save(stream);

			Y = y;
		}

		protected virtual float GetSpeed() {
			return 30f;
		}
		
		#region Walker States
		public class IdleState : SmartState<WallWalker> {
			protected Vector2 velocity;
			protected float vx;
			protected float vy;
			protected int mx;
			protected int my;

			public void ResetVelocity() {
				velocity = Vector2.Zero;
				vx = 0;
				vy = 0;
				Self.GetComponent<RectBodyComponent>().Velocity = velocity;
			}

			public void InvertVelocity() {
				Self.Left = !Self.Left;
				velocity *= -1;
				vx *= -1;
				vy *= -1;
				Self.GetComponent<RectBodyComponent>().Velocity = velocity;
				T = 0;
			}

			public override void Init() {
				base.Init();
				
				if (!Self.Inited) {
					Self.Inited = true;
					T = Rnd.Float(1f);
				}

				var f = Self.GetSpeed();
				var a = Self.Direction.ToAngle();

				if (Self.Left) {
					a += (float) Math.PI;
				}
				
				vx = (float) Math.Sin(a);
				vy = (float) Math.Cos(a);
				
				velocity.X = vx * f;
				velocity.Y = vy * f;
				
				mx = Self.Direction.GetMx();
				my = Self.Direction.GetMy();
			}

			public override void Update(float dt) {
				base.Update(dt);
				
				var mx = Self.X + (this.mx) * 16;
				var my = Self.CenterY + (this.my) * 16;

				if (!Run.Level.Get((int) Math.Round(mx / 16f), (int) Math.Round(my / 16f)).IsWall()) {
					Self.GetComponent<HealthComponent>().Kill(Self);
					//return;
				}
				
				mx = Self.X + (this.mx + vx * 0.5f) * 16;
				my = Self.CenterY + (this.my + vy * 0.5f) * 16;
							
				var tx = (int) Math.Round(mx / 16f);
				var ty = (int) Math.Round(my / 16f);

				if (!Run.Level.Get(tx, ty).IsWall()) {
					Flip();
					return;
				}
				
				mx = Self.X + (vx) * 12;
				my = Self.CenterY + (vy) * 12;
							
				tx = (int) Math.Round(mx / 16f);
				ty = (int) Math.Round(my / 16f);
				
				if (Run.Level.Get(tx, ty).IsWall()) {
					Flip();
					return;
				}

				DoLogic(dt);
				Self.GetComponent<RectBodyComponent>().Velocity = velocity;
			}

			public virtual void DoLogic(float dt) {
				
			}
			
			public virtual void Flip() {
				InvertVelocity();
			}
		}
		#endregion

		protected virtual Type GetIdleState() {
			return typeof(IdleState);
		}

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent ev) {
				var en = ev.Entity;

				if (en is Door || (en is SolidProp && !(en is BreakableProp))) {
					var state = GetComponent<StateComponent>().StateInstance;

					if (state is IdleState s) {
						s.Flip();
					}
				}
			}
			
			return base.HandleEvent(e);
		}
		
		public override void Update(float dt) {
			if (Done) {
				return;
			}
			
			if (!locked) {
				locked = true;
				LockToWall();
			}
			
			base.Update(dt);
			T += dt;

			var room = GetComponent<RoomComponent>().Room;

			if (room == null) {
				return;
			}

			if (room.Type != RoomType.Regular) {
				Done = true;
				return;
			}

			if (room.Tagged[Tags.Player].Count == 0) {
				return;
			}

			foreach (var m in room.Tagged[Tags.MustBeKilled]) {
				if (m is Mob && !(m is WallWalker || m is bk.BurningKnight)) {
					return;
				}
			}

			Kill(this);
		}
	}
}