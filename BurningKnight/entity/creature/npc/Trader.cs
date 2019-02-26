using BurningKnight.entity.item;
using BurningKnight.entity.level.rooms.special;
using BurningKnight.entity.level.save;
using BurningKnight.game;
using BurningKnight.util;
using Lens.util.file;

namespace BurningKnight.entity.creature.npc {
	public class Trader : Npc {
		public static List<Trader> All = new List<>();
		private static string[] Dialogs = {"hi", "hey", "how_is_it_going"};
		private Dialog Ac = Dialog.Make("accessory-trader");
		private AnimationData Animation;
		private Dialog Cs = Dialog.Make("consumable-trader");
		private Dialog Ha = Dialog.Make("hat-trader");
		public string Id;
		public bool Saved;
		private float ToFlip;
		private Dialog Up = Dialog.Make("upgrade-trader");
		private Dialog Wp = Dialog.Make("weapon-trader");

		public Trader() {
			_Init();
		}

		protected void _Init() {
			{
				IgnoreRooms = true;
			}
		}

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

		public override void Init() {
			base.Init();
			All.Add(this);
			Flipped = Random.Chance(50);

			if (Id != null) {
				Saved = GlobalSave.IsTrue("npc_" + Id + "_saved");
				LoadSprite();
			}
		}

		public override void Destroy() {
			base.Destroy();
			All.Remove(this);
		}

		public override void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteBoolean(Saved);
			Writer.WriteString(Id);
		}

		public override void Load(FileReader Reader) {
			base.Load(Reader);
			Saved = Reader.ReadBoolean();
			Id = Reader.ReadString();

			if (Id != null) {
				Saved = GlobalSave.IsTrue("npc_" + Id + "_saved");
				LoadSprite();
			}
		}

		private void LoadSprite() {
			if (Id.Equals("c"))
				Animation = Animation.Make("actor-npc-consumables-trader").Get("idle");
			else if (Id.Equals("a"))
				Animation = Animation.Make("actor-npc-accessory-trader").Get("idle");
			else if (Id.Equals("b"))
				Animation = Animation.Make("actor-npc-permanent-upgrades-trader").Get("idle");
			else if (Id.Equals("d"))
				Animation = Animation.Make("actor-npc-weapons-trader").Get("idle");
			else if (Id.Equals("h")) Animation = Animation.Make("actor-npc-hats-trader").Get("idle");

			if (Animation != null) {
				W = Animation.GetFrames().Get(0).Frame.GetRegionWidth();
				H = Animation.GetFrames().Get(0).Frame.GetRegionHeight();
			}
		}

		public override bool WantToTalk() {
			return !(Room is NpcSaveRoom);
		}

		protected override DialogData SelectDialog() {
			if (Id.Equals("b"))
				return Up.Get("how_is_it");
			if (Id.Equals("d"))
				return Wp.Get("want_a_weapon");
			if (Id.Equals("c"))
				return Cs.Get("pppotions");
			if (Id.Equals("h"))
				return Ha.Get("nice");
			return Ac.Get("sorry");
		}

		public override void Update(float Dt) {
			if (!Saved && Dungeon.Depth == -2) return;

			ToFlip -= Dt;

			if (ToFlip <= 0) {
				Flipped = !Flipped;
				ToFlip = Random.NewFloat(3, 20f);
			}

			if (Dungeon.Depth != -2 && Saved && !OnScreen) {
				Done = true;
				Remove();
				Log.Error("Removing the npc");
			}

			if (Animation != null) Animation.Update(Dt);

			base.Update(Dt);
		}

		public override void Render() {
			if (!Saved && Dungeon.Depth == -2) return;

			float Dt = Gdx.Graphics.GetDeltaTime();
			Al = MathUtils.Clamp(0, 1, Al + ((LastWhite ? 1 : 0) - Al) * Dt * 10);

			if (Al > 0.05f && !Ui.HideUi) {
				Graphics.Batch.End();
				Shader.Begin();
				Shader.SetUniformf("u_color", ColorUtils.WHITE);
				Shader.SetUniformf("u_a", Al);
				Shader.End();
				Graphics.Batch.SetShader(Shader);
				Graphics.Batch.Begin();

				for (var Xx = -1; Xx < 2; Xx++)
				for (var Yy = 0; Yy < 2; Yy++)
					if (Math.Abs(Xx) + Math.Abs(Yy) == 1) {
						if (Animation != null)
							Graphics.Render(Animation.GetCurrent().Frame, this.X + Xx, this.Y + Yy, 0, 0, 0, Flipped, false);
						else
							Graphics.Render(Item.Missing, this.X + Xx, this.Y + Yy);
					}

				Graphics.Batch.End();
				Graphics.Batch.SetShader(null);
				Graphics.Batch.Begin();
			}

			if (Animation != null)
				Graphics.Render(Animation.GetCurrent().Frame, this.X, this.Y, 0, 0, 0, Flipped, false);
			else
				Graphics.Render(Item.Missing, this.X, this.Y);
		}

		public override void RenderShadow() {
			if (Saved || Dungeon.Depth != -2) base.RenderShadow();
		}

		public class TraderState : State<Trader> {
		}

		public class IdleState : TraderState {
		}

		public class HiState : TraderState {
			private float Delay;
			private NpcDialog Dialog;

			public override void OnEnter() {
				base.OnEnter();
				Delay = Random.NewFloat(5f, 7f);
				Dialog = new NpcDialog(Self, Locale.Get(Dialogs[Random.NewInt(Dialogs.Length - 1)]));
				Dialog.Open();
				Dungeon.Area.Add(Dialog);
			}

			public override void Update(float Dt) {
				base.Update(Dt);

				if (T >= Delay) Self.Become("idle");
			}

			public override void OnExit() {
				base.OnExit();
				Dialog.Remove();
			}
		}

		public class ThanksState : TraderState {
			private float Delay;
			private NpcDialog Dialog;
			private bool Second;
			private bool Third;

			public override void OnEnter() {
				base.OnEnter();
				Delay = Random.NewFloat(5f, 7f);
				Dialog = new NpcDialog(Self, Locale.Get("thanks_for_saving"));
				Dialog.Open();
				Dungeon.Area.Add(Dialog);
			}

			public override void Update(float Dt) {
				base.Update(Dt);

				if (T >= Delay) {
					if (Third) {
						Self.Become("idle");
					}
					else {
						Second = true;
						Delay = Random.NewFloat(5f, 7f);
						T = 0;
						Dialog.Remove();
					}
				}
				else if (!Third && Second) {
					if (T >= 1f) {
						Dialog = new NpcDialog(Self, Locale.Get("see_you_in_the_asylum"));
						Dialog.Open();
						Third = true;
						Dungeon.Area.Add(Dialog);
					}
				}
			}

			public override void OnExit() {
				base.OnExit();
				Dialog.Remove();
			}
		}
	}
}