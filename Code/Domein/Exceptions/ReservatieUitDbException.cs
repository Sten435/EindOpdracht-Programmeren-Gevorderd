using System;
using System.Runtime.Serialization;

namespace Domein {
	public class ReservatieUitDbException : Exception {
		public ReservatieUitDbException() {
		}

		public ReservatieUitDbException(string message) : base(message) {
		}
	}
}