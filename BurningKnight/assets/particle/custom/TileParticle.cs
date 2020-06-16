using BurningKnight.entity;
using BurningKnight.entity.component;
using BurningKnight.level.tile;
using BurningKnight.state;
using Lens;
using Lens.entity;
using Lens.graphics;
using Lens.util.math;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using Tile = BurningKnight.level.tile.Tile;

namespace BurningKnight.assets.particle.custom {
	public class TileParticle : Entity {
		private static Vector2 origin = new Vector2(8, 24);
		private static Vector2 originB = new Vector2(8, 8);
		public static float MaxZ = Display.Height;
		
		public TextureRegion Top;
		public TextureRegion TopTarget;
		public TextureRegion Side;
		public TextureRegion Sides;
		public Vector2 Scale = new Vector2(0, 3);
		public float Z;
		public Tile Tile;
		public Tile OriginalTile;
		public bool FromBottom;
		public Vector2 Target;
		public float TargetZ;
		
		public override void AddComponents() {
			base.AddComponents();

			Height = 24;
			
			AlwaysActive = true;

			if (FromBottom) {
				var level = Run.Level;
				var x = (int) (X / 16);
				var y = (int) ((Y + 8) / 16);

				OriginalTile = level.Get(x, y);

				if (OriginalTile == Tile.Chasm) {
					Done = true;
					return;
				}
				
				if (!OriginalTile.IsWall()) {
					Z = -8;
				}
				
				Scale.X = 1;
				Scale.Y = 1;
				
				level.Set(x, y, Tile.Chasm);
				level.UpdateTile(x, y);
				level.ReCreateBodyChunk(x, y);
				
				Tween.To(MaxZ, Z, z => {
					if (z > 8) {
						Depth = Layers.FlyingMob;
					}

					Z = z;
				}, 0.5f, Ease.QuadIn).OnEnd = () => {
					Tween.To(Target.X, X, v => X = v, 1f, Ease.QuadInOut);
					Tween.To(Target.Y, Y, v => Y = v, 1f, Ease.QuadInOut).OnEnd = () => {
						x = (int) ((Target.X + 8) / 16);
						y = (int) ((Target.Y + 8) / 16);

						if (level.Get(x, y) != Tile.Chasm) {
							Top = TopTarget;
						} else {
							TargetZ = -8;
							Tile = OriginalTile;
						}

						AnimateFall();
					};
				};
			} else {
				Z = MaxZ;
				Depth = Layers.FlyingMob;

				Tween.To(1f, Scale.X, x => Scale.X = x, 0.4f);
				Tween.To(1f, Scale.Y, x => Scale.Y = x, 0.4f);

				AnimateFall();
			}

			AddComponent(new ShadowComponent(RenderShadow));
		}

		private void AnimateFall() {
			Tween.To(TargetZ, Z, x => {
				Z = x;

				if (Z <= 8f) {
					Depth = 0;
				}
			}, 0.5f, Ease.QuadIn).OnEnd = () => {
				if (TargetZ >= -0.01f) {
					Scale.X = 3;
					Scale.Y = 0.3f;
				}

				AudioEmitterComponent.Dummy(Area, Center).Emit($"level_rock_{Rnd.Int(1, 3)}", 0.5f);

				if (TargetZ < 0) {
					Finish();
				} else {
					Tween.To(1, Scale.X, x => Scale.X = x, 0.5f);
					Tween.To(1, Scale.Y, x => Scale.Y = x, 0.5f).OnEnd = () => { Finish(); };
				}
			};
		}
		
		private void Finish() {
			var level = Run.Level;
			var x = (int) (CenterX / 16);
			var y = (int) ((Y + 8) / 16);

			level.Liquid[level.ToIndex(x, y)] = 0;
			level.Set(x, y, Tile);
			level.UpdateTile(x, y);
			level.ReCreateBodyChunk(x, y);

			Done = true;
		}

		public void RenderShadow() {
			if (Z >= 0) {
				var or = Top.Center;
				Graphics.Render(Top, Position + or + new Vector2(0, 16), 0, or, Scale);
			}
		}

		public override void Render() {
			var v = Position + originB - new Vector2(0, Z - 16);
			
			Graphics.Render(Top, Position + origin - new Vector2(0, Z), 0, origin, Scale);

			if (Z >= 0) {
				Graphics.Render(Side, v, 0, originB, Scale);
				Graphics.Render(Sides, v, 0, originB, Scale);
			}
		}
	}
}