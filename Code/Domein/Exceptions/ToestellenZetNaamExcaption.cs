using System;
using System.Runtime.Serialization;

namespace Domein {
	public class ToestellenZetNaamExcaption : Exception {
		public ToestellenZetNaamExcaption() {
		}

		public ToestellenZetNaamExcaption(string message) : base(message) {
		}
	}
}