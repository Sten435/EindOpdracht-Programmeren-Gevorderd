namespace Domein {

	public class Toestel {
		public int IdentificatieCode { get; }
		public string ToestelType { get; }
		public bool InHerstelling { get; set; }

		public Toestel(int identificatieCode, string toestelType, bool inHerstelling) {
			IdentificatieCode = identificatieCode;
			ToestelType = toestelType;
			InHerstelling = inHerstelling;
		}

		public override string ToString() {
			return $"{ToestelType} - InHerstelling: {InHerstelling}";
		}
	}
}