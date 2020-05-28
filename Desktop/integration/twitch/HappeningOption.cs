using BurningKnight.assets;
using Desktop.integration.twitch.happening;
using Lens.assets;
using Lens.entity;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace Desktop.integration.twitch {
	public class HappeningOption {
		public Vector2 Position;

		public string Id;
		public string Label;
		public float LabelWidth;
		public int Votes;
		public Happening Happening;

		public HappeningOption(string id) {
			Happening = HappeningRegistry.Get(id);
			Id = id;
			Label = Locale.Get($"happening_{id}");
			LabelWidth = Font.Small.MeasureString(Label).Width;
		}

		public void Render() {
			Graphics.Print(Label, Font.Small, Position);
			Graphics.Print($"{Votes}", Font.Small, Position + new Vector2(0, 10));
		}
	}
}