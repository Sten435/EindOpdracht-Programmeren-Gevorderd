using System;
using System.Runtime.Serialization;

namespace Domein {
	public class KlantenNummerException : Exception {
		public KlantenNummerException() {
		}

		public KlantenNummerException(string message) : base(message) {
		}
	}
}