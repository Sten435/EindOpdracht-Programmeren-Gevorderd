using System;

namespace Domein {

	public class Toestel {
		public long IdentificatieCode { get; }
		public string ToestelType { get; set; }
		public bool InHerstelling { get; set; }

		public Toestel(int identificatieCode, string toestelType, bool inHerstelling) {
			IdentificatieCode = identificatieCode;
			ToestelType = toestelType;
			InHerstelling = inHerstelling;
		}

		public override string ToString() => $"{ToestelType} - InHerstelling: {InHerstelling}";

		public override bool Equals(object obj) {
			return obj is Toestel toestel &&
				   IdentificatieCode == toestel.IdentificatieCode &&
				   ToestelType == toestel.ToestelType &&
				   InHerstelling == toestel.InHerstelling;
		}

		public override int GetHashCode() {
			return HashCode.Combine(IdentificatieCode, ToestelType, InHerstelling);
		}
	}
}