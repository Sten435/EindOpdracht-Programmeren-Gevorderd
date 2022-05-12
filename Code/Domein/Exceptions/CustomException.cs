using System;

namespace Domein {

	public class CustomExceptions : Exception {

		public CustomExceptions(string message) : base(message) {
		}
	}
}