using System;
using System.Runtime.Serialization;

namespace Domein {
	public class ReservatieException : CustomExceptions {
	

		public ReservatieException(string message) : base(message) {
		}
	}
}