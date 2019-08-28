using System;
using System.Collections.Generic;
using System.Linq;
using Lens.util;

namespace Lens.entity {
	public class EventListener {
		private Dictionary<Type, List<Subscriber>> subscribers = new Dictionary<Type, List<Subscriber>>();

		public void Copy(EventListener l) {
			foreach (var pair in l.subscribers) {
				if (!subscribers.ContainsKey(pair.Key)) {
					subscribers[pair.Key] = new List<Subscriber>();
				}
				
				subscribers[pair.Key].AddRange(pair.Value);
			}
			
			l.subscribers.Clear();
		}
		
		public bool Handle(Event e) {
			if (subscribers.TryGetValue(e.GetType(), out var subs)) {
				foreach (var sub in subs) {
					if (sub.HandleEvent(e)) {
						e.Handled = true;
					}
				}
			}

			return e.Handled;
		}

		public void Subscribe<T>(Subscriber s) where T : Event {
			var type = typeof(T);

			if (subscribers.TryGetValue(type, out var subs)) {
				subs.Add(s);
			} else {
				subs = new List<Subscriber>();
				subs.Add(s);
				
				subscribers[type] = subs;
			}
		}

		public void Unsubscribe<T>(Subscriber s) where T : Event {
			if (subscribers.TryGetValue(typeof(T), out var subs)) {
				subs.Remove(s);
			}
		}

		public void Destroy() {
			subscribers.Clear();
		}
	}
}