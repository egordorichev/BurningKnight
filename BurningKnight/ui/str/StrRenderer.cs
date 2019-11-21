namespace BurningKnight.ui.str {
	public class StrRenderer {
		public int Id;
		public int Where;

		public virtual int GetWidth(UiString str) {
			return 0;
		}
	}
}