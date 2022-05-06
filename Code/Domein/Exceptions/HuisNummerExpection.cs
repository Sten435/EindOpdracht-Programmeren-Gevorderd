using System;
using System.Runtime.Serialization;

namespace Domein {
	public class HuisNummerExpection : Exception {
		public HuisNummerExpection() {
		}

		public HuisNummerExpection(string message) : base(message) {
		}
	}
}