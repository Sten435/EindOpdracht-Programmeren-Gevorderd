using System;
using System.Runtime.Serialization;

namespace Persistentie {
	public class NoDataInDbException : Exception {
		public NoDataInDbException() {
		}

		public NoDataInDbException(string message) : base(message) {
		}
	}
}