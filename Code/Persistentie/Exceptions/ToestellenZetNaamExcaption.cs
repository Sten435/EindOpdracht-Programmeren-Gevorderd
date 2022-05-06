using System;
using System.Runtime.Serialization;

namespace Persistentie {
	public class ToestellenZetNaamExcaption : Exception {
		public ToestellenZetNaamExcaption() {
		}

		public ToestellenZetNaamExcaption(string message) : base(message) {
		}
	}
}