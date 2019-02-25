using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.mob;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.item;
using BurningKnight.core.entity.item.weapon.gun;
using BurningKnight.core.game;
using BurningKnight.core.physics;
using BurningKnight.core.util;
using BurningKnight.core.util.file;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.creature.npc {
	public class Shopkeeper : Npc {
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

		public class SKState : Mob.State<Shopkeeper>  {

		}

		public class IdleState : SKState {
			public override Void Update(float Dt) {
				base.Update(Dt);

				if (Player.Instance.Room == Self.Room) {
					Self.Become(Self.Enranged ? "hana" : "hi");
				} 
			}
		}

		public class HelpState : SKState {
			public override Void Update(float Dt) {
				base.Update(Dt);

				if (Self.Room != Player.Instance.Room) {
					Self.Become("idle");
				} else if (this.MoveTo(new Point(Player.Instance.X + Player.Instance.W / 2, Player.Instance.Y + Player.Instance.H / 2), 20f, 24f)) {
					Self.Become("stand");
				} 
			}
		}

		public class HiState : SKState {
			private NpcDialog Dialog;
			private float Delay;

			public override Void OnEnter() {
				base.OnEnter();
				Delay = Random.NewFloat(5f, 10f);
				Dialog = new NpcDialog(Self, Locale.Get(Messages[Random.NewInt(Messages.Length)]));
				Dialog.Open();
				Dungeon.Area.Add(Dialog);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (Self.Room != Player.Instance.Room) {
					Self.Become("idle");
				} else if (this.T >= Delay) {
					Self.Become("stand");
				} 
			}

			public override Void OnExit() {
				base.OnExit();
				Dialog.Remove();
			}
		}

		public class TalkState : SKState {
			private NpcDialog Dialog;
			private float Delay;

			public override Void OnEnter() {
				base.OnEnter();
				Delay = Random.NewFloat(7f, 12f);
				Dialog = new NpcDialog(Self, Locale.Get(ItemMessages[Random.NewInt(ItemMessages.Length)]));
				Dialog.Open();
				Dungeon.Area.Add(Dialog);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (Self.Room != Player.Instance.Room) {
					Self.Become("idle");
				} else if (this.T >= Delay) {
					Self.Become("stand");
				} 
			}

			public override Void OnExit() {
				base.OnExit();
				Dialog.Remove();
			}
		}

		public class ThanksState : SKState {
			private NpcDialog Dialog;
			private float Delay;

			public override Void OnEnter() {
				base.OnEnter();
				Delay = Random.NewFloat(7f, 12f);
				Dialog = new NpcDialog(Self, Locale.Get(ThanksMessages[Random.NewInt(ThanksMessages.Length)]));
				Dialog.Open();
				Dungeon.Area.Add(Dialog);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (Self.Room != Player.Instance.Room) {
					Self.Become("idle");
				} else if (this.T >= Delay) {
					Self.Become("help");
				} 
			}

			public override Void OnExit() {
				base.OnExit();
				Dialog.Remove();
			}
		}

		public class StandState : SKState {
			private float Delay;

			public override Void OnEnter() {
				base.OnEnter();
				Delay = Random.NewFloat(1, 3f);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (Self.Room != Player.Instance.Room) {
					Self.Become("idle");
				} else if (this.T >= Delay) {
					Self.Become({ "help", "walk", "sell" }[Random.NewInt(3)]);
				} 
			}
		}

		public class WalkState : SKState {
			private float Delay;
			private Point To;

			public override Void OnEnter() {
				base.OnEnter();
				this.Delay = Random.NewFloat(5, 10);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (To == null) {
					To = Self.Room.GetRandomFreeCell();
					To.X *= 16;
					To.Y *= 16;
				} 

				if (this.MoveTo(this.To, 20f, 16f)) {
					Self.Become("stand");
				} 

				if (this.T >= this.Delay) {
					Self.Become("stand");
				} 
			}
		}

		public class SellState : SKState {
			private Point To;

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (To == null) {
					List<ItemHolder> List = new List<>();

					foreach (ItemHolder Holder in ItemHolder.All) {
						if (Holder.GetItem().Shop) {
							List.Add(Holder);
						} 
					}

					if (List.Size() > 0) {
						ItemHolder Target = List.Get(Random.NewInt(List.Size()));
						To = new Point();
						To.X = Target.X + (Self.W - Target.W) / 2;
						To.Y = Target.Y + 15;
					} else {
						Self.Become("stand");

						return;
					}

				} 

				if (this.MoveTo(this.To, 20f, 16f)) {
					Self.Become("talk");
				} 
			}
		}

		public class HanaState : SKState {
			private Point To;
			private float Tt;

			public override Void OnEnter() {
				base.OnEnter();
				Enrage();
			}

			public override Void Update(float Dt) {
				base.Update(Dt);
				this.Tt += Dt;

				if (Self.Shotgun == null) {
					Friendly = false;
					Shotgun = new Shotgun();
					Shotgun.SetOwner(Self);
				} 

				if (this.T >= 3f) {
					if (this.T >= 3.3f) {
						this.T = 0f;
						Self.Shotgun.Use();
						Self.Shotgun.SetAmmoLeft(20);
					} 
				} else if (Player.Instance != null && Player.Instance.Room != null) {
					if (To == null) {
						To = Player.Instance.Room.GetRandomFreeCell();
						To.X *= 16;
						To.Y *= 16;
					} 

					if (this.MoveTo(To, 20f, 32f) || this.Tt >= 5f) {
						To = null;
						this.Tt = 0;
					} 
				} 
			}
		}

		private AnimationData Idle = GetAnimation().Get("idle");
		private AnimationData Run = GetAnimation().Get("run");
		private AnimationData Hurt = GetAnimation().Get("hurt");
		private AnimationData Death = GetAnimation().Get("death");
		private AnimationData Animation = Idle;
		public static Shopkeeper Instance;
		private static Animation Animations = Animation.Make("actor-trader", "-green");
		public bool Enranged;
		private static string[] Messages = { "hi", "bad_to_see_you", "still_alive", "buy_something", "welcome" };
		private static string[] ItemMessages = { "look_at_this", "so_pretty", "only_for_you", "must_have", "buy_this" };
		private static string[] ThanksMessages = { "thanks", "nice_choice", "great", "$$$" };
		public Shotgun Shotgun;

		public Animation GetAnimation() {
			return Animations;
		}

		protected override List GetDrops<Item> () {
			List<Item> Drops = base.GetDrops();

			for (int I = 0; I < Random.NewInt(3, 8); I++) {
				Drops.Add(new Gold());
			}

			return Drops;
		}

		public override Void Load(FileReader Reader) {
			base.Load(Reader);
			Enranged = Reader.ReadBoolean();
		}

		public override Void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteBoolean(Enranged);
		}

		public override Void Init() {
			base.Init();
			Friendly = true;
			this.Body = World.CreateSimpleBody(this, 4, 0, 8, 14, BodyDef.BodyType.DynamicBody, false);
			World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
			Instance = this;
		}

		public override Void Destroy() {
			base.Destroy();
			Instance = null;
		}

		protected override DialogData SelectDialog() {
			return null;
		}

		public override Void Update(float Dt) {
			this.Saw = true;
			base.Update(Dt);

			if (this.Invt > 0) {
				this.Animation = Hurt;
			} else if (this.Velocity.Len2() >= 20f) {
				this.Animation = Run;
			} else {
				this.Animation = Idle;
			}


			if (this.Freezed) {
				return;
			} 

			if (Math.Abs(this.Velocity.X) > 1f) {
				this.Flipped = this.Velocity.X < 0;
			} 

			if (this.Dead) {
				base.Common();

				return;
			} 

			if (this.Animation != null) {
				this.Animation.Update(Dt * SpeedMod);
			} 

			if (this.Shotgun != null) {
				this.Shotgun.Update(Dt * SpeedMod);
				this.Shotgun.UpdateInHands(Dt * SpeedMod);
			} 
		}

		protected override Void Die(bool Force) {
			base.Die(Force);
			this.PlaySfx("death_towelknight");
			this.Done = true;

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

		public override Void Render() {
			Animation.Render(this.X, this.Y, this.Flipped);

			if (this.Shotgun != null) {
				this.Shotgun.Render(this.X, this.Y, this.W, this.H, this.Flipped, false);
			} 

			base.RenderStats();
		}

		public override Void RenderSigns() {
			base.RenderSigns();

			if (this.Shotgun != null) {
				this.Shotgun.RenderReload();
			} 
		}

		public override Void RenderShadow() {
			Graphics.Shadow(this.X + 2, this.Y, this.W - 4, this.H);
		}

		public override bool WantToTalk() {
			return !Enranged;
		}

		public Void Enrage() {
			if (Enranged) {
				return;
			} 

			Friendly = false;
			Enranged = true;
			this.Become("hana");

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

		protected override Void OnHurt(int A, Entity From) {
			base.OnHurt(A, From);
			this.PlaySfx("damage_towelknight");
			this.Become("hana");
		}

		public Shopkeeper() {
			_Init();
		}
	}
}
