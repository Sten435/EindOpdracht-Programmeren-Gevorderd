using System;
using System.Runtime.Serialization;

namespace Domein {
	public class TijdsSlotException : Exception {
		public TijdsSlotException() {
		}

		public TijdsSlotException(string message) : base(message) {
		}
	}
}