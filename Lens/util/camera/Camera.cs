using System.Collections.Generic;
using Lens.entity;
using Lens.graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Lens.util.camera {
	public class Camera : Entity {
		public static Camera Instance;

		public new float X => position.X - Width / 2;
		public new float Y => position.Y - Height / 2;
		public new float Right => position.X + Width / 2;
		public new float Bottom => position.Y + Height / 2;
		
		#region Camera logic

		public class Target {
			public float Priority;
			public Entity Entity;

			public Target(Entity entity, float priority) {
				Entity = entity;
				Priority = priority;
			}
		}
		
		public List<Target> Targets = new List<Target>();
		public bool Detached;
		private CameraDriver driver;

		public void Follow(Entity entity, float priority) {
			Targets.Add(new Target(entity, priority));
		}

		public CameraDriver Driver {
			get => driver;

			set {
				driver?.Destroy();
				driver = value;
				driver.Camera = this;
				driver?.Init();
			}
		}

		public Camera() {
			Instance = this;
			Driver = new FollowingDriver();

			Width = Display.Width;
			Height = Display.Height;

			Viewport = new Viewport();
			Viewport.Width = Display.Width;
			Viewport.Height = Display.Height;

			origin = new Vector2(Display.Width / 2f, Display.Height / 2f);

			AlwaysActive = true;
			AlwaysVisible = true;
			
			UpdateMatrices();
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (!Detached) {
				driver?.Update(dt);
			}

			if (changed) {
				UpdateMatrices();
			}
		}

		public override void Destroy() {
			base.Destroy();
			driver?.Destroy();
			Instance = null;
		}

		public void RoundPosition() {
			position.X = (float) System.Math.Round(position.X);
			position.Y = (float) System.Math.Round(position.Y);
			changed = true;
		}

		public Vector2 ScreenToCamera(Vector2 position) {
			return Vector2.Transform(position, Matrix.Invert(Engine.ScreenMatrix) * inverse);
		}

		public Vector2 CameraToScreen(Vector2 position) {
			return Vector2.Transform(position, matrix);
		}

		public void Approach(Vector2 position, float ease) {
			Position += (position - Position) * ease;
		}

		public void Approach(Vector2 position, float ease, float maxDistance) {
			var move = (position - Position) * ease;
			
			if (move.Length() > maxDistance) {
				Position += Vector2.Normalize(move) * maxDistance;
			} else {
				Position += move;
			}
		}

		#endregion

		#region Camera matrix

		public Viewport Viewport;

		private Matrix matrix = Matrix.Identity;
		private Matrix inverse = Matrix.Identity;
		private Vector2 position;
		private Vector2 origin;
		private float angle;
		private float zoom = 1f;
		private bool changed;

		public Matrix Matrix => matrix;
		
		public new Vector2 Position {
			get => position;

			set {
				position = value;
				changed = true;
			}
		}

		public Vector2 Origin {
			get => origin;

			set {
				origin = value;
				changed = true;
			}
		}

		public float Angle {
			get => angle;

			set {
				angle = value;
				changed = true;
			}
		}

		public float Zoom {
			get => zoom;

			set {
				zoom = value;
				changed = true;
			}
		}

		private void UpdateMatrices() {			
			matrix = Matrix.Identity *
				Matrix.CreateTranslation(new Vector3(
				 -new Vector2((int) System.Math.Round(position.X), (int) System.Math.Round(position.Y)), 0)) *
				Matrix.CreateRotationZ(angle) *
				Matrix.CreateScale(new Vector3(zoom, 1, 1)) *
				Matrix.CreateTranslation(
				 new Vector3(new Vector2((int) System.Math.Round(origin.X), (int) System.Math.Round(origin.Y)), 0));

			inverse = Matrix.Invert(matrix);
			changed = false;
		}

		#endregion
		
		private static Color DebugColor = new Color(1, 1, 1, 0.5f);

		public override void RenderDebug() {
			if (true) {
				return;
			}
			// Graphics.Batch.DrawRectangle(new RectangleF(position.X - Display.Width / 2f, position.Y - Display.Height / 2f, Display.Width, Display.Height), Color.Wheat);
			Graphics.Batch.DrawRectangle(new RectangleF(Display.Width / 2 - 4, Display.Height / 2 - 4, 8, 8), DebugColor);

			for (int x = 1; x < 3; x++) {
				float xx = x * Display.Width / 3f;
				Graphics.Batch.DrawLine(new Vector2(xx, 0), new Vector2(xx, Display.Height), DebugColor);
			}
			
			for (int y = 1; y < 3; y++) {
				float yy = y * Display.Height / 3f;
				Graphics.Batch.DrawLine(new Vector2(0, yy), new Vector2(Display.Width, yy), DebugColor);
			}
		}
		
		public override bool Overlaps(Entity entity) {
			return !(entity.X > Right ||
			         entity.Right < X ||
			         entity.Y > Bottom ||
			         entity.Bottom < Y);
		}

		public override bool Contains(Entity entity) {
			return entity.X >= X && entity.Right <= Right
			                     && entity.Y >= Y && entity.Bottom <= Bottom;
		}

		public override bool Contains(Vector2 point) {
			return point.X >= X && point.X <= Right
			                    && point.Y >= Y && point.Y <= Bottom;
		}
	}
}