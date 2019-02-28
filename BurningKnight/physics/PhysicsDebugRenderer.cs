using System;
using System.Diagnostics;
using Lens.graphics;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using VelcroPhysics.Collision.Shapes;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Extensions.DebugView;
using VelcroPhysics.Shared;
using VelcroPhysics.Utilities;

namespace BurningKnight.physics {
	public class PhysicsDebugRenderer : DebugViewBase {
		public Color DefaultShapeColor = new Color(0.9f, 0.7f, 0.7f);
		public Color InactiveShapeColor = new Color(0.5f, 0.5f, 0.3f);
		public Color KinematicShapeColor = new Color(0.5f, 0.5f, 0.9f);
		public Color SleepingShapeColor = new Color(0.6f, 0.6f, 0.6f);
		public Color StaticShapeColor = new Color(0.5f, 0.9f, 0.5f);
		
		public PhysicsDebugRenderer(World world) : base(world) {
			AppendFlags(DebugViewFlags.Shape);
		}
		
		public void DrawPolygon(Vector2[] vertices, int count, Color color) {
			Graphics.Batch.DrawPolygon(Vector2.Zero, vertices, color);
		}

		public void DrawSolidPolygon(Vector2[] vertices, int count, Color color) {
			Graphics.Batch.DrawPolygon(Vector2.Zero, vertices, color);
		}

		public void DrawCircle(Vector2 center, float radius, Color color) {
			Graphics.Batch.DrawCircle(center, radius, 32, color);
		}

		public void DrawSolidCircle(Vector2 center, float radius, Vector2 axis, Color color) {
			Graphics.Batch.DrawCircle(center, radius, 32, color);
		}
		
		public override void DrawPolygon(Vector2[] vertices, int count, float red, float blue, float green, bool closed = true) {
			Graphics.Batch.DrawPolygon(Vector2.Zero, vertices, new Color(red, blue, green));
		}

		public override void DrawSolidPolygon(Vector2[] vertices, int count, float red, float blue, float green) {
			Graphics.Batch.DrawPolygon(Vector2.Zero, vertices, new Color(red, blue, green));
		}

		public override void DrawCircle(Vector2 center, float radius, float red, float blue, float green) {
			Graphics.Batch.DrawCircle(center, radius, 32, new Color(red, blue, green));
		}

		public override void DrawSolidCircle(Vector2 center, float radius, Vector2 axis, float red, float blue, float green) {
			Graphics.Batch.DrawCircle(center, radius, 32, new Color(red, blue, green));
		}

		public override void DrawSegment(Vector2 start, Vector2 end, float red, float blue, float green) {
			throw new NotImplementedException();
		}

		public override void DrawTransform(ref Transform transform) {
			throw new NotImplementedException();
		}

		public void DrawDebugData() {
			if ((Flags & DebugViewFlags.Shape) == DebugViewFlags.Shape) {
				foreach (var b in World.BodyList) {
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
		}

		public void DrawShape(Fixture fixture, Transform xf, Color color) {
			switch (fixture.Shape.ShapeType) {
				case ShapeType.Circle: {
					var circle = (CircleShape) fixture.Shape;

					Vector2 center = MathUtils.Mul(ref xf, circle.Position);
					float radius = circle.Radius;
					Vector2 axis = MathUtils.Mul(xf.q, new Vector2(1.0f, 0.0f));

					DrawSolidCircle(center, radius, axis, color);
					break;
				}

				case ShapeType.Polygon: {
					PolygonShape poly = (PolygonShape) fixture.Shape;
					int vertexCount = poly.Vertices.Count;
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