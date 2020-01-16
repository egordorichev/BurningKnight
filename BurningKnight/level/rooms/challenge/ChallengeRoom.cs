using System;
using System.Collections.Generic;
using BurningKnight.assets.items;
using BurningKnight.entity.item;
using BurningKnight.entity.item.stand;
using BurningKnight.entity.room;
using BurningKnight.level.entities.chest;
using BurningKnight.level.rooms.special;
using BurningKnight.util.geometry;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.challenge {
	public class ChallengeRoom : SpecialRoom {
		public override void SetupDoors(Level level) {
			foreach (var door in Connected.Values) {
				door.Type = DoorPlaceholder.Variant.Challenge;
			}
		}

		public override bool CanConnect(RoomDef R, Dot P) {
			if (P.X == Left || P.X == Right) {
				return false;
			}
			
			return base.CanConnect(R, P);
		}

		public override void Paint(Level level) {
			var center = GetCenter() * 16 + new Vector2(8);
			
			if (Rnd.Chance(20)) {
				var stand = new ItemStand();
				level.Area.Add(stand);
				stand.Center = center;

				stand.SetItem(Items.CreateAndAdd(Items.Generate(ItemPool.Treasure), level.Area), null);
			} else {
				var chests = new List<Chest>();
				var i = Rnd.Int(4);

				switch (i) {
					case 0: {
						for (var k = 0; k < 2; k++) {
							chests.Add(new RedChest());	
						}
						
						break;
					}

					case 1: {
						for (var k = 0; k < 4; k++) {
							chests.Add(new GoldChest());	
						}

						break;
					}

					case 2: {
						for (var k = 0; k < 3; k++) {
							chests.Add(new WoodenChest());	
						}

						break;
					}

					default: {
						chests.Add(new GoldChest());	
						chests.Add(new WoodenChest());	
						chests.Add(new RedChest());	
						
						break;
					}
				}

				var j = 0;
				var cn = chests.Count;
				
				foreach (var c in chests) {
					level.Area.Add(c);
					c.Center = center + new Vector2((j - cn / 2f) * 16, 0);
					
					j++;
				}
			}
		}

		public override void ModifyRoom(Room room) {
			base.ModifyRoom(room);
			room.AddController("bk:challenge");
		}
	}
}