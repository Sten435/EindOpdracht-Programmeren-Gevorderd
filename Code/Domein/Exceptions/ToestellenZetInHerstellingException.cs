using System;
using System.Runtime.Serialization;

namespace Domein {
	public class ToestellenZetInHerstellingException : Exception {
		public ToestellenZetInHerstellingException() {
		}

		public ToestellenZetInHerstellingException(string message) : base(message) {
		}
	}
}