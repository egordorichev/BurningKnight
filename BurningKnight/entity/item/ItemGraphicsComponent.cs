using System;
using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.ui.imgui;
using Lens.graphics;
using Lens.util;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.item {
	public class ItemGraphicsComponent : SliceComponent {
		public const float FlashSize = 0.025f;
		public const int ScourgedColorId = 48;
		public static Color MaskedColor = new Color(0f, 0f, 0f, 0.75f);
		public static Vector4 ScourgedColor = Palette.Default[ScourgedColorId].ToVector4();
		
		public float T;
		
		public ItemGraphicsComponent(string slice) : base(CommonAse.Items, slice) {
			T = Rnd.Float(32f);
		}

		public override void Init() {
			base.Init();
			
			Entity.Width = Sprite.Source.Width;
			Entity.Height = Sprite.Source.Height;
		}

		public override void Update(float dt) {
			base.Update(dt);
			T += dt;
		}

		public static float CalculateMove(float t) {
			return (float) (Math.Sin(t * 3f) * 0.5f + 0.5f) * -5.5f;
		}

		public virtual Vector2 CalculatePosition(bool shadow = false) {
			return Entity.Position + Sprite.Center + new Vector2(0, shadow ? 8 : CalculateMove(T));
		}

		public override void Render(bool shadow) {
			if (Entity.HasComponent<OwnerComponent>()) {
				return;
			}
			
			var item = (Item) Entity;
			var s = item.Hidden ? Item.UnknownRegion : Sprite;
			var origin = s.Center;
			var position = CalculatePosition(shadow);
			var angle = (float) Math.Cos(T * 1.8f) * 0.4f;
			var cursed = item.Scourged;

			if (!shadow) {
				var interact = Entity.TryGetComponent<InteractableComponent>(out var component) &&
				               component.OutlineAlpha > 0.05f;

				if (cursed || interact) {
					var shader = Shaders.Entity;
					Shaders.Begin(shader);

					shader.Parameters["flash"].SetValue(cursed ? 1f : component.OutlineAlpha);
					shader.Parameters["flashReplace"].SetValue(1f);
					shader.Parameters["flashColor"].SetValue(!cursed ? ColorUtils.White : ColorUtils.Mix(ScourgedColor, ColorUtils.White, component.OutlineAlpha));

					foreach (var d in MathUtils.Directions) {
						Graphics.Render(s, position + d, angle, origin);
					}

					Shaders.End();
				}
			}

			if (item.Masked) {
				Graphics.Color = MaskedColor;
				Graphics.Render(s, position, angle, origin);
				Graphics.Color = ColorUtils.WhiteColor;
			} else {
				if (!shadow && DebugWindow.ItemShader && !Settings.LowQuality) {
					var shader = Shaders.Item;
				
					Shaders.Begin(shader);
					shader.Parameters["time"].SetValue(T * 0.1f);
					shader.Parameters["size"].SetValue(FlashSize);
				}

				Graphics.Render(s, position, angle, origin);

				if (!shadow && DebugWindow.ItemShader && !Settings.LowQuality) {
					Shaders.End();
				}
			}
		}
	}
}