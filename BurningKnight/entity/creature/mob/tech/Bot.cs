namespace BurningKnight.entity.creature.mob.tech {
	public class Bot : Mob {
		public static List<DeathData> Data = new List<>();

		protected override void DeathEffects() {
			base.DeathEffects();
			var Data = new DeathData();
			Data.Type = this.GetClass();
			Data.X = Math.Floor(this.X / 16) * 16 + 8;
			Data.Y = Math.Floor(this.Y / 16) * 16 + 8;
			Bot.Data.Add(Data);
		}

		public class DeathData {
			public Class Type;
			public float X;
			public float Y;
		}
	}
}