using System;
using BurningKnight.entity.component;
using BurningKnight.entity.item;
using BurningKnight.ui;
using Lens;
using Lens.entity.component.logic;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.mob {
	public class He : Mob {
		private HealthBar healthBar;
	
		public override void AddComponents() {
			base.AddComponents();
			
			RemoveTag(Tags.LevelSave);
			RemoveTag(Tags.MustBeKilled);
			
			AddTag(Tags.PlayerSave);
			AddTag(Tags.BurningKnight);

			Width = 42;
			Height = 42;

			AddComponent(new RectBodyComponent(0, 0, Width, Height, BodyType.Dynamic, true));
			
			// FIXME: TMP sprite and size, obv
			AddComponent(new AnimationComponent("burning_knight"));

			var health = GetComponent<HealthComponent>();
			health.InitMaxHealth = 1024;
			
			GetComponent<StateComponent>().Become<IdleState>();
		}

		public void SetLamp(Item item) {
			// todo: take into the account
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (healthBar == null) {
				healthBar = new HealthBar(this);
				Engine.Instance.State.Ui.Add(healthBar);
			}
		}
		
		#region Burning Knight States
		public class IdleState : CreatureState<He> {
			public override void Update(float dt) {
				base.Update(dt);

				var room = Self.Target?.GetComponent<RoomComponent>().Room;

				if (room == null) {
					return;
				}

				var dx = Self.DxTo(room);
				var dy = Self.DyTo(room);
				var d = (float) Math.Sqrt(dx * dx + dy * dy);

				var body = Self.GetAnyComponent<BodyComponent>();
				
				if (d > 32f) {
					var s = dt * 4;
					body.Velocity += new Vector2(dx * s, dy * s);
				}

				body.Velocity -= body.Velocity * (dt * 2);
			}
		}
		#endregion
	}
}