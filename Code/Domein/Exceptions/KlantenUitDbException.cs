using System;
using System.Runtime.Serialization;

namespace Domein {
	public class KlantenUitDbException : Exception {
		public KlantenUitDbException() {
		}

		public KlantenUitDbException(string message) : base(message) {
		}
	}
}