using System;
using BurningKnight.entity.component;
using Lens.assets;
using Lens.graphics;
using Lens.util;
using Lens.util.file;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.mob.jungle {
	public class ManEater : Mob {
		private static TextureRegion jointTexture;
		private static TextureRegion stemTexture;
		
		private Vector2 joint;
		
		protected override void SetStats() {
			base.SetStats();

			if (jointTexture == null) {
				var anim = Animations.Get("maneater_stem");
				
				jointTexture = anim.GetSlice("joint");
				stemTexture = anim.GetSlice("stem");
			}
			
			AddAnimation("maneater");
			SetMaxHp(20);
			
			Become<IdleState>();

			Flying = true;
			Depth = Layers.FlyingMob;
			AlwaysVisible = true; // Hackz :x

			var body = new CircleBodyComponent(1, 2.5f, 5.5f, BodyType.Dynamic, true);
			AddComponent(body);

			body.KnockbackModifier = 0.5f;
			body.Body.LinearDamping = 5;

			Width = 13;

			var a = GetComponent<MobAnimationComponent>();

			a.Centered = true;
			a.OriginX = 6.5f;
			a.OriginY = 8;
			a.AddOrigin = false;
			
			RemoveComponent<ShadowComponent>();
		}

		private bool set;

		public override void Update(float dt) {
			if (!set) {
				set = true;
				joint = Center;
			}
			base.Update(dt);
		}

		public override void Save(FileWriter stream) {
			var ocenter = Center;
			Center = joint;
			base.Save(stream);
			Center = ocenter;
		}

		public override void Render() {
			var a = AngleTo(joint) - Math.PI;
			var d = DistanceTo(joint);
			var or = new Vector2(0, 2);

			for (var i = 0; i < Math.Ceiling(d / 5) * 5; i += 5) {
				Graphics.Render(stemTexture, joint + MathUtils.CreateVector(a, i), (float) a, or);
			}

			Graphics.Render(jointTexture, joint, 0, jointTexture.Center);
			GetComponent<MobAnimationComponent>().Angle = (float) a;
			base.Render();
		}
		
		#region Man Eater States
		public class IdleState : SmartState<ManEater> {
			private float a;
			
			public override void Update(float dt) {
				base.Update(dt);
				
				a = Self.Target == null ? T * 0.5f : (float) MathUtils.LerpAngle(a, Self.Target.AngleTo(Self.joint) 
				  - (float) Math.PI + (float) Math.Cos(T * 2) * 0.25f, dt);

				Self.Center = Self.joint + MathUtils.CreateVector(a, (float) Math.Cos(T * 0.25f) * 32 + 64f);
			}
		}
		#endregion
	}
}