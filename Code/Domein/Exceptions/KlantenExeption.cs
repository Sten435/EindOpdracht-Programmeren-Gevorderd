using System;
using System.Runtime.Serialization;

namespace Domein {
	internal class KlantenExeption : Exception {
		public KlantenExeption() {
		}

		public KlantenExeption(string message) : base(message) {
		}
	}
}