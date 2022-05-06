using System;
using System.Runtime.Serialization;

namespace Persistentie {
	public class ToestellenZetInHerstellingException : Exception {
		public ToestellenZetInHerstellingException() {
		}

		public ToestellenZetInHerstellingException(string message) : base(message) {
		}
	}
}