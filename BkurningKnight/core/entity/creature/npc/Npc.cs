using BurningKnight.core.entity.creature.mob;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.game.input;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.creature.npc {
	public class Npc : Mob {
		protected void _Init() {
			{
				Hp = 10;
				HpMax = 10;
				Friendly = true;
			}
		}

		public class TalkDialogState : Mob.State {

		}

		protected float Al;
		protected bool Talking;
		protected bool LastWhite;
		public static Npc Active;

		public override Void Init() {
			base.Init();
			Friendly = true;
			Depth = 0;
		}

		protected DialogData SelectDialog() {
			return null;
		}

		protected Void SetupDialog(DialogData Dialog) {

		}

		public bool WantToTalk() {
			return true;
		}

		protected override State GetAi(string State) {
			if (State.Equals("talk_dialog")) {
				return new TalkDialogState();
			} 

			return base.GetAi(State);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);
			LastWhite = (Active == null && Upgrade.ActiveUpgrade == null && !this.Talking && Player.Instance.PickupFx == null && Player.Instance.Room == this.Room && WantToTalk() && Player.Instance.GetDistanceTo(this.X + this.W / 2, this.Y + this.H / 2) < 10);

			if (LastWhite && Input.Instance.WasPressed("interact")) {
				Dialog.Active = SelectDialog();

				if (Dialog.Active == null) {
					return;
				} 

				Active = this;
				Talking = true;
				Camera.Follow(this, false);
				this.Become("talk_dialog");
				Dialog.Active.Start();
				SetupDialog(Dialog.Active);
				Dialog.Active.OnEnd(new Runnable() {
					public override Void Run() {
						Talking = false;
						Active = null;
						Camera.Follow(Player.Instance, false);
						Become("idle");
					}
				});
			} 

			base.Common();
		}

		public override Void Render() {
			Graphics.Render(Item.Missing, this.X, this.Y);
		}

		public override Void RenderShadow() {
			Graphics.Shadow(this.X, this.Y, this.W, this.H);
		}

		public Npc() {
			_Init();
		}
	}
}
