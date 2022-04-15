using System;

namespace Domein {
	public class LoginException : Exception {
		public LoginException() {
		}

		public LoginException(string message) : base(message) {
		}
	}
}
