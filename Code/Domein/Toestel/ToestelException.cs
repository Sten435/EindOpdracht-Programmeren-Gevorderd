using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domein {
	public class ToestelException : Exception {
		public ToestelException() {

		}
		public ToestelException(string message) : base(message) {
		}
	}
}
