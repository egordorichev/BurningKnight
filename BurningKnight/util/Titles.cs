using System;
using Lens.util.math;

namespace BurningKnight.util {
	public static class Titles {
		private static Birthday[] birthdays = {
			new Birthday(29, 6), // Egor
			new Birthday(25, 9), // Mate
			new Birthday(21, 2), // Bibiki
			new Birthday(14, 6), // Viktor
    };
		
		public static string Generate() {
			var now = DateTime.Now;
			
			foreach (var b in birthdays) {
				if (b.Day == now.Day && b.Month == now.Month) {
					return birthdayTitles[Rnd.Int(birthdayTitles.Length)];
				}
			}

			if (Rnd.Chance(0.01f)) {
				return "You feel lucky";
			}

			return titles[Rnd.Int(titles.Length)];
		}

		private static string[] titles = {
			"oh no my game... EXPLODED!!!",
			"Fireproof",
			"Might burn",
			"'Friendly' fire",
			"Get ready to burn",
			"Do you need some heat?",
			"sqrt(-1) love you!",
			"Han shot first",
			"Sneak 100",
			"BBQ is ready!",
			"Hot sales!",
			"AAAAAA",
			"It burns burns burns",
			"Not for children under -1",
			"Unhandled fire",
			"Chili music",
			"Fire trap",
			"On-fire",
			"Hot potatoo",
			"Also try Nuclear Throne",
			"undefined",
			"If only you could talk to the enemies...",
			"It slit. Thank you, good knight",
			"Why aren't you in fullscreen?",
			"so fire, so good",
			"Please don't feed the goblins",
			"Burn, baby, burn",
			"Only you can prevent the absence of forest fire",
			"If at first you don't success, just burn everything",
			"Attention: may cause sick burns",
			"What doesn't make you stronger kills you",
			"Is that a plane? Is that a bird? It is a flying fire!",
			"Of course we still love you",
			"Just read the instructions",
			"Your CPU is close to overheating",
			"This is fine",
			"Ice cool",
			"Overheat alarm was triggered",
			"Burn",
			"Being a Knight does not guarantee burning",
			"Burning does not guarantee becoming a knight",
			"The thing of fire",
			"Sick burn, knight",
			"Wombats are dangerous",
			"Stuff happens",
			"1000% screen shake",
			"Water, fire, air, and... chocolate!",
			"What can be cooler than fire?",
			"???",
			"!!!",
			"Ya feel lucky",
			"Today is the day",
			"Run, gobbo, run!",
			"Oszor is watching you",
			"Always wear dry socks",
			"But can you procedurally generate titles?",
			"Now with more Math.random() than ever!",
			"Never gets the cold feet",
			"If you play with fire...",
			"Now with more pathos than ever!",
			"Now with less bugs than ever!",
			"Now with more code than ever!",
			"There is a throne, however it's made of metal and not uranium...",
			"Не ожидали?",
			"Flameberrry",
			"This is a title mimic",
			"Lets play",
			"Remains functional up to 4000 degrees Kelvin",
			"Caution: might contain knights",
			"/!\\ Do not throw oil at the knight!",
			"/!\\ /!\\ /!\\",
			"xD",
			"All your knights are belong to us",
			"It's dangerous to go alone, take this!",
			"Note: This game contains flammables",
			"We didn't start the fire",
			"I'm on fire!",
			"The Knight is lava",
			"Hello fire, my old friend",
			"I'm here to talk with you again",
			"Hot n' spicy!",
			"This it lit",
			"Вообще огонь",
			"Roses are red, violets are blue, burning is knight and gobbo is you",
			"Roses are red, violets are blue, water boils at 100°C and freezes at 0°C - @DSF100",
			"Burn CPU, BURN",
			"Please, read the instructions",
			"¯\\_(ツ)_/¯",
			"☉_☉",
			"⌐■-■",
			"Open fire",
			"In case of fire break the monitor",
			"In case of fire backup the saves",
			"You'd better get burning",
			"Why are you reading this?",
			"Yes",
			"No",
			"No U",
			"Uno",
			"The ducks in the park are free",
			"The Trickster",
			"Sneak 100",
			"Title 100",
			"Stonks",
			"Confused Stonks",
			"Not Stonks",
			"Never gonna give you up",
			"Oh no",
			"Nobody:",
			"Do or do not there is no try",
			"NaN",
			"[object Object]",
			"An interesting title",
			"I'm once again asking for your emotional support",
			"Outstanding move",
			"Never gonna let you down",
			"You know the rules and so do I",
			"F",
			"Nobody expects the Spanish Inquisition!"
		};

		private static string[] birthdayTitles = {
			"Happy burning!",
			"It's a good day to die hard",
			"Someone is not burning today",
			"Burning party",
			"Fire hard!",
			"Today is a special day",
			"I need a cake",
			"I hope the presents won't explode"
		};
	}
}