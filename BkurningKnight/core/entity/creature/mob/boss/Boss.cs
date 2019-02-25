using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.level.entities;
using BurningKnight.core.util;
using BurningKnight.core.util.file;

namespace BurningKnight.core.entity.creature.mob.boss {
	public class Boss : Mob {
		public class BossState<T>  : State<T> where T : Mob {
			public Void CheckForTarget() {
				if (Self.Target != null && !Self.GetState().Equals("roam") && !Self.GetState().Equals("idle")) {
					return;
				} 

				foreach (Player Player in Player.All) {
					if (Player.Invisible) {
						continue;
					} 

					if (Self.CanSee(Player) && (!ShouldBeInTheSameRoom || Player.Room == Self.Room)) {
						Self.Target = Player;
						Self.Become("alerted");
						Self.NoticeSignT = 2f;
						Self.HideSignT = 0f;

						return;
					} 
				}
			}
		}

		public static List<Boss> All = new List<>();
		public string Texture;
		public bool IgnoreHealthbar;
		public bool ShouldBeInTheSameRoom;
		public bool Talked;
		public bool Rage;

		public override Void Load(FileReader Reader) {
			base.Load(Reader);
			Talked = Reader.ReadBoolean();
			this.ShouldBeInTheSameRoom = !this.Talked;
		}

		public override Void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteBoolean(Talked);
		}

		protected override State GetAiWithLow(string State) {
			return GetAi(State);
		}

		public override Void Init() {
			base.Init();
			All.Add(this);
		}

		public override Void Destroy() {
			base.Destroy();
			All.Remove(this);
		}

		protected override Void Die(bool Force) {
			base.Die(Force);
			Audio.HighPriority("Reckless");
		}

		protected override List GetDrops<Item> () {
			List<Item> Drops = base.GetDrops();

			for (int I = 0; I < Random.NewInt(5, 16); I++) {
				Drops.Add(new Coin());
			}

			return Drops;
		}
	}
}
