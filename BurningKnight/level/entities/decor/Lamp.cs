using System;
using BurningKnight.assets.lighting;
using BurningKnight.entity.component;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.entities.decor {
	public class Lamp : SlicedProp {
		public override void Init() {
			base.Init();

			Width = 8;
			Height = 12;
			Sprite = "lamp";

			t = Rnd.Float(6);
		}

		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new LightComponent(this, 32f, new Color(1f, 0.8f, 0.3f, 1f)));
			AddComponent(new ShadowComponent());
		}

		private float t;
		
		public override void Update(float dt) {
			base.Update(dt);
			t += dt * 0.5f;
			GetComponent<LightComponent>().Light.Radius = 38f + (float) Math.Cos(t) * 6;
		}
	}
}