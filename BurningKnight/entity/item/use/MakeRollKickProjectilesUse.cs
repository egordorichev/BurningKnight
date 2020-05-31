using System;
using BurningKnight.assets.lighting;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.item.util;
using BurningKnight.entity.projectile;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.util;
using Lens.util.camera;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.item.use {
	public class MakeRollKickProjectilesUse : ItemUse {
		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent cse) {
				if (!(Item.Owner is Player pl) || !(pl.GetComponent<StateComponent>().StateInstance is Player.RollState)) {
					return base.HandleEvent(e);
				}
				
				if (cse.Entity is Projectile p && !(p.Owner is Player) && p.CanBeReflected) {
					var owner = Item.Owner;
					var a = owner.AngleTo(p.Owner);

					p.Owner = owner;
					p.Damage *= 2f;

					p.Pattern?.Remove(p);

					var b = p.BodyComponent;
					var d = Math.Max(400, b.Velocity.Length() * 1.8f);

					b.Velocity = MathUtils.CreateVector(a, d);
					
					if (p.TryGetComponent<LightComponent>(out var l)) {
						l.Light.Color = MeleeArc.ReflectedColor;
					}

					p.Color = ProjectileColor.Yellow;

					Camera.Instance.ShakeMax(4f);
					owner.GetComponent<AudioEmitterComponent>().EmitRandomizedPrefixed("projectile_reflected", 2);
				}
			}
			
			return base.HandleEvent(e);
		}
	}
}