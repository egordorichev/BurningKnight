using System;
using BurningKnight.assets;
using BurningKnight.entity;
using BurningKnight.entity.component;
using Lens.entity;
using Lens.graphics;
using Lens.util.timer;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.entities {
	public class Claw : Entity {
		private const float TopZ = 32;
		private const float OpenAngle = (float) Math.PI * 0.25f;
		
		private static Vector2 origin = new Vector2(15f / 2, 1);
		private static Vector2 rightOrigin = new Vector2(-0.5f, 1);
		private static Vector2 topOrigin = new Vector2(7 / 2f, 32);

		private TextureRegion leftClaw;
		private TextureRegion rightClaw;
		private TextureRegion top;
		private float angle ;
		private float z = TopZ;
		internal Vector2 start;
		
		public override void AddComponents() {
			base.AddComponents();

			Width = 15;
			Depth = Layers.FlyingMob;

			var body = new SensorBodyComponent(-origin.X, -origin.Y, 15, 14);
			AddComponent(body);

			body.Body.LinearDamping = 10;

			var animation = CommonAse.Props;

			leftClaw = animation.GetSlice("left_claw");
			rightClaw = animation.GetSlice("right_claw");
			top = animation.GetSlice("claw_hand");
			
			AddComponent(new ShadowComponent(() => {
				SimpleRender(true);
			}));
		}

		public override void PostInit() {
			base.PostInit();
			start = Position;
		}

		public override void Render() {
			SimpleRender(false);
		}

		private void SimpleRender(bool shadow) {
			var p = Position;

			if (!shadow) {
				p.Y -= z;
			}
			
			Graphics.Render(leftClaw, p, angle, origin);
			Graphics.Render(rightClaw, p, -angle, rightOrigin);
			Graphics.Render(top, p, 0, topOrigin);
		}

		public void Grab(Action a) {
			Tween.To(OpenAngle, angle, x => angle = x, 0.5f, Ease.Linear).OnEnd = () => {
				var t5 = Tween.To(0, z, x => z = x, 0.8f, Ease.Linear);

				t5.Delay = 0.5f;
				t5.OnEnd = () => {
					var t1 = Tween.To(0, angle, x => angle = x, 0.5f, Ease.Linear);

					t1.Delay = 0.5f;
					t1.OnEnd = () => {
						var t2 = Tween.To(TopZ, z, x => z = x, 0.8f, Ease.Linear);

						t2.Delay = 0.5f;
						t2.OnEnd = () => {
							Tween.To(start.Y, Y, x => Y = x, 1.2f, Ease.Linear);
							var t6 = Tween.To(start.X, X, x => X = x, 1.2f, Ease.Linear);
								
							t6.OnEnd = () => {
								var t3 = Tween.To(OpenAngle, angle, x => angle = x, 0.5f, Ease.Linear);

								t3.Delay = 0.5f;
								t3.OnEnd = () => {
									var t9 = Tween.To(0, z, x => z = x, 0.8f, Ease.Linear);

									t9.Delay = 0.5f;
									t9.OnEnd = () => {
										var t7 = Tween.To(0, angle, x => angle = x, 0.5f, Ease.Linear);

										t7.Delay = 0.5f;
										t7.OnEnd = () => {
											Tween.To(TopZ, z, x => z = x, 0.8f, Ease.Linear).OnEnd = () => {
												Timer.Add(a, 0.5f);
											};
										};
									};
								};
							};
						};
					};
				};
			};
		}
	}
}