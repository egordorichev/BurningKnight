using Lens.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lens.Util.Camera {
	public class Camera : Entity {
		public static Camera Instance;

		#region Camera logic

		public Entity Target;
		public bool Detached;
		private CameraDriver driver;

		public CameraDriver Driver {
			get { return driver; }

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

			Viewport = new Viewport();
			Viewport.Width = Display.Width;
			Viewport.Height = Display.Height;

			position = new Vector2();
			origin = new Vector2(Display.Width / 2f, Display.Height / 2f);

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
			return Vector2.Transform(position, inverse);
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
			get { return position; }

			set {
				position = value;
				changed = true;
			}
		}

		public Vector2 Origin {
			get { return origin; }

			set {
				origin = value;
				changed = true;
			}
		}

		public float Angle {
			get { return angle; }

			set {
				angle = value;
				changed = true;
			}
		}

		public float Zoom {
			get { return zoom; }

			set {
				zoom = value;
				changed = true;
			}
		}

		private void UpdateMatrices() {
			matrix = Matrix.Identity *
			         Matrix.CreateTranslation(new Vector3(
				         -new Vector2((int) System.Math.Floor(position.X), (int) System.Math.Floor(position.Y)), 0)) *
			         Matrix.CreateRotationZ(angle) *
			         Matrix.CreateScale(new Vector3(zoom, 1, 1)) *
			         Matrix.CreateTranslation(
				         new Vector3(new Vector2((int) System.Math.Floor(origin.X), (int) System.Math.Floor(origin.Y)), 0));

			inverse = Matrix.Invert(matrix);
			changed = false;
		}

		#endregion
	}
}