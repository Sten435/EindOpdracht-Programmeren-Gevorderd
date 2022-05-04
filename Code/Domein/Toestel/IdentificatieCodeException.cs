using System;
using System.Runtime.Serialization;

namespace Domein {
	public class IdentificatieCodeException : Exception {
		public IdentificatieCodeException() {
		}

		public IdentificatieCodeException(string message) : base(message) {
		}
	}
}