using BurningKnight.entity;
using BurningKnight.entity.component;
using Lens;
using Lens.entity;
using Lens.graphics;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.assets.particle.custom {
	public class TileParticle : Entity {
		private static Vector2 origin = new Vector2(8, 24);
		private static Vector2 originB = new Vector2(8, 8);
		public static float MaxZ = Display.Height;
		
		public TextureRegion Top;
		public TextureRegion Side;
		public TextureRegion Sides;
		public Vector2 Scale = new Vector2(0, 3);
		public float Z;

		public override void AddComponents() {
			base.AddComponents();

			AlwaysActive = true;
			Z = MaxZ;
			Depth = Layers.FlyingMob;
			
			Tween.To(1f, Scale.X, x => Scale.X = x, 0.4f);
			Tween.To(1f, Scale.Y, x => Scale.Y = x, 0.4f);

			Tween.To(0, Z, x => Z = x, 0.5f, Ease.QuadIn).OnEnd = () => {
				Scale.X = 2;
				Scale.Y = 0.5f;

				Tween.To(1, Scale.X, x => Scale.X = x, 0.5f);
				Tween.To(1, Scale.Y, x => Scale.Y = x, 0.5f);
			};
			
			AddComponent(new ShadowComponent(RenderShadow));
		}

		public void RenderShadow() {
			var or = Top.Center;
			Graphics.Render(Top, Position + or + new Vector2(0, 16), 0, or, Scale);
		}

		public override void Render() {
			var v = Position + originB - new Vector2(0, Z - 16);
			
			Graphics.Render(Top, Position + origin - new Vector2(0, Z), 0, origin, Scale);
			Graphics.Render(Side, v, 0, originB, Scale);
			Graphics.Render(Sides, v, 0, originB, Scale);
		}
	}
}