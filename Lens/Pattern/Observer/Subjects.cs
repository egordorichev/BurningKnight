using System.Collections.Generic;

namespace Lens.Pattern.Observer {
	public static class Subjects {
		public static Subject Audio;
		public static Subject Game;

		private static List<Subject> subjects = new List<Subject>();
		
		public static void Init() {
			Register(Audio = new Subject());
			Register(Game = new Subject());
		}

		public static void Destroy() {
			subjects.Clear();
			
			Audio = null;
			Game = null;
		}

		public static void Register(Subject subject) {
			subjects.Add(subject);
		}

		public static void Update() {
			foreach (var s in subjects) {
				s.Update();
			}
		}
	}
}