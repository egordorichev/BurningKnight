using BurningKnight.level;

namespace BurningKnight.entity.editor.command {
	public interface Command {
		void Do(Level level);
		void Undo(Level level);
	}
}