using System;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.boss;
using BurningKnight.entity.projectile;
using Lens;
using Lens.util;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.bk.forms.king {
	public class JumpAttack : BossAttack<BurningKing> {
		private bool ready;
		
		public override void Init() {
			base.Init();
			
			var a = Self.GetComponent<ZAnimationComponent>();

			Tween.To(1.8f, a.Scale.X, x => a.Scale.X = x, 0.2f);
			Tween.To(0.2f, a.Scale.Y, x => a.Scale.Y = x, 0.2f).OnEnd = () => {
				Tween.To(1.8f, a.Scale.X, x => a.Scale.X = x, 0.1f);
				Tween.To(0.2f, a.Scale.Y, x => a.Scale.Y = x, 0.1f).OnEnd = () => {
					Tween.To(1, a.Scale.X, x => a.Scale.X = x, 0.2f);
					Tween.To(1, a.Scale.Y, x => a.Scale.Y = x, 0.2f);
				};

				var z = Self.GetComponent<ZComponent>();
				Tween.To(Display.Height, z.Z, x => z.Z = x, 0.5f, Ease.QuadIn).OnEnd = () => {
					var spot = Self.Target.Center;

					Tween.To(spot.X, Self.CenterX, x => Self.CenterX = x, 0.5f, Ease.QuadInOut);
					Tween.To(spot.Y, Self.CenterY, x => Self.CenterY = x, 0.5f, Ease.QuadInOut).OnEnd = () => {
						Tween.To(0, z.Z, x => z.Z = x, 0.5f, Ease.QuadIn).OnEnd = () => {
							for (var i = 0; i < 16; i++) {
								var aa = (float) Math.PI * i / 8f;
								var p = Projectile.Make(Self, "triangle", aa, 5, false);

								p.Center += MathUtils.CreateVector(aa, 16f);
								p.AddLight(32f, Projectile.RedLight);
							}

							ready = true;
							
							Tween.To(1.8f, a.Scale.X, x => a.Scale.X = x, 0.2f);
							Tween.To(0.2f, a.Scale.Y, x => a.Scale.Y = x, 0.2f).OnEnd = () => {
								Tween.To(1, a.Scale.X, x => a.Scale.X = x, 0.2f);
								Tween.To(1, a.Scale.Y, x => a.Scale.Y = x, 0.2f);
							};
							
						};
					};
				};
			};
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (!ready) {
				T = 0;
				return;
			}

			if (T > 1f) {
				Self.SelectAttack();
			}
		}
	}
}