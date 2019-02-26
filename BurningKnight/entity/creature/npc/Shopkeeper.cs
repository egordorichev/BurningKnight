using BurningKnight.entity.creature.player;
using BurningKnight.entity.item;
using BurningKnight.entity.item.weapon.gun;
using BurningKnight.game;
using BurningKnight.physics;
using BurningKnight.util;
using BurningKnight.util.geometry;
using Lens.util.file;

namespace BurningKnight.entity.creature.npc {
	public class Shopkeeper : Npc {
		public static Shopkeeper Instance;
		private static Animation Animations = Animation.Make("actor-trader", "-green");
		private static string[] Messages = {"hi", "bad_to_see_you", "still_alive", "buy_something", "welcome"};
		private static string[] ItemMessages = {"look_at_this", "so_pretty", "only_for_you", "must_have", "buy_this"};
		private static string[] ThanksMessages = {"thanks", "nice_choice", "great", "$$$"};
		private AnimationData Animation = Idle;
		private AnimationData Death = GetAnimation().Get("death");
		public bool Enranged;
		private AnimationData Hurt = GetAnimation().Get("hurt");

		private AnimationData Idle = GetAnimation().Get("idle");
		private AnimationData Run = GetAnimation().Get("run");
		public Shotgun Shotgun;

		public Shopkeeper() {
			_Init();
		}

		protected void _Init() {
			{
				IgnoreRooms = true;
				W = 15;
				H = 15;
				Speed = 7;
				MaxSpeed = 200;
				HpMax = 20;
			}
		}

		public Animation GetAnimation() {
			return Animations;
		}

		protected override List GetDrops<Item>() {
			List<Item> Drops = base.GetDrops();

			for (var I = 0; I < Random.NewInt(3, 8); I++) Drops.Add(new Gold());

			return Drops;
		}

		public override void Load(FileReader Reader) {
			base.Load(Reader);
			Enranged = Reader.ReadBoolean();
		}

		public override void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteBoolean(Enranged);
		}

		public override void Init() {
			base.Init();
			Friendly = true;
			Body = World.CreateSimpleBody(this, 4, 0, 8, 14, BodyDef.BodyType.DynamicBody, false);
			World.CheckLocked(Body).SetTransform(this.X, this.Y, 0);
			Instance = this;
		}

		public override void Destroy() {
			base.Destroy();
			Instance = null;
		}

		protected override DialogData SelectDialog() {
			return null;
		}

		public override void Update(float Dt) {
			this.Saw = true;
			base.Update(Dt);

			if (Invt > 0)
				Animation = Hurt;
			else if (Velocity.Len2() >= 20f)
				Animation = Run;
			else
				Animation = Idle;


			if (Freezed) return;

			if (Math.Abs(Velocity.X) > 1f) Flipped = Velocity.X < 0;

			if (Dead) {
				Common();

				return;
			}

			if (Animation != null) Animation.Update(Dt * SpeedMod);

			if (Shotgun != null) {
				Shotgun.Update(Dt * SpeedMod);
				Shotgun.UpdateInHands(Dt * SpeedMod);
			}
		}

		protected override void Die(bool Force) {
			base.Die(Force);
			PlaySfx("death_towelknight");
			Done = true;

			foreach (ItemHolder Holder in ItemHolder.All) {
				Holder.GetItem().Shop = false;

				if (Holder.Price != null) {
					Holder.Price.Remove();
					Holder.Price = null;
				}
			}

			DeathEffect(Death);
			Achievements.Unlock(Achievements.UNLOCK_SALE);
		}

		public override void Render() {
			Animation.Render(this.X, this.Y, Flipped);

			if (Shotgun != null) Shotgun.Render(this.X, this.Y, W, H, Flipped, false);

			base.RenderStats();
		}

		public override void RenderSigns() {
			base.RenderSigns();

			if (Shotgun != null) Shotgun.RenderReload();
		}

		public override void RenderShadow() {
			Graphics.Shadow(this.X + 2, this.Y, W - 4, H);
		}

		public override bool WantToTalk() {
			return !Enranged;
		}

		public void Enrage() {
			if (Enranged) return;

			Friendly = false;
			Enranged = true;
			Become("hana");

			foreach (ItemHolder Holder in ItemHolder.All) {
				Holder.GetItem().Shop = false;

				if (Holder.Price != null) {
					Holder.Price.Remove();
					Holder.Price = null;
				}
			}

			Shotgun = new Shotgun();
			Shotgun.SetOwner(this);
		}

		protected override State GetAi(string State) {
			switch (State) {
				case "alerted":
				case "chase":
				case "idle":
				case "roam": {
					return new IdleState();
				}

				case "help": {
					return new HelpState();
				}

				case "hana": {
					return new HanaState();
				}

				case "hi": {
					return new HiState();
				}

				case "stand": {
					return new StandState();
				}

				case "walk": {
					return new WalkState();
				}

				case "sell": {
					return new SellState();
				}

				case "talk": {
					return new TalkState();
				}

				case "thanks": {
					return new ThanksState();
				}
			}

			return base.GetAi(State);
		}

		protected override void OnHurt(int A, Entity From) {
			base.OnHurt(A, From);
			PlaySfx("damage_towelknight");
			Become("hana");
		}

		public class SKState : State<Shopkeeper> {
		}

		public class IdleState : SKState {
			public override void Update(float Dt) {
				base.Update(Dt);

				if (Player.Instance.Room == Self.Room) Self.Become(Self.Enranged ? "hana" : "hi");
			}
		}

		public class HelpState : SKState {
			public override void Update(float Dt) {
				base.Update(Dt);

				if (Self.Room != Player.Instance.Room)
					Self.Become("idle");
				else if (MoveTo(new Point(Player.Instance.X + Player.Instance.W / 2, Player.Instance.Y + Player.Instance.H / 2), 20f, 24f)) Self.Become("stand");
			}
		}

		public class HiState : SKState {
			private float Delay;
			private NpcDialog Dialog;

			public override void OnEnter() {
				base.OnEnter();
				Delay = Random.NewFloat(5f, 10f);
				Dialog = new NpcDialog(Self, Locale.Get(Messages[Random.NewInt(Messages.Length)]));
				Dialog.Open();
				Dungeon.Area.Add(Dialog);
			}

			public override void Update(float Dt) {
				base.Update(Dt);

				if (Self.Room != Player.Instance.Room)
					Self.Become("idle");
				else if (T >= Delay) Self.Become("stand");
			}

			public override void OnExit() {
				base.OnExit();
				Dialog.Remove();
			}
		}

		public class TalkState : SKState {
			private float Delay;
			private NpcDialog Dialog;

			public override void OnEnter() {
				base.OnEnter();
				Delay = Random.NewFloat(7f, 12f);
				Dialog = new NpcDialog(Self, Locale.Get(ItemMessages[Random.NewInt(ItemMessages.Length)]));
				Dialog.Open();
				Dungeon.Area.Add(Dialog);
			}

			public override void Update(float Dt) {
				base.Update(Dt);

				if (Self.Room != Player.Instance.Room)
					Self.Become("idle");
				else if (T >= Delay) Self.Become("stand");
			}

			public override void OnExit() {
				base.OnExit();
				Dialog.Remove();
			}
		}

		public class ThanksState : SKState {
			private float Delay;
			private NpcDialog Dialog;

			public override void OnEnter() {
				base.OnEnter();
				Delay = Random.NewFloat(7f, 12f);
				Dialog = new NpcDialog(Self, Locale.Get(ThanksMessages[Random.NewInt(ThanksMessages.Length)]));
				Dialog.Open();
				Dungeon.Area.Add(Dialog);
			}

			public override void Update(float Dt) {
				base.Update(Dt);

				if (Self.Room != Player.Instance.Room)
					Self.Become("idle");
				else if (T >= Delay) Self.Become("help");
			}

			public override void OnExit() {
				base.OnExit();
				Dialog.Remove();
			}
		}

		public class StandState : SKState {
			private float Delay;

			public override void OnEnter() {
				base.OnEnter();
				Delay = Random.NewFloat(1, 3f);
			}

			public override void Update(float Dt) {
				base.Update(Dt);

				if (Self.Room != Player.Instance.Room) {
					Self.Become("idle");
				}
				else if (T >= Delay) {
					Self.Become({
						"help", "walk", "sell"
					}[Random.NewInt(3)]);
				}
			}
		}

		public class WalkState : SKState {
			private float Delay;
			private Point To;

			public override void OnEnter() {
				base.OnEnter();
				Delay = Random.NewFloat(5, 10);
			}

			public override void Update(float Dt) {
				base.Update(Dt);

				if (To == null) {
					To = Self.Room.GetRandomFreeCell();
					To.X *= 16;
					To.Y *= 16;
				}

				if (MoveTo(To, 20f, 16f)) Self.Become("stand");

				if (T >= Delay) Self.Become("stand");
			}
		}

		public class SellState : SKState {
			private Point To;

			public override void Update(float Dt) {
				base.Update(Dt);

				if (To == null) {
					List<ItemHolder> List = new List<>();

					foreach (ItemHolder Holder in ItemHolder.All)
						if (Holder.GetItem().Shop)
							List.Add(Holder);

					if (List.Size() > 0) {
						ItemHolder Target = List.Get(Random.NewInt(List.Size()));
						To = new Point();
						To.X = Target.X + (Self.W - Target.W) / 2;
						To.Y = Target.Y + 15;
					}
					else {
						Self.Become("stand");

						return;
					}
				}

				if (MoveTo(To, 20f, 16f)) Self.Become("talk");
			}
		}

		public class HanaState : SKState {
			private Point To;
			private float Tt;

			public override void OnEnter() {
				base.OnEnter();
				Enrage();
			}

			public override void Update(float Dt) {
				base.Update(Dt);
				Tt += Dt;

				if (Self.Shotgun == null) {
					Friendly = false;
					Shotgun = new Shotgun();
					Shotgun.SetOwner(Self);
				}

				if (T >= 3f) {
					if (T >= 3.3f) {
						T = 0f;
						Self.Shotgun.Use();
						Self.Shotgun.SetAmmoLeft(20);
					}
				}
				else if (Player.Instance != null && Player.Instance.Room != null) {
					if (To == null) {
						To = Player.Instance.Room.GetRandomFreeCell();
						To.X *= 16;
						To.Y *= 16;
					}

					if (MoveTo(To, 20f, 32f) || Tt >= 5f) {
						To = null;
						Tt = 0;
					}
				}
			}
		}
	}
}