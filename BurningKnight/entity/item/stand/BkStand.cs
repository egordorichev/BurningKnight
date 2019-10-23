using System;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.bk;
using BurningKnight.entity.events;
using BurningKnight.entity.fx;
using BurningKnight.level;
using BurningKnight.level.entities.decor;
using BurningKnight.level.tile;
using BurningKnight.state;
using Lens;
using Lens.entity;
using Lens.util.camera;
using Lens.util.timer;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.item.stand {
	public class BkStand : ItemStand {
		private bool triggered;
		private bool did;
		private float t;
		
		public override void Update(float dt) {
			base.Update(dt);
			
			if (!did && triggered) {
				Camera.Instance.Shake(0.5f);
				t += dt;

				if (t >= 1f) {
					did = true;

					var torches = GetComponent<RoomComponent>().Room.Tagged[Tags.Torch];
					var target = new Vector2(CenterX, Y - 16);

					foreach (var t in torches) { 
						var tr = (Torch) t;

						tr.XSpread = 1;
						tr.On = true;
						tr.Target = target;
					}
					
					Timer.Add(() => {
						foreach (var t in torches) {
							t.Done = true;
						}
					}, 1f);

					Timer.Add(() => {
						var bk = new entity.creature.bk.BurningKnight();
						Area.Add(bk);
						bk.Center = target;
				
						Camera.Instance.Shake(10);

						foreach (var t in torches) {
							t.Done = true;
						}

						Done = true;
					}, 2f);
				}
			}
		}

		public override bool HandleEvent(Event e) {
			if (e is ItemTakenEvent && !triggered) {
				Timer.Add(() => { triggered = true; }, 1f);
				
				Camera.Instance.Shake(12);
				
				var xx = (int) Math.Floor(CenterX / 16) * 16;
				var xy = (int) Math.Floor(CenterY / 16) * 16;
				var p = 32;

				var torches = GetComponent<RoomComponent>().Room.Tagged[Tags.Torch];

				foreach (var t in torches) {
					((Torch) t).On = false;
				}

				Timer.Add(() => {
					foreach (var t in torches) {
						var tr = (Torch) t;
						tr.On = true;
						tr.XSpread = 0.1f;
					}
				}, 3f);

				for (var x = xx - p; x < xx + Width + p; x += 16) {
					for (var i = 0; i < Random.Int(3, 9); i++) {
						Area.Add(new FireParticle {
								Position = new Vector2(x + Random.Float(-2, 18), xy - p + Random.Float(-2, 18)),
								Delay = Random.Float(0.5f),
								XChange = 0.1f,
								Scale = 0.3f,
								Vy = 8,
								T = 0.5f,
								B = 0
						});

						Area.Add(new FireParticle {
								Position = new Vector2(x + Random.Float(-2, 18), xy + Height + p - 16 + Random.Float(-2, 18)),
								Delay = Random.Float(0.5f),
								XChange = 0.1f,
								Scale = 0.3f,
								Vy = 8,
								T = 0.5f,
								B = 0
						});
					}
				}

				for (var y = xy; y < xy + Height; y += 16) {
					for (var i = 0; i < Random.Int(3, 9); i++) {
						Area.Add(new FireParticle {
								Position = new Vector2(xx + Random.Float(-2, 18) - p, y + Random.Float(-2, 18)),
								Delay = Random.Float(0.5f),
								XChange = 0.1f,
								Scale = 0.3f,
								Vy = 8,
								T = 0.5f,
								B = 0
						});

						Area.Add(new FireParticle {
								Position = new Vector2(xx + Width + p - 16 + Random.Float(-2, 18), y + Random.Float(-2, 18)),
								Delay = Random.Float(0.5f),
								XChange = 0.1f,
								Scale = 0.3f,
								Vy = 8,
								T = 0.5f,
								B = 0
						});
					}
				}
			}
			
			return base.HandleEvent(e);
		}
	}
}