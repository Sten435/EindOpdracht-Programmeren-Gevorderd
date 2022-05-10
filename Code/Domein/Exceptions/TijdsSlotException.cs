using System;
using System.Runtime.Serialization;

namespace Domein {
	public class TijdsSlotException : CustomExceptions {

		public TijdsSlotException(string message) : base(message) {
		}
	}
}