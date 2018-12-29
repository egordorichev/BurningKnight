using System.Collections.Generic;
using Lens.Entities;
using Lens.Util;

namespace Lens.Pattern.Observer {
	public class Subject {
		public List<Observer> Observers = new List<Observer>();
		private List<ObserverEvent> messages = new List<ObserverEvent>();

		public void Add(Observer observer) {
			Observers.Add(observer);
		}

		public void Remove(Observer observer) {
			Observers.Remove(observer);
		}

		public void Notify(Entity entity, ObserverEvent message) {
			message.Entity = entity;
			messages.Add(message);
		}

		public void Update() {
			if (messages.Count == 0) {
				return;
			}

			foreach (var m in messages) {
				foreach (var o in Observers) {
					if (o.Observe(m)) {
						break;
					}
				}
			}

			messages.Clear();
		}
	}
}