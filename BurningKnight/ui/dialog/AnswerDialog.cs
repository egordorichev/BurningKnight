namespace BurningKnight.ui.dialog {
	public class AnswerDialog : Dialog {
		// Keep in sync with AnswerType!
		public static string[] Types = {
			"Text", "Seed"
		};
		
		public string Answer = "";
		public bool Focused = true;
		public AnswerType Type;
		
		public AnswerDialog(string id, AnswerType type, string[] next = null) : base(id, next) {
			Type = type;
		}
		
		

		public override string Modify(string dialog) {
			return $"{dialog}\n[rn 0] ";
		}
	}
}