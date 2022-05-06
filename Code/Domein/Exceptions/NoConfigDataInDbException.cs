using System;
using System.Runtime.Serialization;

namespace Domein {
	public class NoConfigDataInDbException : Exception {
		public NoConfigDataInDbException() {
		}

		public NoConfigDataInDbException(string message) : base(message) {
		}
	}
}