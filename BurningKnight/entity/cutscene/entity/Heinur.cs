using System;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.bk;
using BurningKnight.util;
using Lens;
using Lens.util;
using Lens.util.camera;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.cutscene.entity {
	public class Heinur : CutsceneEntity {
		public bool Attract;
		public Action Callback;
		
		public override void AddComponents() {
			base.AddComponents();

			Width = 42;
			Height = 42;
			
			AddComponent(new BkGraphicsComponent("heinur"));
			AddComponent(new SensorBodyComponent(0, 0, 42, 42));

			GetComponent<SensorBodyComponent>().Body.LinearDamping = 2;
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (!Attract) {
				return;
			}

			foreach (var p in Area.Tagged[Tags.LocalPlayer]) {
				var dx = p.DxTo(this);
				var dy = p.DyTo(this);

				var d = MathUtils.Distance(dx, dy);

				if (d <= 24) {
					AnimationUtil.Explosion(Center);
					Done = true;
					Callback();

					Camera.Instance.Shake(20);
					Engine.Instance.Flash = 2;

					return; 
				}

				var a = MathUtils.Angle(dx, dy);
				var force = 360 * dt;

				if (d <= 64) {
					force *= 2;
				}
				
				p.GetComponent<RectBodyComponent>().Velocity += new Vector2((float) Math.Cos(a) * force, (float) Math.Sin(a) * force);
				a += (float) Math.PI;
				GetComponent<SensorBodyComponent>().Velocity += new Vector2((float) Math.Cos(a) * force, (float) Math.Sin(a) * force);
			}
		}
	}
}