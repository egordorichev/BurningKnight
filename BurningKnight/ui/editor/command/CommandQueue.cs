using System.Collections.Generic;
using BurningKnight.state;

namespace BurningKnight.ui.editor.command {
	public class CommandQueue {
		private Stack<Command> commands = new Stack<Command>();
		private Stack<Command> redo = new Stack<Command>();

		public Editor Editor;
		
		public void Do(Command command) {
			redo.Clear();
			
			commands.Push(command);
			command.Do(Editor.Level);
		}

		public void Undo() {
			if (commands.Count == 0) {
				return;
			}

			var command = commands.Pop();
			
			command.Undo(Editor.Level);
			redo.Push(command);
		}

		public void Redo() {
			if (redo.Count == 0) {
				return;
			}

			var command = redo.Pop();
			
			command.Do(Editor.Level);
			commands.Push(command);
		}
	}
}