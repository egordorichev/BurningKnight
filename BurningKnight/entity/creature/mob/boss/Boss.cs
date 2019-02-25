using BurningKnight.entity.creature.player;
using BurningKnight.entity.level.entities;
using BurningKnight.util;
using Lens.util.file;

namespace BurningKnight.entity.creature.mob.boss {
	public class Boss : Mob {
		public static List<Boss> All = new List<>();
		public bool IgnoreHealthbar;
		public bool Rage;
		public bool ShouldBeInTheSameRoom;
		public bool Talked;
		public string Texture;

		public override void Load(FileReader Reader) {
			base.Load(Reader);
			Talked = Reader.ReadBoolean();
			ShouldBeInTheSameRoom = !Talked;
		}

		public override void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteBoolean(Talked);
		}

		protected override State GetAiWithLow(string State) {
			return GetAi(State);
		}

		public override void Init() {
			base.Init();
			All.Add(this);
		}

		public override void Destroy() {
			base.Destroy();
			All.Remove(this);
		}

		protected override void Die(bool Force) {
			base.Die(Force);
			Audio.HighPriority("Reckless");
		}

		protected override List GetDrops<Item>() {
			List<Item> Drops = base.GetDrops();

			for (var I = 0; I < Random.NewInt(5, 16); I++) Drops.Add(new Coin());

			return Drops;
		}

		public class BossState<T> : State<T> where T : Mob {
			public void CheckForTarget() {
				if (Self.Target != null && !Self.GetState().Equals("roam") && !Self.GetState().Equals("idle")) return;

				foreach (Player Player in Player.All) {
					if (Player.Invisible) continue;

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
	}
}