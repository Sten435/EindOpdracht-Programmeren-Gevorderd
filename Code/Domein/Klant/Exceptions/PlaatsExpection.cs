using System;
using System.Runtime.Serialization;

namespace Domein {
	public class PlaatsExpection : Exception {
		public PlaatsExpection() {
		}

		public PlaatsExpection(string message) : base(message) {
		}
	}
}