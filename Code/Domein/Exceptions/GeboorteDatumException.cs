using System;
using System.Runtime.Serialization;

namespace Domein {
	public class GeboorteDatumException : Exception {
		public GeboorteDatumException() {
		}

		public GeboorteDatumException(string message) : base(message) {
		}
	}
}