using System;
using System.Collections.Generic;
using Lens.util;

namespace Lens.entity {
	public class EventListener {
		private Dictionary<Type, List<Subscriber>> subscribers = new Dictionary<Type, List<Subscriber>>();
		
		public bool Handle(Event e) {
			if (subscribers.TryGetValue(e.GetType(), out var subs)) {
				foreach (var sub in subs) {
					if (sub.HandleEvent(e)) {
						return true;
					}
				}
			}

			return false;
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
	}
}