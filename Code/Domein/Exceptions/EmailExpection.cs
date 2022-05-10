using System;
using System.Runtime.Serialization;

namespace Domein {
	public class EmailExpection : CustomExceptions {

		public EmailExpection(string message) : base(message) {
		}
	}
}