using BurningKnight.entity.component;
using Lens.util.file;
using Random = Lens.util.math.Random;

namespace BurningKnight.level.entities.plant {
	public class Plant : Prop {
		private static string[] variants = {
			"plant_a", "plant_b", "plant_c", "plant_d", "plant_e", "plant_f",
			"plant_g", "plant_i", "plant_j", "plant_k", "plant_l", "plant_m"
		};

		private byte variant;

		public override void Init() {
			base.Init();
			variant = (byte) Random.Int(variants.Length * 2);
		}

		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new ShadowComponent());
		}

		public override void PostInit() {
			base.PostInit();

			var g = new PlantGraphicsComponent("props", $"{variants[variant % variants.Length]}{(variant >= variants.Length ? "s" : "")}");
			AddComponent(g);

			Width = g.Sprite.Width;
			Height = g.Sprite.Height;
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			variant = stream.ReadByte();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteByte(variant);
		}
	}
}