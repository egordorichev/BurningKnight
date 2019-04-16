using System;
using System.Collections.Generic;
using BurningKnight.entity.component;
using BurningKnight.level.tile;
using BurningKnight.state;
using BurningKnight.util;
using Lens.graphics;
using Lens.util;
using Lens.util.file;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.creature.mob.castle {
	public class WallCrawler : Mob {
		private Direction direction;
		
		protected override void SetStats() {
			base.SetStats();
			
			AddComponent(new WallAnimationComponent("crawler"));
			SetMaxHp(2);

			// todo: rotate body
			Width = 16;
			Height = 16;

			var body = new RectBodyComponent(0, 0, 16, 16);
			AddComponent(body);
			body.KnockbackModifier = 0;
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
			
			GetComponent<WallAnimationComponent>().Angle = angle;
			Become<IdleState>();
		}

		public int tx;
		public int ty;

		public int ttx;
		public int tty;
		
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

				if (Random.Chance()) {
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

				if (T < 0.2f) {
					return;
				}
				
				var mx = Self.X + (this.mx + vx) * 16;
				var my = Self.CenterY + (this.my + vy) * 16;
							
				Self.tx = (int) Math.Round(mx / 16f);
				Self.ty = (int) Math.Round(my / 16f);

				if (!Run.Level.Get((int) Self.tx, (int) Self.ty).IsWall()) {
					Flip();
					return;
				}
				
				mx = Self.X + (vx) * 16;
				my = Self.CenterY + (vy) * 16;
							
				Self.ttx = (int) Math.Round(mx / 16f);
				Self.tty = (int) Math.Round(my / 16f);
				
				if (Run.Level.Get(Self.ttx, Self.tty).IsWall()) {
					Flip();
				}
			}
			
			private void Flip() {
				velocity *= -1;
				vx *= -1;
				vy *= -1;
				Self.GetComponent<RectBodyComponent>().Velocity = velocity;
				T = 0;
			}
		}
		#endregion

		public override void Render() {
			Graphics.Batch.FillRectangle(new RectangleF(tx * 16 - 1, ty * 16 - 9, 18, 18), Color.Red);
			Graphics.Batch.FillRectangle(new RectangleF(ttx * 16 - 1, tty * 16 - 9, 18, 18), Color.Blue);
			base.Render();
		}

		public override bool SpawnsNearWall() {
			return true;
		}
	}
}