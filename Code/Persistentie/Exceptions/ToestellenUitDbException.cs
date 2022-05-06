using System;
using System.Runtime.Serialization;

namespace Persistentie {
	public class ToestellenUitDbException : Exception {
		public ToestellenUitDbException() {
		}

		public ToestellenUitDbException(string message) : base(message) {
		}
	}
}