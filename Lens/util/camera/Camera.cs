using System;
using System.Collections.Generic;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Lens.util.camera {
	public class Camera : Entity {
		public static bool Debug = true;
		public static Camera Instance;

		public Vector2 TopLeft => new Vector2(X, Y);
		public new float X => position.X - Width / 2;
		public new float Y => position.Y - Height / 2;
		public new float Right => position.X + Width / 2;
		public new float Bottom => position.Y + Height / 2;
		
		#region Camera logic

		public class Target {
			public float Priority;
			public Entity Entity;
			public bool Inside;

			public Target(Entity entity, float priority) {
				Entity = entity;
				Priority = priority;
			}
		}
		
		public List<Target> Targets = new List<Target>();
		public bool Detached;
		public float TextureZoom = 1f;
		
		private CameraDriver driver;
		
		public CameraDriver Driver {
			get => driver;

			set {
				driver?.Destroy();
				driver = value;
				driver.Camera = this;
				driver?.Init();
			}
		}

		public Camera(CameraDriver driver) {
			Instance = this;
			Driver = driver;

			Width = Display.Width;
			Height = Display.Height;

			Viewport = new Viewport();
			Viewport.Width = Display.Width;
			Viewport.Height = Display.Height;

			origin = new Vector2(Display.Width / 2f, Display.Height / 2f);

			AlwaysActive = true;
			AlwaysVisible = true;

			changed = true;
		}

		public void Shake(float a = 1f) {
			var component = GetComponent<ShakeComponent>();
			component.Amount = Math.Min(component.Amount + a, 20f);
		}

		public override void AddComponents() {
			base.AddComponents();
			AddComponent(new ShakeComponent());
		}

		public void Follow(Entity entity, float priority) {
			Targets.Add(new Target(entity, priority));
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Engine.Instance.State.Paused) {
				return;
			}
			
			for (int i = Targets.Count - 1; i >= 0; i--) {
				if (Targets[i].Entity.Done) {
					Targets.RemoveAt(i);
				}
			}

			if (!Detached) {
				driver?.Update(dt);
			}
			
			if (changed || GetComponent<ShakeComponent>().Amount > 0.001f) {
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
				Position = Vector2.Normalize(move) * maxDistance;
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
		
		public float PositionX {
			get => position.X;

			set {
				position.X = value;
				changed = true;
			}
		}
		
		public float PositionY {
			get => position.Y;

			set {
				position.Y = value;
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
			var shake = GetComponent<ShakeComponent>();
			
			matrix = Matrix.Identity *
				Matrix.CreateTranslation(new Vector3(
				 -new Vector2((int) System.Math.Floor(position.X + shake.Position.X), (int) System.Math.Floor(position.Y + shake.Position.Y)), 0)) *
				Matrix.CreateRotationZ(angle) *
				Matrix.CreateScale(new Vector3(zoom, 1, 1)) *
				Matrix.CreateTranslation(
				 new Vector3(new Vector2((int) System.Math.Floor(origin.X), (int) System.Math.Floor(origin.Y)), 0));

			inverse = Matrix.Invert(matrix);
			changed = false;
		}

		#endregion
		
		private static Color DebugColor = new Color(1, 1, 1, 0.5f);

		public void Jump() {
			position.X = 0;
			position.Y = 0;

			foreach (var follow in Targets) {
				position.X += follow.Priority * follow.Entity.CenterX;
				position.Y += follow.Priority * follow.Entity.CenterY;
			}

			changed = true;
		}
		
		public override void RenderDebug() {
			if (!Debug) {
				return;
			}
			
			// Graphics.Batch.DrawRectangle(new RectangleF(position.X - Display.Width / 2f, position.Y - Display.Height / 2f, Display.Width, Display.Height), Color.Wheat);
			Graphics.Batch.DrawRectangle(new RectangleF(Display.UiWidth / 2 - 4, Display.UiHeight / 2 - 4, 8, 8), DebugColor);

			for (int x = 1; x < 3; x++) {
				float xx = x * Display.UiWidth / 3f;
				Graphics.Batch.DrawLine(new Vector2(xx, 0), new Vector2(xx, Display.UiHeight), DebugColor);
			}
			
			for (int y = 1; y < 3; y++) {
				float yy = y * Display.UiHeight / 3f;
				Graphics.Batch.DrawLine(new Vector2(0, yy), new Vector2(Display.UiWidth, yy), DebugColor);
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