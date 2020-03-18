using System.Collections.Generic;

namespace BurningKnight.level.challenge {
	public static class ChallengeRegistry {
		private static List<Challenge> challenges = new List<Challenge>();

		static ChallengeRegistry() {
			Add(new BombOnlyChallenge());
		}

		public static void Add(Challenge challenge) {
			challenges.Add(challenge);
		}

		public static Challenge Get(byte id) {
			if (challenges.Count > id - 1 && id > 0) {
				return challenges[id - 1];
			}

			return null;
		}
	}
}