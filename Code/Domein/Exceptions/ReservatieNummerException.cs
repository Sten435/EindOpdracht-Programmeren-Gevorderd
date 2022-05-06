using System;
using System.Runtime.Serialization;

namespace Domein {
	public class ReservatieNummerException : Exception {
		public ReservatieNummerException() {
		}

		public ReservatieNummerException(string message) : base(message) {
		}
	}
}