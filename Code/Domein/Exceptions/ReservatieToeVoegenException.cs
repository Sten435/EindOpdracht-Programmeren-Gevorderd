using System;
using System.Runtime.Serialization;

namespace Domein {
	public class ReservatieToeVoegenException : Exception {
		public ReservatieToeVoegenException() {
		}

		public ReservatieToeVoegenException(string message) : base(message) {
		}
	}
}