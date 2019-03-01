using System;
using System.Collections.Generic;
using Lens.entity.component;
using Lens.entity.component.graphics;
using Lens.util;
using Microsoft.Xna.Framework;

namespace Lens.entity {
	public class Entity {
		public Area Area;
		
		public bool Active = true;
		public bool Visible = true;
		public bool AlwaysActive;
		public bool AlwaysVisible;
		public bool OnScreen;
		public bool Done;
		
		public int Depth;
		
		#region Bounds and position
		
		public Vector2 Position = new Vector2();
		public float Width;
		public float Height;

		public float X {
			get => Position.X;
			set => Position.X = value;
		}
		
		public float Y {
			get => Position.Y;
			set => Position.Y = value;
		}
		
		public Vector2 Center {
			get => new Vector2(Position.X + Width / 2, Position.Y + Height / 2);
			set {
				Position.X = value.X - Width / 2;
				Position.Y = value.Y - Height / 2;
			}
		}
		
		public float CenterX {
			get => Position.X + Width / 2;
			set => Position.X = value - Width / 2;
		}
		
		public float CenterY {
			get => Position.Y + Height / 2;
			set => Position.Y = value - Height / 2;
		}
		
		public float Right => Position.X + Width;
		public float Bottom => Position.Y + Height;

		#endregion
		
		#region Entity tags

		private int tag;
		
		public int Tag {
			get => tag;

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
			return (this.tag & 1 << tag) != 0;
		}

		public void AddTag(int tag) {
			Tag |= 1 << tag;
		}

		public void RemoveTag(int tag) {
			Tag &= ~(1 << tag);
		}
		
		#endregion
		
		#region Entity logic

		private GraphicsComponent graphicsComponent;
		protected Dictionary<Type, Component> components;
		
		public virtual void Init() {
			if (components == null) {
				AddComponents();
			}
		}

		public virtual void AddComponents() {
			components = new Dictionary<Type, Component>();
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

		public void RemoveComponent<T>() {
			var type = typeof(T);
			
			if (components.TryGetValue(type, out var component)) {
				component.Destroy();
				components.Remove(type);
			}
		}

		public void SetGraphicsComponent(GraphicsComponent component) {
			graphicsComponent = component;
			AddComponent(component);
		}
				
		public T GetComponent<T>() where T : Component {
			return (T) components[typeof(T)];
		}

		public bool TryGetCompoenent<T>(out T t) where T : Component {
			if (components.TryGetValue(typeof(T), out var tmp)) {
				t = (T) tmp;
				return true;
			}


			t = default(T);
			return false;
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