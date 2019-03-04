using System;
using System.Collections.Generic;

namespace Lens.entity {
	public class EventListener {
		private Dictionary<Type, List<Subscriber>> subscribers = new Dictionary<Type, List<Subscriber>>();
		
		public void Handle(Event e) {
			if (subscribers.TryGetValue(e.GetType(), out var subs)) {
				foreach (var sub in subs) {
					sub.HandleEvent(e);
				}
			}
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
	}
}