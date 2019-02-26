using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.player;
using BurningKnight.game.input;
using BurningKnight.util;

namespace BurningKnight.entity.creature.npc {
	public class Npc : Mob {
		public static Npc Active;

		protected float Al;
		protected bool LastWhite;
		protected bool Talking;

		protected void _Init() {
			{
				Hp = 10;
				HpMax = 10;
				Friendly = true;
			}
		}

		public override void Init() {
			base.Init();
			Friendly = true;
			Depth = 0;
		}

		protected DialogData SelectDialog() {
			return null;
		}

		protected void SetupDialog(DialogData Dialog) {
		}

		public bool WantToTalk() {
			return true;
		}

		protected override State GetAi(string State) {
			if (State.Equals("talk_dialog")) return new TalkDialogState();

			return base.GetAi(State);
		}

		public override void Update(float Dt) {
			base.Update(Dt);
			LastWhite = Active == null && Upgrade.ActiveUpgrade == null && !Talking && Player.Instance.PickupFx == null && Player.Instance.Room == Room && WantToTalk() && Player.Instance.GetDistanceTo(this.X + W / 2, this.Y + H / 2) < 10;

			if (LastWhite && Input.Instance.WasPressed("interact")) {
				Dialog.Active = SelectDialog();

				if (Dialog.Active == null) return;

				Active = this;
				Talking = true;
				Camera.Follow(this, false);
				Become("talk_dialog");
				Dialog.Active.Start();
				SetupDialog(Dialog.Active);
				Dialog.Active.OnEnd(new Runnable {

		public override void Run() {
			Talking = false;
			Active = null;
			Camera.Follow(Player.Instance, false);
			Become("idle");
		}

		public class TalkDialogState : Mob.State {
		}
	});
}

base.Common();

}
public override void Render() {
Graphics.Render(Item.Missing, this.X, this.Y);
}
public override void RenderShadow() {
Graphics.Shadow(this.X, this.Y, this.W, this.H);
}
public Npc() {
_Init();
}
}
}