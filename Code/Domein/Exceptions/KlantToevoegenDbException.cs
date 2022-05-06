using System;
using System.Runtime.Serialization;

namespace Domein {
	public class KlantToevoegenDbException : Exception {
		public KlantToevoegenDbException() {
		}

		public KlantToevoegenDbException(string message) : base(message) {
		}
	}
}