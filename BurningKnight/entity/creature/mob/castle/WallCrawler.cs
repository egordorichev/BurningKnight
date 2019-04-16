using System;
using System.Collections.Generic;
using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.entity.door;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using BurningKnight.level.entities;
using BurningKnight.level.tile;
using BurningKnight.state;
using BurningKnight.util;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.util;
using Lens.util.file;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.creature.mob.castle {
	public class WallCrawler : Mob {
		private Direction direction;
		public bool Left;
		public float T;
		
		/*
		 * TODO:
		 *
		 * proper body rotation
		 * melee arc reflect bullets
		 * a bit of randomisation in init?
		 * fix exceptions when loading
		 */
		
		protected override void SetStats() {
			base.SetStats();
			
			AddComponent(new WallAnimationComponent("crawler"));
			SetMaxHp(2);

			Width = 16;
			Height = 16;

			var body = new RectBodyComponent(0, 8, 16, 8, BodyType.Dynamic, true);
			AddComponent(body);
			body.KnockbackModifier = 0;

			Left = Random.Chance();
		}

		public override void PostInit() {
			base.PostInit();
			LockToWall();
		}
		
		private void LockToWall() {
			var dirs = new List<Direction>();
			
			var x = (int) Math.Floor(X / 16f);
			var y = (int) Math.Floor((Y + 8) / 16f);
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
			
			direction = dirs[Random.Int(dirs.Count)];
			var angle = direction.ToAngle();

			GetComponent<RectBodyComponent>().Body.Rotation = (float) (angle + Math.PI / 2f);
			GetComponent<WallAnimationComponent>().Angle = angle;
			Become<IdleState>();
		}

		public override void Save(FileWriter stream) {
			var y = Y;
				
			X = (int) Math.Floor(X / 16f) * 16;
			Y = (int) Math.Floor(y / 16f) * 16;
			
			base.Save(stream);

			Y = y;
		}
		
		#region Crawler States
		public class IdleState : MobState<WallCrawler> {
			private Vector2 velocity;
			private float vx;
			private float vy;
			private int mx;
			private int my;
			
			public override void Init() {
				base.Init();

				var f = 30f;
				var a = Self.direction.ToAngle();

				if (Self.Left) {
					a += (float) Math.PI;
				}
				
				vx = (float) Math.Sin(a);
				vy = (float) Math.Cos(a);
				
				velocity.X = vx * f;
				velocity.Y = vy * f;

				Self.GetComponent<RectBodyComponent>().Velocity = velocity;
				
				mx = Self.direction.GetMx();
				my = Self.direction.GetMy();
			}

			public override void Update(float dt) {
				base.Update(dt);
				
				var mx = Self.X + (this.mx) * 16;
				var my = Self.CenterY + (this.my) * 16;

				if (!Run.Level.Get((int) Math.Round(mx / 16f), (int) Math.Round(my / 16f)).IsWall()) {
					Self.GetComponent<HealthComponent>().Kill(Self);
					return;
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
				
				Self.GetComponent<RectBodyComponent>().Velocity = velocity;

				if (T >= 3f) {
					Become<FireState>();
				}
			}
			
			public void Flip() {
				Self.Left = !Self.Left;

				if (Self.T >= 3f) {
					Become<FireState>();
					return;
				}

				velocity *= -1;
				vx *= -1;
				vy *= -1;
				Self.GetComponent<RectBodyComponent>().Velocity = velocity;
				T = 0;
			}
		}

		public class FireState : MobState<WallCrawler> {
			private bool fired;
			
			public override void Init() {
				base.Init();

				Self.T = 0;
				
				Self.GetComponent<RectBodyComponent>().Velocity = Vector2.Zero;
				Self.GetComponent<WallAnimationComponent>().SetAutoStop(true);
			}

			public override void Destroy() {
				base.Destroy();
				Self.GetComponent<WallAnimationComponent>().SetAutoStop(false);
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (!fired && Self.GetComponent<WallAnimationComponent>().Animation.Paused) {
					fired = true;
					T = 0;
					
					var angle = Self.direction.ToAngle();
					var projectile = Projectile.Make(Self, "small", angle, 30f);

					projectile.AddLight(32f, Color.Red);
				} else if (fired && T > 0.2f) {
					Self.GetComponent<StateComponent>().Become<IdleState>();
				}
			}
		}
		#endregion

		public override bool SpawnsNearWall() {
			return true;
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
			base.Update(dt);
			T += dt;
		}
	}
}