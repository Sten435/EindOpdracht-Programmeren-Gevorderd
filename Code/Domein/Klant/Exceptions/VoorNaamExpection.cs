using System;
using System.Runtime.Serialization;

namespace Domein {
	public class VoorNaamException : Exception {
		public VoorNaamException() {
		}

		public VoorNaamException(string message) : base(message) {
		}
	}
}