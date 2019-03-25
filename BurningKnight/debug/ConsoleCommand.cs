namespace BurningKnight.debug {
	public abstract class ConsoleCommand {
		public enum Access {
			Everyone,
			Testers,
			Developers
		}

		public Access RunPermission = Access.Everyone;
		public string Name;
		public string ShortName;

		public abstract void Run(Console Console, string[] Args);

		public virtual string AutoComplete(string input) {
			return "";
		}
	}
}