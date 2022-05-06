using System;
using System.Runtime.Serialization;

namespace Domein {
	public class ToestelTypeException : Exception {
		public ToestelTypeException() {
		}

		public ToestelTypeException(string message) : base(message) {
		}
	}
}