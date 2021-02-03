using BurningKnight.assets;
using BurningKnight.assets.lighting;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.boss;
using Lens.assets;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.projectile {
	public class ProjectileGraphicsComponent : BasicProjectileGraphicsComponent {
		public static TextureRegion Flash;
		public bool IgnoreRotation;
		public float Rotation => IgnoreRotation ? 0 : ((Projectile) Entity).GetAnyComponent<BodyComponent>().Body.Rotation;
		public TextureRegion Aura;
		public TextureRegion Light;

		public ProjectileGraphicsComponent(string image, string slice) : base(image, slice) {
			if (Flash == null) {
				Flash = CommonAse.Particles.GetSlice("flash");
			}

			var a = Animations.Get(image);

			Aura = a.GetSlice($"{slice}_aura", false);
			Light = a.GetSlice($"{slice}_light", false);
		}

		public override void Init() {
			base.Init();

			if (Light == null) {
				((Projectile) Entity).Color = ColorUtils.WhiteColor;
			}
		}

		private bool ShouldIndicateProjectileDeath(Projectile projectile) {
			return projectile.Owner is OldKing;
		}

		public override void Render(bool shadow) {
			var p = (Projectile) Entity;
			var scale = new Vector2(p.Scale);
			var a = Rotation;
			var b = false; // p.FlashTimer > 0; // future egor: do we really need this frame that no one notices anyway? think about it, requires extra 4 bytes per bullet
			var spr = b ? Flash : Sprite;
			var or = spr.Center;

			if (shadow) {
				Graphics.Render(spr, Entity.Center + new Vector2(0, 6),
					a, or, scale);

				return;
			}

			var d = p.Dying || (ShouldIndicateProjectileDeath(p) && p.NearingDeath);
			var started = false;

			bool scourged = p.HasFlag(ProjectileFlags.Scourged);

			if (!d) {
				// fixme: p.Effect.GetColor()
				Graphics.Color = scourged ? ColorUtils.BlackColor : p.Color;
			} else if (Light == null) {
				var shader = Shaders.Entity;
				Shaders.Begin(shader);

				shader.Parameters["flash"].SetValue(1f);
				shader.Parameters["flashReplace"].SetValue(1f);
				shader.Parameters["flashColor"].SetValue(ColorUtils.White);

				started = true;
			}

			Graphics.Render(spr, Entity.Center, a, or, scale);
			Graphics.Color = ColorUtils.WhiteColor;

			if (started) {
				Shaders.End();
			}

			if (!b && Light != null) {
				if (scourged) {
					Graphics.Color = p.Color;
				}

				Graphics.Render(Light, Entity.Center, a, or, scale);

				if (scourged) {
					Graphics.Color = ColorUtils.WhiteColor;
				}
			}
		}

		public override void RenderLight() {
			if (Aura != null) {
				var p = (Projectile) Entity;

				if (!p.HasFlag(ProjectileFlags.BreakableByMelee) || !p.HasFlag(ProjectileFlags.Reflectable)) {
					return;
				}

				/*if (p.HasFlag(ProjectileFlags.Scourged)) {
					return;
				}*/

				if (!(p.Dying || (ShouldIndicateProjectileDeath(p) && p.NearingDeath))) {
					Graphics.Color = p.Color;
				}

				Graphics.Color.A = Lights.AuraAlpha;
				Graphics.Render(Aura, Entity.Center, Rotation, Aura.Center, new Vector2(((Projectile) Entity).Scale));
				Graphics.Color.A = 255;
				Graphics.Color = ColorUtils.WhiteColor;
			}
		}
	}
}