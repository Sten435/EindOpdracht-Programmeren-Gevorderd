using System;
using System.Runtime.Serialization;

namespace Domein {
	public class SelectConfigFromDbException : Exception {
		public SelectConfigFromDbException() {
		}

		public SelectConfigFromDbException(string message) : base(message) {
		}
	}
}