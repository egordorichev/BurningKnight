using System;
using BurningKnight.util;
using Lens;
using Microsoft.Xna.Framework;

namespace BurningKnight.assets.lighting {
	public abstract class Light {
		public readonly byte Id;

		private bool dirty;
		private float radius = 32;

		public float Radius {
			get => radius;

			set {
				radius = value;
				dirty = true;
			}
		}

		public Color Color;
		public Vector2 Scale = Vector2.One;
		
		public Light(byte id) {
			Id = id;
			dirty = true;
		}

		public void Update(float dt) {
			if (dirty) {
				Scale.X = radius / 128f;
				Scale.Y = Scale.X;
			}
		}
		
		public abstract Vector2 GetPosition();
	}
}