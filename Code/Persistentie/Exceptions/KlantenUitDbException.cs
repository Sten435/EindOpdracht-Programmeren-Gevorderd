using System;
using System.Runtime.Serialization;

namespace Persistentie {
	public class KlantenUitDbException : Exception {
		public KlantenUitDbException() {
		}

		public KlantenUitDbException(string message) : base(message) {
		}
	}
}