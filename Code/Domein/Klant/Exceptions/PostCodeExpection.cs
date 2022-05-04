using System;
using System.Runtime.Serialization;

namespace Domein {
	public class PostCodeExpection : Exception {
		public PostCodeExpection() {
		}

		public PostCodeExpection(string message) : base(message) {
		}
	}
}