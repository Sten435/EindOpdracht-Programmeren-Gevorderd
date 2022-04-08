using System;

namespace Domein.Exceptions {

	public class LeeftijdException : Exception {

		public LeeftijdException() {
		}

		public LeeftijdException(string message) : base(message) {
		}
	}
}