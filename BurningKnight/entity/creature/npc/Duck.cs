using System;
using BurningKnight.assets;
using BurningKnight.assets.input;
using BurningKnight.entity.component;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using Lens;
using Lens.util.file;
using Lens.util.math;
using VelcroPhysics.Utilities;

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

			quantom = Rnd.Chance(1);
			
			if (Run.Depth == -2) {
				AddComponent(new CloseDialogComponent("control_4"));
			} else {
				AddComponent(new CloseDialogComponent($"duck_{(quantom ? 1 : 0)}"));
			}
			
			Become<IdleState>();
		}

		private bool set;
		private float x;
		private bool quantom;

		public override void PostInit() {
			base.PostInit();
			x = X;
		}

		public override void Save(FileWriter stream) {
			if (Engine.Instance.State is EditorState) {
				x = X;
			} else {
				X = x;
			}

			base.Save(stream);
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (!set) {
				set = true;
				
				var dialog = GetComponent<DialogComponent>();
								
				dialog.Dialog.Str.ClearIcons();
				dialog.Dialog.Str.AddIcon(CommonAse.Ui.GetSlice(Controls.FindSlice(Controls.Duck, false)));

				if (GamepadComponent.Current != null && GamepadComponent.Current.Attached) {
					dialog.Dialog.Str.AddIcon(CommonAse.Ui.GetSlice(Controls.FindSlice(Controls.Duck, true)));
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
						tillPause = Rnd.Float(0.5f, 2f);
					}
				} else if (tillPause >= 0) {
					tillPause -= dt;

					if (tillPause < 0) {
						tillPause = 0;
						pauseTimer = Rnd.Float(0.5f, 2f);
					}
					
					vx += (toLeft ? -1 : 1) * (dt * 10);
				}
				
				vx -= vx * (dt * 3);
				var n = Self.X + vx * (10 * dt);

				if (Self.quantom) {
					Self.X = MathUtils.Clamp(n, Self.x - 16, Self.x + 16);

					if (Math.Abs(Self.X - Self.x) >= 16) {
						toLeft = !toLeft;
						Self.X = Self.x + (toLeft ? -16 : 16);
						vx *= -1;

						if (Rnd.Chance(10)) {
							Self.X = Self.x;
						}
					}
				} else {
					Self.X = MathUtils.Clamp(n, Self.x - 16, Self.x + 16);

					if (Math.Abs(Self.X - Self.x) >= 16) {
						toLeft = !toLeft;
						vx *= -1;
					}
				}
			}
		}
		#endregion
	}
}