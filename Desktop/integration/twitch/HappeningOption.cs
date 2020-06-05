using BurningKnight.assets;
using BurningKnight.entity.twitch.happening;
using Lens.assets;
using Lens.entity;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace Desktop.integration.twitch {
	public class HappeningOption {
		public Vector2 Position;

		public int Num;
		public string Id;
		public string Label;
		public string Name;
		public float LabelWidth;
		public int Votes;
		public int Percent;
		public Happening Happening;

		public HappeningOption(string id, int i) {
			Happening = HappeningRegistry.Get(id);
			Id = id;

			var name = Locale.Get($"happening_{id}");

			Num = i;
			Name = name.ToLower();
			Label = $"#{i} {name}";
			LabelWidth = Font.Small.MeasureString($"{Label} (100%)").Width;
		}

		public void Render() {
			Graphics.Print($"{Label} ({Percent}%)", Font.Small, Position);
		}
	}
}