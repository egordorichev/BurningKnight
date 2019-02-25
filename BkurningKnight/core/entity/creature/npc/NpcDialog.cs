using BurningKnight.core.assets;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.creature.npc {
	public class NpcDialog : Entity {
		protected void _Init() {
			{
				Depth = 14;
				AlwaysActive = true;
				AlwaysRender = true;
			}
		}

		private Npc Npc;
		private string Message;
		private string Full;
		private TextureRegion Top = Graphics.GetTexture("bubble-top");
		private TextureRegion TopLeft = Graphics.GetTexture("bubble-top_left");
		private TextureRegion TopRight = Graphics.GetTexture("bubble-top_right");
		private TextureRegion Center = Graphics.GetTexture("bubble-center");
		private TextureRegion Left = Graphics.GetTexture("bubble-left");
		private TextureRegion Right = Graphics.GetTexture("bubble-right");
		private TextureRegion Bottom = Graphics.GetTexture("bubble-bottom");
		private TextureRegion BottomLeft = Graphics.GetTexture("bubble-bottom_left");
		private TextureRegion BottomRight = Graphics.GetTexture("bubble-bottom_right");
		private TextureRegion Overlay = Graphics.GetTexture("bubble-overlay");
		private Color Color = Color.ValueOf("0e071b");
		private Tween.Task Last;
		private float A;
		private float LastLt;
		private bool ToRemove;

		public NpcDialog(Npc Npc, string Message) {
			_Init();
			this.Npc = Npc;
			this.W = 4f;
			this.H = 0;
			this.SetMessage(Message);
		}

		public Void SetMessage(string Message) {
			this.Full = Message;
			this.Message = "";
			this.Open();
		}

		public override Void Render() {
			float X = Math.Round(this.Npc.X + this.Npc.W / 2 + this.X - TopLeft.GetRegionWidth());
			float Y = Math.Round(this.Npc.Y + this.Npc.H + 12 - this.H / 2);
			float Sx = (this.W - TopLeft.GetRegionWidth() * 2) / ((float) Top.GetRegionWidth());
			float Sy = (this.H - Left.GetRegionHeight() - 1) / ((float) Left.GetRegionHeight());
			Graphics.Batch.SetColor(1, 1, 1, this.A * 0.5f);
			Graphics.Render(Top, X + TopLeft.GetRegionWidth(), Y + this.H - TopLeft.GetRegionHeight(), 0, 0, 0, false, false, Sx, 1);
			Graphics.Render(TopLeft, X, Y + this.H - TopLeft.GetRegionHeight());
			Graphics.Render(TopRight, X + this.W - TopRight.GetRegionWidth(), Y + this.H - TopRight.GetRegionHeight());
			Graphics.Render(Left, X, Y + BottomLeft.GetRegionHeight(), 0, 0, 0, false, false, 1, Sy);
			Graphics.Render(Right, X + this.W - Right.GetRegionWidth(), Y + BottomLeft.GetRegionHeight(), 0, 0, 0, false, false, 1, Sy);
			Graphics.Render(Center, X + Left.GetRegionWidth(), Y + BottomLeft.GetRegionHeight(), 0, 0, 0, false, false, Sx, Sy);
			Graphics.Render(Bottom, X + BottomLeft.GetRegionWidth(), Y, 0, 0, 0, false, false, Sx, 1);
			Graphics.Render(BottomLeft, X, Y);
			Graphics.Render(BottomRight, X + this.W - TopRight.GetRegionWidth(), Y);
			Graphics.Render(Overlay, X + BottomLeft.GetRegionWidth(), Y - 4);
			Graphics.Batch.SetColor(1, 1, 1, 1);
			Graphics.SmallSimple.SetColor(Color.R, Color.G, Color.B, this.A);
			Graphics.Print(this.Message, Graphics.SmallSimple, X + 4, Y + 4);
			Graphics.SmallSimple.SetColor(1, 1, 1, 1);
		}

		public Void Open() {
			ToRemove = false;

			if (this.Last != null) {
				Tween.Remove(this.Last);
				this.Last = null;
			} 

			Graphics.Layout.SetText(Graphics.SmallSimple, this.Full);
			this.H = Graphics.Layout.Height + 12;
			this.W = Graphics.Layout.Width + 8;
			this.Message = "";
			this.A = 1;
			this.Last = Tween.To(new Tween.Task(1, 0.3f) {
				public override float GetValue() {
					return A;
				}

				public override Void SetValue(float Value) {
					A = Value;
				}
			});
		}

		public override Void Update(float Dt) {
			base.Update(Dt);

			if (this.Message.Length() != this.Full.Length()) {
				this.LastLt = Math.Max(0, LastLt - Dt);

				if (this.LastLt == 0) {
					LastLt = 0.05f;
					this.Message = this.Full.Substring(0, this.Message.Length() + 1);
				} 
			} 
		}

		public Void Remove() {
			ToRemove = true;

			if (this.Last != null) {
				Tween.Remove(this.Last);
				this.Last = null;
			} 

			this.Last = Tween.To(new Tween.Task(0, 0.3f) {
				public override float GetValue() {
					return A;
				}

				public override Void SetValue(float Value) {
					A = Value;
				}
			});
		}
	}
}
