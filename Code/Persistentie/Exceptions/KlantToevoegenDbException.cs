using System;
using System.Runtime.Serialization;

namespace Persistentie {
	public class KlantToevoegenDbException : Exception {
		public KlantToevoegenDbException() {
		}

		public KlantToevoegenDbException(string message) : base(message) {
		}
	}
}