using System;
using System.Runtime.Serialization;

namespace Persistentie {
	public class ToestellenToeVoegenException : Exception {
		public ToestellenToeVoegenException() {
		}

		public ToestellenToeVoegenException(string message) : base(message) {
		}
	}
}