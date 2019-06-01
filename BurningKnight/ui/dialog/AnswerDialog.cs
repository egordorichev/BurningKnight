using System.Text;

namespace BurningKnight.ui.dialog {
	public class AnswerDialog : Dialog {
		public string Answer = "";
		
		public AnswerDialog(string id, string[] next = null) : base(id, next) {
			
		}

		public override string Modify(string dialog) {
			var builder = new StringBuilder();

			builder.Append(dialog).Append("\n[rn 0]");

			return builder.ToString();
		}
	}
}