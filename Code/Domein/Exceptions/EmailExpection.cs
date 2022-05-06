using System;
using System.Runtime.Serialization;

namespace Domein {
	public class EmailExpection : Exception {
		public EmailExpection() {
		}

		public EmailExpection(string message) : base(message) {
		}
	}
}