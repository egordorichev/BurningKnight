using System;
using BurningKnight.assets.input;
using BurningKnight.entity.component;
using BurningKnight.save;
using BurningKnight.ui.dialog;
using VelcroPhysics.Utilities;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.creature.npc {
	public class Duck : Npc {
		public override void AddComponents() {
			base.AddComponents();
			
			AlwaysActive = true;
			Width = 12;
			Height = 10;
			
			AddComponent(new AnimationComponent("duck"));

			var b = new RectBodyComponent(0, 0, Width, Height);
			AddComponent(b);
			b.KnockbackModifier = 0;

			if (GlobalSave.IsFalse("control_duck")) {
				AddComponent(new CloseDialogComponent("control_4"));
				check = true;
			}
			
			Become<IdleState>();
		}

		private bool set;
		private float x;
		private bool check;

		public override void PostInit() {
			base.PostInit();
			x = X;
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (!set) {
				set = true;
				GetComponent<DialogComponent>().Dialog.Str.SetVariable("ctrl", Controls.Find(Controls.Duck, GamepadComponent.Current != null));
			}

			if (check) {
				if (GlobalSave.IsTrue("control_duck")) {
					check = false;
					RemoveComponent<CloseDialogComponent>();
					GetComponent<DialogComponent>().Close();
				}
			}
		}
		
		#region Duck States

		private class IdleState : SmartState<Duck> {
			private bool toLeft;
			private float vx;
			private float pauseTimer;
			private float tillPause;
			
			public override void Update(float dt) {
				base.Update(dt);

				Self.GraphicsComponent.Flipped = toLeft;

				if (pauseTimer > 0) {
					pauseTimer -= dt;

					if (pauseTimer <= 0) {
						tillPause = Random.Float(0.5f, 2f);
					}
				} else if (tillPause >= 0) {
					tillPause -= dt;

					if (tillPause < 0) {
						tillPause = 0;
						pauseTimer = Random.Float(0.5f, 2f);
					}
					
					vx += (toLeft ? -1 : 1) * (dt * 10);
				}

				vx -= vx * (dt * 3);
				Self.X = MathUtils.Clamp(Self.X + vx * (10 * dt), Self.x - 16, Self.x + 16);

				if (Math.Abs(Self.X - Self.x) >= 16) {
					toLeft = !toLeft;
					vx *= -1;
				}
			}
		}
		#endregion
	}
}