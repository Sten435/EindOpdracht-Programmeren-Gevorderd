using System;
using System.Runtime.Serialization;

namespace Persistentie {
	public class ToestellenVerwijderenException : Exception {
		public ToestellenVerwijderenException() {
		}

		public ToestellenVerwijderenException(string message) : base(message) {
		}
	}
}