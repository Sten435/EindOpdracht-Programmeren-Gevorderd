using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domein {
	public class UniekeCode {
		Random rnd = new Random();

		List<int> VoorbijeCodes = new List<int>();

		private static UniekeCode instance = null;
		private static readonly object padlock = new object();

		UniekeCode() {
		}

		public static UniekeCode Instance {
			get {
				lock (padlock) {
					if (instance == null) {
						instance = new UniekeCode();
					}
					return instance;
				}
			}
		}

		public int GenereerRandomCode() {
			int code;
			do {
				code = rnd.Next(0, 100000);
			} while (VoorbijeCodes.Contains(code));
			VoorbijeCodes.Add(code);
			return code;
		}
	}
}
