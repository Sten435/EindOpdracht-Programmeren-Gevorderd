using System;
using System.Runtime.Serialization;

namespace Domein {
	public class ToestellenUitDbException : Exception {
		public ToestellenUitDbException() {
		}

		public ToestellenUitDbException(string message) : base(message) {
		}
	}
}