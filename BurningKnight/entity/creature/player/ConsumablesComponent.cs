using System;
using BurningKnight.assets.particle;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.util;
using Lens.entity;
using Lens.input;
using Lens.util;
using Lens.util.file;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.player {
	public class ConsumablesComponent : ItemComponent {
		private byte bombs = 32; // fixme
		private byte keys;
		private byte coins;

		public int Bombs {
			set {
				var n = (byte) MathUtils.Clamp(0, 99, value);

				if (n != bombs && AcceptChange(n - bombs, n, ItemType.Bomb)) {
					bombs = n;
				}
			}
			
			get => bombs;
		}

		public int Keys {
			set {
				var n = (byte) MathUtils.Clamp(0, 99, value);

				if (n != keys && AcceptChange(n - keys, n, ItemType.Key)) {
					keys = n;
				}
			}
			
			get => keys;
		}
		
		public int Coins {
			set {
				var n = (byte) MathUtils.Clamp(0, 99, value);

				if (n != coins && AcceptChange(n - coins, n, ItemType.Coin)) {
					coins = n;
				}
			}
			
			get => coins;
		}
		
		private bool AcceptChange(int amount, int totalNow, ItemType type) {
			if (amount > 0) {
				return !Send(new ConsumableAddedEvent {
					Amount = amount,
					TotalNow =  totalNow,
					Type = type
				});	
			}

			return !Send(new ConsumableRemovedEvent {
				Amount = amount,
				TotalNow =  totalNow,
				Type = type
			});	
		}

		public override bool HandleEvent(Event e) {
			if (e is ItemCheckEvent ev) {
				var type = ev.Item.Type;
				
				if (type == ItemType.Bomb) {	
					Bombs += ev.Item.Count;
					return true;
				}
				
				if (type == ItemType.Key) {
					Keys += ev.Item.Count;
					return true;
				}
				
				if (type == ItemType.Coin) {
					Coins += ev.Item.Count;
					return true;
				}
			}
			
			return base.HandleEvent(e);
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (bombs > 0 && Input.WasPressed(Controls.Bomb)) {
				Bombs--;
				
				var bomb = new Bomb();
				bomb.Position = Entity.Center;
				// todo: vel to mouse
				
				Entity.Area.Add(bomb);

				var explosion = new ParticleEntity(Particles.Animated("explosion", "explosion"));
				explosion.Particle.Position = Entity.Center;
				explosion.Depth = 32;
				Entity.Area.Add(explosion);

				for (int i = 0; i < 4; i++) {
					explosion = new ParticleEntity(Particles.Animated("explosion", "smoke"));
					explosion.Particle.Position = Entity.Center;
					explosion.Depth = 31;
					Entity.Area.Add(explosion);

					var a = explosion.Particle.Angle - Math.PI / 2;//.ToRadians();
					var d = 16;

					explosion.Particle.Position += new Vector2((float) Math.Cos(a) * d, (float) Math.Sin(a) * d);
				}
			}
		}

		public override void Set(Item item) {
			if (item.Type == ItemType.Bomb) {
				Bombs += item.Count;
			} else if (item.Type == ItemType.Key) {
				Keys += item.Count;
			} else if (item.Type == ItemType.Coin) {
				Coins += item.Count;
			}
			
			item.Done = true;
		}

		protected override bool ShouldReplace(Item item) {
			return item.Type == ItemType.Bomb || item.Type == ItemType.Key || item.Type == ItemType.Coin;
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			
			stream.WriteByte(bombs);
			stream.WriteByte(keys);
			stream.WriteByte(coins);
		}

		public override void Load(FileReader reader) {
			base.Load(reader);

			bombs = reader.ReadByte();
			keys = reader.ReadByte();
			coins = reader.ReadByte();
		}
	}
}