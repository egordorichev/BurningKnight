using System;
using System.Collections.Generic;
using Lens.Asset;
using Lens.Graphics.Animation;

namespace Lens.Entities.Components.Graphics {
	public class SpriteComponent : GraphicsComponent {
		public Dictionary<string, Animation> States = new Dictionary<string, Animation>();
		
		private String name;
		private Animation current;
		private string currentName;
		
		public SpriteComponent(string name) {
			this.name = name;
		}
		
		public void Set(string animationName, bool reset = true) {
			if (currentName == animationName) {
				return;
			}
			
			var animation = Animations.Get($"{name}_${animationName}");

			if (animation == null) {
				return;
			}

			current = animation;
			currentName = animationName;

			if (reset) {
				animation.Reset();
			}
		}
	}
}