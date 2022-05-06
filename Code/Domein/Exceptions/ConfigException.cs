using System;
using System.Runtime.Serialization;

namespace Domein {
	public class ConfigException : Exception {
		public ConfigException() {
		}

		public ConfigException(string message) : base(message) {
		}
	}
}