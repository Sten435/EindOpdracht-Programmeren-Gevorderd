using System;
using System.Runtime.Serialization;

namespace Persistentie {
	internal class KlantenExeption : Exception {
		public KlantenExeption() {
		}

		public KlantenExeption(string message) : base(message) {
		}
	}
}