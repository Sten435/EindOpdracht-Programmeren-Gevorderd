using System;
using System.Runtime.Serialization;

namespace Persistentie {
	public class SelectFromDbException : Exception {
		public SelectFromDbException() {
		}

		public SelectFromDbException(string message) : base(message) {
		}
	}
}