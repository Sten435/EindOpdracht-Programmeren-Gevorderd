using System;
using System.Runtime.Serialization;

namespace Domein {
	public class KlantenExeption : CustomExceptions {
	

		public KlantenExeption(string message) : base(message) {
		}
	}
}