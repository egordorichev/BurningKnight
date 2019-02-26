using BurningKnight.util;

namespace BurningKnight.entity.creature.npc {
	public class NpcDialog : Entity {
		private float A;
		private TextureRegion Bottom = Graphics.GetTexture("bubble-bottom");
		private TextureRegion BottomLeft = Graphics.GetTexture("bubble-bottom_left");
		private TextureRegion BottomRight = Graphics.GetTexture("bubble-bottom_right");
		private TextureRegion Center = Graphics.GetTexture("bubble-center");
		private Color Color = Color.ValueOf("0e071b");
		private string Full;
		private Tween.Task Last;
		private float LastLt;
		private TextureRegion Left = Graphics.GetTexture("bubble-left");
		private string Message;

		private Npc Npc;
		private TextureRegion Overlay = Graphics.GetTexture("bubble-overlay");
		private TextureRegion Right = Graphics.GetTexture("bubble-right");
		private TextureRegion Top = Graphics.GetTexture("bubble-top");
		private TextureRegion TopLeft = Graphics.GetTexture("bubble-top_left");
		private TextureRegion TopRight = Graphics.GetTexture("bubble-top_right");
		private bool ToRemove;

		public NpcDialog(Npc Npc, string Message) {
			_Init();
			this.Npc = Npc;
			W = 4f;
			H = 0;
			SetMessage(Message);
		}

		protected void _Init() {
			{
				Depth = 14;
				AlwaysActive = true;
				AlwaysRender = true;
			}
		}

		public void SetMessage(string Message) {
			Full = Message;
			this.Message = "";
			Open();
		}

		public override void Render() {
			float X = Math.Round(Npc.X + Npc.W / 2 + this.X - TopLeft.GetRegionWidth());
			float Y = Math.Round(Npc.Y + Npc.H + 12 - H / 2);
			var Sx = (W - TopLeft.GetRegionWidth() * 2) / (float) Top.GetRegionWidth();
			var Sy = (H - Left.GetRegionHeight() - 1) / (float) Left.GetRegionHeight();
			Graphics.Batch.SetColor(1, 1, 1, A * 0.5f);
			Graphics.Render(Top, X + TopLeft.GetRegionWidth(), Y + H - TopLeft.GetRegionHeight(), 0, 0, 0, false, false, Sx, 1);
			Graphics.Render(TopLeft, X, Y + H - TopLeft.GetRegionHeight());
			Graphics.Render(TopRight, X + W - TopRight.GetRegionWidth(), Y + H - TopRight.GetRegionHeight());
			Graphics.Render(Left, X, Y + BottomLeft.GetRegionHeight(), 0, 0, 0, false, false, 1, Sy);
			Graphics.Render(Right, X + W - Right.GetRegionWidth(), Y + BottomLeft.GetRegionHeight(), 0, 0, 0, false, false, 1, Sy);
			Graphics.Render(Center, X + Left.GetRegionWidth(), Y + BottomLeft.GetRegionHeight(), 0, 0, 0, false, false, Sx, Sy);
			Graphics.Render(Bottom, X + BottomLeft.GetRegionWidth(), Y, 0, 0, 0, false, false, Sx, 1);
			Graphics.Render(BottomLeft, X, Y);
			Graphics.Render(BottomRight, X + W - TopRight.GetRegionWidth(), Y);
			Graphics.Render(Overlay, X + BottomLeft.GetRegionWidth(), Y - 4);
			Graphics.Batch.SetColor(1, 1, 1, 1);
			Graphics.SmallSimple.SetColor(Color.R, Color.G, Color.B, A);
			Graphics.Print(Message, Graphics.SmallSimple, X + 4, Y + 4);
			Graphics.SmallSimple.SetColor(1, 1, 1, 1);
		}

		public void Open() {
			ToRemove = false;

			if (Last != null) {
				Tween.Remove(Last);
				Last = null;
			}

			Graphics.Layout.SetText(Graphics.SmallSimple, Full);
			H = Graphics.Layout.Height + 12;
			W = Graphics.Layout.Width + 8;
			Message = "";
			A = 1;
			Last = Tween.To(new Tween.Task(1, 0.3f) {

		public override float GetValue() {
			return A;
		}

		public override void SetValue(float Value) {
			A = Value;
		}
	});
}

public override void Update(float Dt) {
base.Update(Dt);
if (this.Message.Length() != this.Full.Length()) {
this.LastLt = Math.Max(0, LastLt - Dt);
if (this.LastLt == 0) {
LastLt = 0.05f;
this.Message = this.Full.Substring(0, this.Message.Length() + 1);
}
}
}
public void Remove() {
ToRemove = true;
if (this.Last != null) {
Tween.Remove(this.Last);
this.Last = null;
}
this.Last = Tween.To(new Tween.Task(0, 0.3f) {
public override float GetValue() {
return A;
}
public override void SetValue(float Value) {
A = Value;
}
});
}
}
}