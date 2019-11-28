using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.state;
using ImGuiNET;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.entity.component;
using Lens.entity.component.logic;
using Lens.graphics;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BurningKnight.ui.dialog {
	public delegate void DialogCallback(DialogComponent d);
	
	public class DialogComponent : Component {
		public static DialogComponent Talking;
		
		public UiDialog Dialog;
		public Dialog Last;
		public Dialog Current;

		public DialogCallback OnNext;
		public Entity To;

		private bool added;
		private float tillClose = -1;

		private void HandleInput(object sender, TextInputEventArgs args) {
			if (Current is AnswerDialog a) {
				if (a.Focused) {
					a.HandleInput(args);
				}
			}
		}

		public override void Init() {
			base.Init();
			
			Engine.Instance.Window.TextInput += HandleInput;

			Dialog = new UiDialog();
			Dialog.Owner = Entity;
			
			Dialog.OnEnd += () => {
				Dialog next = null;
				
				foreach (var c in Current.Callbacks) {
					var d = c(Current, this);

					if (d != null) {
						next = d;
					}
				}

				if (next == null) {
					next = Current.GetNext();
				}
				
				Current.Reset();

				if (next == null) {
					Last = Current;
					Current = null;
					OnNext?.Invoke(this);

					if (To != null) {
						OnEnd();
						To = null;
					}
						
					return true;
				}

				Setup(next);
				OnNext?.Invoke(this);
				return false;
			};
		}

		public override void Destroy() {
			base.Destroy();
			
			Dialog.Close(() => { Dialog.Done = true; });
			Engine.Instance.Window.TextInput -= HandleInput;

			if (Talking == this) {
				Talking = null;
			}
		}

		private bool tweening;

		public override void Update(float dt) {
			base.Update(dt);

			if (tillClose > -1 && Dialog.DoneSaying) {
				tillClose -= dt;

				if (tillClose <= 0) {
					Close();
				}
			}
			
			if (added) {
				if (GamepadComponent.Current != null && Current is AnswerDialog aa && aa.Focused) {
					aa.CheckGamepadInput(GamepadComponent.Current);
				}
				
				return;
			}

			Engine.Instance.State.Ui.Add(Dialog);

			Dialog.Str.FinishedTyping += s => {
				Entity.HandleEvent(new Dialog.EndedEvent {
					Dialog = Last ?? Current,
					Owner = Entity
				});
			};

			Dialog.Str.CharTyped += (s, i, c) => {
				if (!tweening && c != ' ' && Entity.TryGetComponent<AnimationComponent>(out var a)) {
					tweening = true;
					
					Tween.To(1.3f, a.Scale.X, x => a.Scale.X = x, 0.15f);
					Tween.To(0.75f, a.Scale.Y, x => a.Scale.Y = x, 0.15f).OnEnd = () => {
						Tween.To(1, a.Scale.X, x => a.Scale.X = x, 0.1f);
						Tween.To(1, a.Scale.Y, x => a.Scale.Y = x, 0.1f);

						tweening = false;
					};
				}
			};
			
			added = true;
		}

		public void Start(string id, Entity to = null) {
			var dialog = Dialogs.Get(id);

			if (dialog == null) {
				Setup(new Dialog(id), to);
				return;
			}

			Setup(dialog, to);
		}

		public void StartAndClose(string id, float time, Entity to = null) {
			Start(id, to);
			tillClose = time;
		}

		public void Close() {
			Last = Current;
			tillClose = -1;

			if (Dialog == null || Current == null) {
				Current = null;
				return;
			}

			Current = null;
			Dialog.Close();
		}

		private static string toSay = "";

		public override void RenderDebug() {
			base.RenderDebug();

			if (ImGui.InputText("Say", ref toSay, 256, ImGuiInputTextFlags.EnterReturnsTrue)) {
				Start(toSay);
				toSay = "";
			}

			ImGui.InputInt("Voice", ref Dialog.Voice);

			if (ImGui.Button("Test")) {
				Start("Quick brown fox jumped over lazy dog");
			}

			ImGui.SameLine();

			if (ImGui.Button("Close")) {
				Close();
			}
		}

		private void Setup(Dialog dialog, Entity to = null) {
			Last = Current;
			Current = dialog;

			var c = Locale.Get(dialog.Id);
			var s = dialog.Modify(c);
			
			Dialog.Say(s);
			
			if (Dialog.Str != null) {
				Dialog.Str.Renderer = RenderChoice;
			}

			if (to is Player) {
				To = to;
				OnStart();
			}

			Entity.HandleEvent(new Dialog.StartedEvent {
				Dialog = dialog,
				Owner = Entity
			});
		}

		private void RenderChoice(Vector2 pos, int i) {
			if (Current is ChoiceDialog c) {
				if (i == c.Choice) {
					Graphics.Print(">", Font.Small, pos);
				}
			} else if (Current is AnswerDialog a) {
				Graphics.Print($"{a.Answer}{(a.Focused && Engine.Time % 0.8f > 0.4f ? "|" : "")}", Font.Small, pos);
			}
		}

		private void OnStart() {
			var p = (Player) To;

			if (p.TryGetComponent<PlayerInputComponent>(out var input)) {
				input.InDialog = true;
				input.Dialog = this;
				Dialog.ShowArrow = true;
			}

						
			p.GetComponent<StateComponent>().Become<Player.IdleState>();
				
			((InGameState) Engine.Instance.State).OpenBlackBars();

			Talking = this;
		}
		
		private void OnEnd() {
			if (To != null && To.TryGetComponent<PlayerInputComponent>(out var input)) {
				input.InDialog = false;
				input.Dialog = null;
				Dialog.ShowArrow = false;
			}
						
			((InGameState) Engine.Instance.State).CloseBlackBars();

			Talking = null;
		}
	}
}