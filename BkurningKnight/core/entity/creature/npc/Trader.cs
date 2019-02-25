using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.mob;
using BurningKnight.core.entity.item;
using BurningKnight.core.entity.level.rooms.special;
using BurningKnight.core.entity.level.save;
using BurningKnight.core.game;
using BurningKnight.core.util;
using BurningKnight.core.util.file;

namespace BurningKnight.core.entity.creature.npc {
	public class Trader : Npc {
		protected void _Init() {
			{
				IgnoreRooms = true;
			}
		}

		public class TraderState : Mob.State<Trader>  {

		}

		public class IdleState : TraderState {

		}

		public class HiState : TraderState {
			private NpcDialog Dialog;
			private float Delay;

			public override Void OnEnter() {
				base.OnEnter();
				Delay = Random.NewFloat(5f, 7f);
				Dialog = new NpcDialog(Self, Locale.Get(Dialogs[Random.NewInt(Dialogs.Length - 1)]));
				Dialog.Open();
				Dungeon.Area.Add(Dialog);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (this.T >= this.Delay) {
					Self.Become("idle");
				} 
			}

			public override Void OnExit() {
				base.OnExit();
				Dialog.Remove();
			}
		}

		public class ThanksState : TraderState {
			private NpcDialog Dialog;
			private float Delay;
			private bool Second;
			private bool Third;

			public override Void OnEnter() {
				base.OnEnter();
				Delay = Random.NewFloat(5f, 7f);
				Dialog = new NpcDialog(Self, Locale.Get("thanks_for_saving"));
				Dialog.Open();
				Dungeon.Area.Add(Dialog);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (this.T >= this.Delay) {
					if (this.Third) {
						Self.Become("idle");
					} else {
						this.Second = true;
						Delay = Random.NewFloat(5f, 7f);
						this.T = 0;
						Dialog.Remove();
					}

				} else if (!this.Third && this.Second) {
					if (this.T >= 1f) {
						Dialog = new NpcDialog(Self, Locale.Get("see_you_in_the_asylum"));
						Dialog.Open();
						this.Third = true;
						Dungeon.Area.Add(Dialog);
					} 
				} 
			}

			public override Void OnExit() {
				base.OnExit();
				Dialog.Remove();
			}
		}

		public static List<Trader> All = new List<>();
		private AnimationData Animation;
		public bool Saved = false;
		public string Id;
		private static string[] Dialogs = { "hi", "hey", "how_is_it_going" };
		private Dialog Up = Dialog.Make("upgrade-trader");
		private Dialog Wp = Dialog.Make("weapon-trader");
		private Dialog Cs = Dialog.Make("consumable-trader");
		private Dialog Ha = Dialog.Make("hat-trader");
		private Dialog Ac = Dialog.Make("accessory-trader");
		private float ToFlip;

		protected override State GetAi(string State) {
			switch (State) {
				case "hi": {
					return new HiState();
				}

				case "thanks": {
					return new ThanksState();
				}
			}

			return new IdleState();
		}

		public override Void Init() {
			base.Init();
			All.Add(this);
			Flipped = Random.Chance(50);

			if (this.Id != null) {
				this.Saved = GlobalSave.IsTrue("npc_" + this.Id + "_saved");
				this.LoadSprite();
			} 
		}

		public override Void Destroy() {
			base.Destroy();
			All.Remove(this);
		}

		public override Void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteBoolean(this.Saved);
			Writer.WriteString(this.Id);
		}

		public override Void Load(FileReader Reader) {
			base.Load(Reader);
			this.Saved = Reader.ReadBoolean();
			this.Id = Reader.ReadString();

			if (this.Id != null) {
				this.Saved = GlobalSave.IsTrue("npc_" + this.Id + "_saved");
				this.LoadSprite();
			} 
		}

		private Void LoadSprite() {
			if (this.Id.Equals("c")) {
				this.Animation = Animation.Make("actor-npc-consumables-trader").Get("idle");
			} else if (this.Id.Equals("a")) {
				this.Animation = Animation.Make("actor-npc-accessory-trader").Get("idle");
			} else if (this.Id.Equals("b")) {
				this.Animation = Animation.Make("actor-npc-permanent-upgrades-trader").Get("idle");
			} else if (this.Id.Equals("d")) {
				this.Animation = Animation.Make("actor-npc-weapons-trader").Get("idle");
			} else if (this.Id.Equals("h")) {
				this.Animation = Animation.Make("actor-npc-hats-trader").Get("idle");
			} 

			if (this.Animation != null) {
				W = Animation.GetFrames().Get(0).Frame.GetRegionWidth();
				H = Animation.GetFrames().Get(0).Frame.GetRegionHeight();
			} 
		}

		public override bool WantToTalk() {
			return !(this.Room is NpcSaveRoom);
		}

		protected override DialogData SelectDialog() {
			if (this.Id.Equals("b")) {
				return Up.Get("how_is_it");
			} else if (this.Id.Equals("d")) {
				return Wp.Get("want_a_weapon");
			} else if (this.Id.Equals("c")) {
				return Cs.Get("pppotions");
			} else if (this.Id.Equals("h")) {
				return Ha.Get("nice");
			} else {
				return Ac.Get("sorry");
			}

		}

		public override Void Update(float Dt) {
			if (!this.Saved && Dungeon.Depth == -2) {
				return;
			} 

			ToFlip -= Dt;

			if (ToFlip <= 0) {
				Flipped = !Flipped;
				ToFlip = Random.NewFloat(3, 20f);
			} 

			if (Dungeon.Depth != -2 && this.Saved && !this.OnScreen) {
				this.Done = true;
				this.Remove();
				Log.Error("Removing the npc");
			} 

			if (this.Animation != null) {
				this.Animation.Update(Dt);
			} 

			base.Update(Dt);
		}

		public override Void Render() {
			if (!this.Saved && Dungeon.Depth == -2) {
				return;
			} 

			float Dt = Gdx.Graphics.GetDeltaTime();
			this.Al = MathUtils.Clamp(0, 1, this.Al + ((LastWhite ? 1 : 0) - this.Al) * Dt * 10);

			if (this.Al > 0.05f && !Ui.HideUi) {
				Graphics.Batch.End();
				Mob.Shader.Begin();
				Mob.Shader.SetUniformf("u_color", ColorUtils.WHITE);
				Mob.Shader.SetUniformf("u_a", this.Al);
				Mob.Shader.End();
				Graphics.Batch.SetShader(Mob.Shader);
				Graphics.Batch.Begin();

				for (int Xx = -1; Xx < 2; Xx++) {
					for (int Yy = 0; Yy < 2; Yy++) {
						if (Math.Abs(Xx) + Math.Abs(Yy) == 1) {
							if (this.Animation != null) {
								Graphics.Render(this.Animation.GetCurrent().Frame, this.X + Xx, this.Y + Yy, 0, 0, 0, this.Flipped, false);
							} else {
								Graphics.Render(Item.Missing, this.X + Xx, this.Y + Yy);
							}

						} 
					}
				}

				Graphics.Batch.End();
				Graphics.Batch.SetShader(null);
				Graphics.Batch.Begin();
			} 

			if (this.Animation != null) {
				Graphics.Render(this.Animation.GetCurrent().Frame, this.X, this.Y, 0, 0, 0, this.Flipped, false);
			} else {
				Graphics.Render(Item.Missing, this.X, this.Y);
			}

		}

		public override Void RenderShadow() {
			if (this.Saved || Dungeon.Depth != -2) {
				base.RenderShadow();
			} 
		}

		public Trader() {
			_Init();
		}
	}
}
