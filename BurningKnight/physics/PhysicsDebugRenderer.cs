using System.Diagnostics;
using Lens.graphics;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using VelcroPhysics.Collision.Shapes;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Shared;
using VelcroPhysics.Utilities;

namespace BurningKnight.physics {
	public class PhysicsDebugRenderer {
		private const float alpha = 0.7f;

		public Color DefaultShapeColor = new Color(0.9f, 0.7f, 0.7f, alpha);
		public Color InactiveShapeColor = new Color(0.5f, 0.5f, 0.3f, alpha);
		public Color KinematicShapeColor = new Color(0.5f, 0.5f, 0.9f, alpha);
		public Color SleepingShapeColor = new Color(0.6f, 0.6f, 0.6f, alpha);
		public Color StaticShapeColor = new Color(0.5f, 0.9f, 0.5f, alpha);
		
		private World world;

		public PhysicsDebugRenderer(World world) {
			this.world = world;
		}
		
		public void DrawSolidPolygon(Vector2[] vertices, int count, Color color) {
			Graphics.Batch.DrawPolygon(Vector2.Zero, vertices, color);
		}

		public void DrawSolidCircle(Vector2 center, float radius, Vector2 axis, Color color) {
			Graphics.Batch.DrawCircle(center, radius, 32, color);
		}

		public void DrawDebugData() {
			foreach (var b in world.BodyList) {
				b.GetTransform(out var xf);
				
				foreach (var f in b.FixtureList) {
					if (b.Enabled == false) {
						DrawShape(f, xf, InactiveShapeColor);
					} else if (b.BodyType == BodyType.Static) {
						DrawShape(f, xf, StaticShapeColor);
					} else if (b.BodyType == BodyType.Kinematic) {
						DrawShape(f, xf, KinematicShapeColor);
					} else if (b.Awake == false) {
						DrawShape(f, xf, SleepingShapeColor);
					} else {
						DrawShape(f, xf, DefaultShapeColor);
					}
				}
			}
		}

		public void DrawShape(Fixture fixture, Transform xf, Color color) {
			// todo: dont draw if offscreen
			switch (fixture.Shape.ShapeType) {
				case ShapeType.Circle: {
					var circle = (CircleShape) fixture.Shape;

					var center = MathUtils.Mul(ref xf, circle.Position);
					var radius = circle.Radius;
					var axis = MathUtils.Mul(xf.q, new Vector2(1.0f, 0.0f));

					DrawSolidCircle(center, radius, axis, color);
					break;
				}

				case ShapeType.Polygon: {
					var poly = (PolygonShape) fixture.Shape;
					var vertexCount = poly.Vertices.Count;
					Debug.Assert(vertexCount <= VelcroPhysics.Settings.MaxPolygonVertices);
					var _tempVertices = new Vector2[vertexCount];

					for (var i = 0; i < vertexCount; ++i) {
						_tempVertices[i] = MathUtils.Mul(ref xf, poly.Vertices[i]);
					}

					DrawSolidPolygon(_tempVertices, vertexCount, color);
					break;
				}
			}
		}
	}
}