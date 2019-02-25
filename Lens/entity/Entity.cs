using System;
using System.Collections.Generic;
using Lens.entity.component;
using Lens.entity.component.graphics;
using Microsoft.Xna.Framework;

namespace Lens.entity {
	public class Entity {
		public Area Area;
		
		public bool Active = true;
		public bool Visible = true;
		public bool AlwaysActive;
		public bool AlwaysVisible;
		public bool OnScreen;

		public byte Depth;
		
		#region Bounds and position
		
		public Vector2 Position = new Vector2();
		public float Width;
		public float Height;

		public float X => Position.X;
		public float Y => Position.Y;
		public Vector2 Center => new Vector2(Position.X + Width / 2, Position.Y + Height / 2);
		public float CenterX => Position.X + Width / 2;
		public float CenterY => Position.Y + Height / 2;
		public float Right => Position.X + Width;
		public float Bottom => Position.Y + Height;

		#endregion
		
		#region Entity tags

		private int tag;
		
		public int Tag {
			get { return tag; }
			
			set {
				if (tag != value && Area != null) {
					for (int i = 0; i < BitTag.Total; i++) {
						int check = 1 << i;
						bool add = (value & check) != 0;
						bool has = (Tag & check) != 0;

						if (has != add) {
							if (add) {
								Area.Tags[i].Add(this);
							} else {
								Area.Tags[i].Remove(this);
							}
						}
					}	

					tag = value;
				}	
			}
		}

		public bool HasTag(int tag) {
			return (this.tag & tag) != 0;
		}

		public void AddTag(int tag) {
			Tag |= tag;
		}

		public void RemoveTag(int tag) {
			Tag &= ~tag;
		}
		
		#endregion
		
		#region Entity logic

		private GraphicsComponent graphicsComponent;
		private Dictionary<Type, Component> components = new Dictionary<Type, Component>();
		
		public virtual void Init() {
			AddComponents();
		}

		protected virtual void AddComponents() {
			
		}

		public virtual void Destroy() {
			foreach (var component in components.Values) {
				component.Destroy();
			}
		}
		
		public virtual void Update(float dt) {
			foreach (var component in components.Values) {
				component.Update(dt);
			}
		}

		public virtual void Render() {
			graphicsComponent?.Render();
		}

		public virtual void RenderDebug() {
			
		}

		public void RemoveSelf() {
			if (Area != null) {
				Area.Remove(this);
				Area = null;
			}
		}

		public void AddComponent(Component component) {
			components[component.GetType()] = component;

			component.Entity = this;
			component.Init();
		}

		public void SetGraphicsComponent(GraphicsComponent component) {
			graphicsComponent = component;
			AddComponent(component);
		}
				
		public T GetComponent<T>() where T : Component {
			return (T) components[typeof(T)];
		}

		#endregion
		
		#region Distance and angle

		public float DxTo(Entity enity) {
			return enity.CenterX - CenterX;
		}
		
		public float DyTo(Entity enity) {
			return enity.CenterY - CenterY;
		}

		public float DistanceTo(Entity entity) {
			float dx = DxTo(entity);
			float dy = DyTo(entity);
			
			return (float) Math.Sqrt(dx * dx + dy * dy);
		}
		
		public float AngleTo(Entity entity) {
			return (float) Math.Atan2(DyTo(entity), DxTo(entity));
		}
		
		#endregion
		
		#region Simple collision

		public bool Overlaps(Entity entity) {
			return !(entity.X > Right ||
			         entity.Right < X ||
			         entity.Y > Bottom ||
			         entity.Bottom < Y);
		}

		public bool Contains(Entity entity) {
			return entity.X >= X && entity.Right <= Right
			                     && entity.Y >= Y && entity.Bottom <= Bottom;
		}

		public bool Contains(Vector2 point) {
			return point.X >= X && point.X <= Right
													&& point.Y >= Y && point.Y <= Bottom;
		}
		
		#endregion
	}
}