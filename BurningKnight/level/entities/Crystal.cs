using System;
using BurningKnight.entity;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.level.tile;
using BurningKnight.save;
using BurningKnight.state;
using Lens.entity;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.entities {
	public class Crystal : SaveableEntity {
		public override void AddComponents() {
			base.AddComponents();
			
			Depth = Layers.Entrance;
			
			AddComponent(new SliceComponent("props", "crystal"));
			AddComponent(new ExplodableComponent());

			Width = 5;
			Height = 5;
		}

		public override bool HandleEvent(Event e) {
			if (e is ExplodedEvent ee) {
				if (ee.Origin.DistanceTo(this) < 32) {
					Done = true;

					var exit = new Exit();
					Area.Add(exit);
					exit.To = 13;
					
					var x = (int) Math.Floor(CenterX / 16);
					var y = (int) Math.Floor(CenterY / 16);
					var p = new Vector2(x * 16 + 8, y * 16 + 8);
			
					exit.Center = p;

					Painter.Fill(Run.Level, x - 1, y - 1, 3, 3, Tiles.RandomFloor());
					Run.Level.ReTileAndCreateBodyChunks(x - 1, y - 1, 3, 3);
				}
			}
			
			return base.HandleEvent(e);
		}
	}
}