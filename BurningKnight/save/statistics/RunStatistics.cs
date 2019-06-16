namespace BurningKnight.save.statistics {
	public class RunStatistics {
		public float Time;
		public bool Won;
		public byte Depth;
		public uint Version;
		
		public string[] Items;
		
		public byte Coins;
		public byte Bombs;
		public byte Keys;
		
		public byte Health;
		public byte MaxHealth;
		public byte IronHearts;
		public byte GoldenHearts;

		public uint DamageTaken;
		public uint DamageDealt;
		public uint MobsKilled;

		public uint RoomsExplored;
		public uint RoomsTotal;
		public uint SecretRoomsFound;
		public uint SecretRoomsTotal;

		public short Year;
		public short Day;
	}
}