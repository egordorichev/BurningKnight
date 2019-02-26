using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;

namespace BurningKnight.entity.component {
	public class RectBodyComponent : BodyComponent {
		public RectBodyComponent(float x, float y, float w, float h, bool center = false) {
			if (center) {
				x -= w / 2;
				y -= h / 2;
			}
			
			var def = new BodyDef();
			ModDef(def);
			
			Body = Physics.World.CreateBody(def);
		
			var polyDef = new PolygonDef {
				VertexCount = 4,
				Vertices = new[] {
					new Vec2(x, y),
					new Vec2(x + w, y),
					new Vec2(x + w, y + h),
					new Vec2(x, y + h)
				}
			};

			ModPoly(polyDef);
			Body.CreateShape(polyDef);
		}

		public virtual void ModDef(BodyDef def) {
			
		}

		public virtual void ModPoly(PolygonDef def) {
			
		}
	}
}