using System;
using System.Runtime.Serialization;

namespace Persistentie {
	public class ReservatieUitDbException : Exception {
		public ReservatieUitDbException() {
		}

		public ReservatieUitDbException(string message) : base(message) {
		}
	}
}