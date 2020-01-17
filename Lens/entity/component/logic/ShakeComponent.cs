using System;
using Lens.util;
using Microsoft.Xna.Framework;

namespace Lens.entity.component.logic {
	public class ShakeComponent : Component {
		public static float Modifier = 1;

		public float Amount;
		public float Angle;
		public Vector2 Position;
		public Vector2 PushDirection;
		public float Push;
		public float Time;
		
		public override void Update(float dt) {
			Time += dt * 10f;

			if (Amount <= 0.1f) {
				Angle = 0;
				Position.X = 0;
				Position.Y = 0;
			} else {
				var a = Math.Min(4, Amount * Amount * 0.07f) * Modifier;

				Angle = Noise.Generate(Time) * a * 0.01f;
				Position.X = Noise.Generate(Time * 2f + 32) * a;
				Position.Y = Noise.Generate(Time * 2f + 64) * a;	
			}

			if (Push >= 0.01f) {
				var force = Push * Push * 1.2f * (Modifier * 0.3f);
				Position.X += PushDirection.X * force;
				Position.Y += PushDirection.Y * force;
			}
			
			Amount = Math.Max(0, Amount - dt * 10f);
			Push = Math.Max(0, Push - dt * 20f);
		}
	}
}