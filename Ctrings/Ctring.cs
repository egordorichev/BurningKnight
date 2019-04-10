using System.Collections.Generic;
using System.Text;
using Ctrings.token;
using Microsoft.Xna.Framework;

namespace Ctrings {
	public class Ctring {
		private string text;
		private CFont font;
		private List<Token> tokens = new List<Token>();
		
		public string Text {
			set {
				if (text != value) {
					text = value;
					Parse();
				}
			}
		}
		
		internal Vector2 CurrentScale = Vector2.One;
		internal Color CurrentColor = Color.White;

		public Ctring(string text, CFont font) {
			Text = text;
			this.font = font;
		}

		private void Parse() {
			var builder = new StringBuilder();
			Token token = null;
			
			for (var i = 0; i < text.Length; i++) {
				var c = text[i];

				switch (c) {
					case '#': {
						var str = builder.ToString();

						if (str.Length > 0) {
							builder.Clear();

							if (token != null) {
								token.Text = str;
								token = null;
							} else {
								tokens.Add(new TextToken {
									Text = str
								});
							}
						}

						if (i == text.Length - 1) {
							break;
						}

						i++;
						c = text[i];
						
						switch (c) {
							case 'w':
								token = new WaveToken();
								break;
						}
						
						break;
					}

					default: {
						builder.Append(c);
						break;
					}
				}
			}
			
			var s = builder.ToString();

			if (s.Length == 0) {
				return;
			}
			
			if (token != null) {
				token.Text = s;
				token = null;
			} else {
				tokens.Add(new TextToken {
					Text = s
				});
			}
		}
	}
}