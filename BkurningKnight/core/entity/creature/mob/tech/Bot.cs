namespace BurningKnight.core.entity.creature.mob.tech {
	public class Bot : Mob {
		public class DeathData {
			public Class Type;
			public float X;
			public float Y;
		}

		public static List<DeathData> Data = new List<>();

		protected override Void DeathEffects() {
			base.DeathEffects();
			DeathData Data = new DeathData();
			Data.Type = this.GetClass();
			Data.X = (float) (Math.Floor(this.X / 16) * 16 + 8);
			Data.Y = (float) (Math.Floor(this.Y / 16) * 16 + 8);
			Bot.Data.Add(Data);
		}
	}
}
