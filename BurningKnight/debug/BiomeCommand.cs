using BurningKnight.entity.level.biome;

namespace BurningKnight.debug {
	public class BiomeCommand : ConsoleCommand {
		public BiomeCommand() {
			Name = "biome";
			ShortName = "b";
		}
		
		public override void Run(Console Console, string[] Args) {
			if (Args.Length == 1) {
				state.Run.Level.SetBiome(BiomeRegistry.Get(Args[0]));
			} else {
				Console.Print("/biome [id]");
			}
		}
	}
}