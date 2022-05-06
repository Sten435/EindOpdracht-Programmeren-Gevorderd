using System;
using System.Runtime.Serialization;

namespace Domein {
	public class ToestellenVerwijderenException : Exception {
		public ToestellenVerwijderenException() {
		}

		public ToestellenVerwijderenException(string message) : base(message) {
		}
	}
}