using System;

namespace Domein.Exceptions {

	public class ReservatieException : Exception {

		public ReservatieException() {
		}

		public ReservatieException(string message) : base(message) {
		}
	}
}