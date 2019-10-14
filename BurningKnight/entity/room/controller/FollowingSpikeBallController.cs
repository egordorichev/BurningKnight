using System;
using BurningKnight.entity.room.controllable.spikes;
using Lens.util;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.room.controller {
	public class FollowingSpikeBallController : RoomController {
		private Vector2 ball;

		public override void Init() {
			base.Init();
			ball = Room.Center;
		}

		public override void Update(float dt) {
			var target = Room.Center;

			if (Room.Tagged[Tags.Player].Count > 0) {
				target = Room.Tagged[Tags.Player][0].Center;
			}

			var dx = target.X - ball.X;
			var dy = target.Y - ball.Y;
			var d = MathUtils.Distance(dx, dy);

			if (d > 1f) {
				var s = dt * 32;
				
				ball.X += dx / d * s;
				ball.Y += dy / d * s;
			}
			
			foreach (var c in Room.Controllable) {
				if (c is Spikes) {
					c.SetState(MathUtils.Distance(ball.X - c.CenterX, ball.Y - c.CenterY) < 28);
				}
			}
		}
	}
}