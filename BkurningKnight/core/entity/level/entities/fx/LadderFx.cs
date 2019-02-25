using BurningKnight.core;
using BurningKnight.core.assets;
using BurningKnight.core.entity;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.level.entities;
using BurningKnight.core.game.input;
using BurningKnight.core.game.state;
using BurningKnight.core.ui;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.level.entities.fx {
	public class LadderFx : UiEntity {
		protected void _Init() {
			{
				AlwaysActive = true;
			}
		}

		private Entity Ladder;
		private string Text;
		private float A;

		public LadderFx(Entity Ladder, string Text) {
			_Init();
			this.Ladder = Ladder;
			this.Text = Locale.Get(Text);
			GlyphLayout Layout = new GlyphLayout(Graphics.Medium, this.Text);
			this.X = Ladder.X + 8 - Layout.Width / 2;
			this.Y = Ladder.Y + Ladder.H + 4;
			this.Depth = 16;
			Tween.To(new Tween.Task(1, 0.1f, Tween.Type.QUAD_OUT) {
				public override float GetValue() {
					return A;
				}

				public override Void SetValue(float Value) {
					A = Value;
				}
			});
		}

		public override Void Render() {
			float C = (float) (0.8f + Math.Cos(Dungeon.Time * 10) / 5f);
			Graphics.Medium.SetColor(C, C, C, this.A);
			Graphics.Print(this.Text, Graphics.Medium, this.X, this.Y);
			Graphics.Medium.SetColor(1, 1, 1, 1);

			if (Input.Instance.WasPressed("interact") && Dialog.Active == null && Player.Instance.PickupFx == null) {
				this.Remove();
				this.End();
			} 
		}

		public Void End() {
			Dungeon.DarkR = 0;
			Dungeon.DarkR = Dungeon.MAX_R;
			Player.Instance.SetUnhittable(true);
			Camera.Follow(null);
			Player.Instance.PlaySfx("menu/select");
			InGameState.StartTween = true;
			InGameState.Id = ((Exit) Ladder).GetType();
		}

		public Void Remove() {
			Tween.To(new Tween.Task(0, 0.2f, Tween.Type.QUAD_IN) {
				public override float GetValue() {
					return A;
				}

				public override Void SetValue(float Value) {
					A = Value;
				}

				public override Void OnEnd() {
					base.OnEnd();
					SetDone(true);
				}
			});
		}
	}
}
