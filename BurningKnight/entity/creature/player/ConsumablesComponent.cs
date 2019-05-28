using BurningKnight.assets.particle;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.state;
using BurningKnight.util;
using Lens.entity;
using Lens.input;
using Lens.util;
using Lens.util.file;
using Lens.util.math;

namespace BurningKnight.entity.creature.player {
	public class ConsumablesComponent : ItemComponent {
		private byte bombs = 1;
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
				TotalNow = totalNow,
				Type = type
			});	
		}

		public override bool HandleEvent(Event e) {
			if (e is ItemCheckEvent ev) {
				var type = ev.Item.Type;
				
				if (type == ItemType.Bomb || type == ItemType.Key || type == ItemType.Coin) {
					ev.Item.Use((Player) Entity);
					ev.Item.Done = true;
					
					for (var i = 0; i < 4; i++) {
						Entity.Area.Add(new ParticleEntity(Particles.Dust()) {
							Position = ev.Item.Center, 
							Particle = {
								Scale = Random.Float(0.4f, 0.8f)
							}
						});
					}
					
					
					return true;
				}
			}
			
			return base.HandleEvent(e);
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (bombs > 0 && Input.WasPressed(Controls.Bomb, GetComponent<GamepadComponent>().Controller)) {
				Bombs--;
				
				var bomb = new Bomb();
				Entity.Area.Add(bomb);
				bomb.Center = Entity.Center;
				bomb.MoveToMouse();
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