namespace BurningKnight.core.debug {
	public abstract class ConsoleCommand {
		public string Name;
		public string ShortName;

		public abstract Void Run(Console Console, string Args);
	}
}
