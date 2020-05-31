using System.Collections.Generic;

namespace BurningKnight.state {
	public class Credits {
		public static List<string[]> Text = new List<string[]>();

		private static void Add(string[] text) {
			Text.Add(text);
		}
		
		static Credits() {
			Add(new[] {
				"A game by",
				"Egor Dorichev"
			});
			
			Add(new[] {
				"Developed between May 2018",
				"And June 2020"
			});
			
			Add(new[] {
				"Big thanks to"
			});
			
			Add(new[] {
				"Egor Dorichev (egordorichev)",
				"Programming, Game Design",
				"Art & Russian Translation"
			});
			
			Add(new[] {
				"Viktor Kraus (ViktorRKraus)",
				"Sound Design"
			});
			
			Add(new[] {
				"Jose Ramon (Bibiki)",
				"Early Sound Design & Composing"
			});
			
			Add(new[] {
				"FailPositive",
				"Secret Tracks"
			});

			Add(new[] {
				"Ian Crail (RetrosaurDev)",
				"Early Art"
			});

			Add(new[] {
				"Mate Cziner",
				"Early Art"
			});

			Add(new[] {
				"Roman Gonzo",
				"Early Art"
			});

			Add(new[] {
				"somepx",
				"Awesome Fonts"
			});

			Add(new[] {
				"Cubey",
				"Polish Translation"
			});

			Add(new[] {
				"ArKeid0s & Memeber",
				"French Translation"
			});

			Add(new[] {
				"Nikithn",
				"Belarusian Translation"
			});

			Add(new[] {
				"Rick Nya",
				"Portugese Translation"
			});

			Add(new[] {
				"nade_",
				"Italian Translation"
			});

			Add(new[] {
				"Andreas May (Maanex)",
				"German Translation & Extensive Playtesting & Wiki Development"
			});

			Add(new[] {
				"Brastin",
				"Extensive Playtesting & Inspiration"
			});

			Add(new[] {
				"Dean aka DSF100",
				"Extensive Playtesting & Inspiration"
			});

			Add(new[] {
				"Nufflee",
				"Early Development & Playtesting"
			});

			Add(new[] {
				"Nikita Grichan (anivire)",
				"Trailers & Extensive Playtesting"
			});

			Add(new[] {
				"Special thanks to everyone",
				"from notsosolo, especially to:"
			});

			Add(new[] {
				"wombatstuff",
				"Bilge Kaan",
				"Alex Clay",
				"Gaziter",
				"BenStarDEV",
				"Felipe",
				"torcado",
				"Salman_Shh"
			});
			
			Add(new[] {
				"And you!"
			});
			
			Add(new[] {
				"Thank you for playing my game"
			});
			
			Add(new[] {
				"Egor."
			});
		}
	}
}