using System;
using System.Runtime.Serialization;

namespace Domein {
	[Serializable]
	public class ToestelTypeException : Exception {
		public ToestelTypeException() {
		}

		public ToestelTypeException(string message) : base(message) {
		}

		public ToestelTypeException(string message, Exception innerException) : base(message, innerException) {
		}

		protected ToestelTypeException(SerializationInfo info, StreamingContext context) : base(info, context) {
		}
	}
}