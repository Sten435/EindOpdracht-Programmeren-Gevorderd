using System;
using System.Runtime.Serialization;

namespace Persistentie {
	public class ReservatieToeVoegenException : Exception {
		public ReservatieToeVoegenException() {
		}

		public ReservatieToeVoegenException(string message) : base(message) {
		}
	}
}