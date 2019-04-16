using System;
using System.Collections.Generic;
using BurningKnight.entity.component;
using BurningKnight.level.tile;
using BurningKnight.state;
using BurningKnight.util;
using Lens.graphics;
using Lens.util;
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
			Become<IdleState>();

			// todo: rotate body
			Width = 16;
			Height = 16;

			var body = new RectBodyComponent(3, 4, 9, 16);
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
				return;
			}

			CenterX = x * 16 + 8;
			CenterY = y * 16;
			
			direction = dirs[Random.Int(dirs.Count)];
			var angle = direction.ToAngle();
			
			GetComponent<WallAnimationComponent>().Angle = angle;
		}

		public override void Render() {
			Graphics.Batch.FillRectangle(new RectangleF(X, Y, 16, 16), Color.Blue);
			base.Render();
		}

		#region Crawler States
		public class IdleState : MobState<WallCrawler> {
			
		}
		#endregion

		public override bool SpawnsNearWall() {
			return true;
		}
	}
}