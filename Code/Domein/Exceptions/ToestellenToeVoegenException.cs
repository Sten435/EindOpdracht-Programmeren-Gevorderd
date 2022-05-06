using System;
using System.Runtime.Serialization;

namespace Domein {
	public class ToestellenToeVoegenException : Exception {
		public ToestellenToeVoegenException() {
		}

		public ToestellenToeVoegenException(string message) : base(message) {
		}
	}
}