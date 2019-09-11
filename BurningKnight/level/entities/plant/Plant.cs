using BurningKnight.assets.lighting;
using BurningKnight.entity.component;
using Lens.util.file;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.level.entities.plant {
	public class Plant : Prop {
		private static string[] variants = {
			"plant_a", "plant_b", "plant_c", "plant_d", "plant_e", "plant_f",
			"plant_g", "plant_i", "plant_j", "plant_k", "plant_l", "plant_m", "plant_n"
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

			var s = variants[variant % variants.Length];
			
			var g = new PlantGraphicsComponent("props", $"{s}{(variant >= variants.Length ? "s" : "")}");
			AddComponent(g);

			Width = g.Sprite.Width;
			Height = g.Sprite.Height;

			if (s == "plant_l") {
				AddComponent(new LightComponent(this, 16, new Color(0.3f, 0.3f, 1f, 1f)));
			} else if (s == "plant_j") {
				AddComponent(new LightComponent(this, 16, new Color(1f, 0.3f, 0.3f, 1f)));
			}
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