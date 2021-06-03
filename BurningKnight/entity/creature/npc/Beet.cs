using System;
using System.Text;
using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.input;
using Lens.util;
using Lens.util.math;
using Microsoft.Xna.Framework.Input;

namespace BurningKnight.entity.creature.npc {
	public class Beet : Npc {
		private Entity interactingWith;

		static Beet() {
			Dialogs.RegisterCallback("beet_0", (d, c) => {
				c.Dialog.Str.SetVariable("seed", Run.NextSeed);
				return Dialogs.Get($"beet_{(Run.IgnoreSeed ? 4 : 1)}");
			});
			
			Dialogs.RegisterCallback("beet_2", (d, c) => {
				var a = ((AnswerDialog) d).Answer;

				Log.Info($"Beet set the seed to {a}");
				
				Rnd.Seed = a;
				Run.NextSeed = a;
				Run.IgnoreSeed = true;

				return null;
			});
			
			Dialogs.RegisterCallback("beet_4", (d, c) => {
				if (((ChoiceDialog) d).Choice == 2) {
					Run.NextSeed = Rnd.GenerateSeed();
					Run.IgnoreSeed = false;

					c.Dialog.Str.SetVariable("seed", Run.NextSeed);
					Log.Info($"Beet randomly set the seed to {Run.NextSeed}");
				}

				return null;
			});
		}

		public override void AddComponents() {
			base.AddComponents();

			Width = 18;
			Height = 19;

			AddComponent(new AnimationComponent("beet"));
			AddComponent(new RectBodyComponent(-Padding, -Padding, Width + Padding * 2, Height + Padding * 2) {
				KnockbackModifier = 0
			});
			
			AddComponent(new InteractableComponent(Interact));
			
			GetComponent<StateComponent>().Become<IdleState>();

			var dialog = GetComponent<DialogComponent>();
			
			dialog.Dialog.Voice = 1;
			dialog.OnNext += (c) => {
				if (c.Current == null && !Run.IgnoreSeed) {
					GetComponent<StateComponent>().Become<HideState>();
				}
			};
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (GetComponent<DialogComponent>().Current?.Id == "beet_2" && Input.Keyboard.WasPressed(Keys.V) && (Input.Keyboard.IsDown(Keys.LeftControl) || Input.Keyboard.IsDown(Keys.RightControl))) {
				Log.Info("Pasting the seed");
				var seed = "ERROR";

				try {
					// Needs xclip on linux
					seed = TextCopy.Clipboard.GetText().ToUpper();
				} catch (Exception e) {
					Log.Error(e);
				}

				if (seed.Length == 0) {
					return;
				}

				if (seed.Length > 8) {
					seed = seed.Substring(0, 8);
				}

				var builder = new StringBuilder();

				for (var i = 0; i < seed.Length; i++) {
					var c = seed[i];
					builder.Append(Rnd.SeedChars.IndexOf(c) != -1 && c != '_' ? c : 'X');
				}

				var result = builder.ToString();
				Log.Info($"Initial seed {seed} converted to {result}");

				((AnswerDialog) GetComponent<DialogComponent>().Current).Answer = result;
			}
		}

		private bool Interact(Entity e) {
			var state = GetComponent<StateComponent>();

			if (state.StateInstance is IdleState) {
				interactingWith = e;
				state.Become<PopState>();
			} else {
				GetComponent<DialogComponent>().Start("beet_0", e);
			}

			return true;
		}
		
		#region Beet States
		public class IdleState : SmartState<Beet> {
			
		}
		
		public class PopState : SmartState<Beet> {
			public override void Init() {
				base.Init();

				Self.GetComponent<AudioEmitterComponent>().Emit("npc_beet_show");
				Self.GetComponent<AnimationComponent>().SetAutoStop(true);
			}

			public override void Destroy() {
				base.Destroy();
				Self.GetComponent<AnimationComponent>().SetAutoStop(false);
			}

			public override void Update(float dt) {
				base.Update(dt);
				
				if (Self.GetComponent<AnimationComponent>().Animation.Paused) {
					Self.GetComponent<StateComponent>().Become<PoppedState>();
				}
			}
		}
		
		public class HideState : SmartState<Beet> {
			public override void Init() {
				base.Init();
				
				Self.GetComponent<AudioEmitterComponent>().Emit("npc_beet_hide");
				Self.GetComponent<AnimationComponent>().SetAutoStop(true);
			}

			public override void Destroy() {
				base.Destroy();
				Self.GetComponent<AnimationComponent>().SetAutoStop(false);
			}

			public override void Update(float dt) {
				base.Update(dt);
				
				if (Self.GetComponent<AnimationComponent>().Animation.Paused) {
					Self.GetComponent<StateComponent>().Become<IdleState>();
				}
			}
		}
		
		public class PoppedState : SmartState<Beet> {
			public override void Init() {
				base.Init();
				
				Self.GetComponent<DialogComponent>().Start("beet_0", Self.interactingWith);
				Self.interactingWith = null;
			}
		}
		#endregion
	}
}