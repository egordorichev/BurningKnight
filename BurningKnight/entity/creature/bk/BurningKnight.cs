using System;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.mob.boss;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.ui;
using Lens;
using Lens.entity;
using Lens.entity.component.logic;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.bk {
	public class BurningKnight : Boss {
		private HealthBar healthBar;
	
		public override void AddComponents() {
			base.AddComponents();
			
			RemoveTag(Tags.LevelSave);
			RemoveTag(Tags.MustBeKilled);
			
			AddTag(Tags.PlayerSave);
			AddTag(Tags.BurningKnight);

			Width = 42;
			Height = 42;
			TouchDamage = 0;

			AddComponent(new RectBodyComponent(0, 0, Width, Height, BodyType.Dynamic, true));
			
			// FIXME: TMP sprite and size, obv
			AddComponent(new AnimationComponent("burning_knight"));

			var health = GetComponent<HealthComponent>();
			health.InitMaxHealth = 1024;
			
			GetComponent<StateComponent>().Become<IdleState>();
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (healthBar == null) {
				healthBar = new HealthBar(this);
				Engine.Instance.State.Ui.Add(healthBar);
			}
		}

		public override bool HandleEvent(Event e) {
			if (e is RoomChangedEvent rce) {
				
			}
			
			return base.HandleEvent(e);
		}

		#region Burning Knight States
		public class IdleState : CreatureState<BurningKnight> {
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
				
				if (d > 16f) {
					var s = dt * 8;
					body.Velocity += new Vector2(dx * s, dy * s);
				}

				body.Velocity -= body.Velocity * (dt * 2);
			}
		}
		#endregion
	}
}