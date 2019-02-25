using BurningKnight.core.assets;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.creature.fx {
	public class HpFx : Entity {
		public static Color Bad = Color.ValueOf("#ac3232");
		public static Color Good = Color.ValueOf("#99e550");
		private string Text;
		private bool Low;
		private float A = 1f;
		public bool Crit;
		public bool Block;
		private float Ch;
		private bool Tweened;
		private static TextureRegion BlockTexture = Graphics.GetTexture("ui-block");
		private static Color Orange = Color.ValueOf("#ff9900");

		public HpFx(Creature Creature, int Change, bool Block) {
			this.Text = string.ValueOf(Math.Abs(Change));
			this.Block = Block;
			Graphics.Layout.SetText(Graphics.Small, Text);

			if (this.Block) {
				this.X = Creature.X + Creature.W / 2;
			} else {
				this.X = Creature.X + (Creature.W - Graphics.Layout.Width) / 2;
			}


			this.Y = Creature.Y + Creature.H - 4;
			this.Low = Change < 0;
			this.Depth = 15;
			this.Ch = Creature.H;
		}

		public override Void Update(float Dt) {
			base.Update(Dt);

			if (!Tweened) {
				Tweened = true;
				Tween.To(new Tween.Task(this.Y + Ch * 1.5f, Block ? 0.3f : 0.8f, Tween.Type.BACK_OUT) {
					public override float GetValue() {
						return Y;
					}

					public override Void SetValue(float Value) {
						Y = Value;
					}

					public override Void OnEnd() {
						Tween.To(new Tween.Task(0, 0.3f, Tween.Type.QUAD_IN) {
							public override Void SetValue(float Value) {
								A = Value;
							}

							public override float GetValue() {
								return A;
							}

							public override Void OnEnd() {
								Done = true;
							}
						}).Delay(0.5f);
					}
				});
			} 
		}

		public override Void Render() {
			float C = (float) (0.8f + Math.Cos(Dungeon.Time * 10) / 5f);

			if (this.Block) {
				Graphics.Batch.SetColor(C, C, C, this.A);
				Graphics.Render(BlockTexture, this.X - BlockTexture.GetRegionWidth() / 2, this.Y - BlockTexture.GetRegionHeight() / 2);
				Graphics.Batch.SetColor(1, 1, 1, 1);
			} else {
				Color Color = this.Low ? (this.Crit ? Orange : Bad) : Good;
				Graphics.Small.SetColor(Color.R * C, Color.G * C, Color.B * C, this.A);
				Graphics.Print(this.Text, Graphics.Small, this.X, this.Y - 16);
				Graphics.Small.SetColor(1, 1, 1, 1);
			}

		}
	}
}
