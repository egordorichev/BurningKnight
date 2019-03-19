using System;
using Lens.util;
using Microsoft.Xna.Framework;

namespace Lens.entity.component.logic {
	public class ShakeComponent : Component {
		public float Amount;
		public float Angle;
		public Vector2 Position;
		public float Time;
		
		// fixme: push

		public override void Update(float dt) {
			Time += dt * 10f;

			float a = Math.Min(5, Amount * Amount * 0.5f * 0.1f);
			Amount = Math.Max(0, Amount - dt * 15f);

			if (Amount < 0.01f) {
				Angle = 0;
				Position.X = 0;
				Position.Y = 0;
			} else {
				Angle = Noise.Generate(Time) * a * 0.01f;
				Position.X = Noise.Generate(Time + 32) * a;
				Position.Y = Noise.Generate(Time + 64) * a;	
			}
		}
	}
}