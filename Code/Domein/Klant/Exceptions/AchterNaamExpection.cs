using System;
using System.Runtime.Serialization;

namespace Domein {
	public class AchterNaamExpection : Exception {
		public AchterNaamExpection() {
		}

		public AchterNaamExpection(string message) : base(message) {
		}
	}
}