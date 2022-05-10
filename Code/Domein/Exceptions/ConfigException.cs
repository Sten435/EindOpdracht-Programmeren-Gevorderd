using System;
using System.Runtime.Serialization;

namespace Domein {
	public class ConfigException : CustomExceptions {
		public ConfigException(string message) : base(message) {
		}
	}
}