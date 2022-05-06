using System;
using System.Runtime.Serialization;

namespace Persistentie {
	public class ReservatieVerwijderenException : Exception {
		public ReservatieVerwijderenException() {
		}

		public ReservatieVerwijderenException(string message) : base(message) {
		}
	}
}