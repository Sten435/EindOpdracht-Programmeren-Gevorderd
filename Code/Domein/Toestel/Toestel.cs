using System;

namespace Domein {

	public class Toestel {
		public int IdentificatieCode { get; }
		public string ToestelType { get; set; }
		public bool InHerstelling { get; set; }

		//Config
		public static bool? StandaardInherstelling { get; set; } = false;


		private UniekeCode uniekeCode = UniekeCode.Instance;

		public Toestel(string toestelType) {
			ControlleerStandaardherstelling();
			int identificatieCode = uniekeCode.GenereerRandomCode();
			ControlleerIdentificatieCode(identificatieCode);
			ControlleerToestelNaam(toestelType);
			ControlleerToestelNaam(toestelType);
			IdentificatieCode = identificatieCode;
			ToestelType = toestelType;
			InHerstelling = (bool)StandaardInherstelling;
		}

		public override string ToString() => $"{ToestelType} - InHerstelling: {InHerstelling}";

		private void ControlleerToestelNaam(string toestelType) {
			string toestelNaam = toestelType.Trim();
			if (string.IsNullOrEmpty(toestelNaam)) throw new ToestelTypeException("Toesteltype mag niet leeg zijn.");
			if (toestelType.Length <= 1) throw new ToestelTypeException("Toesteltype moet langer zijn dan 1 letter.");
		}

		private void ControlleerStandaardherstelling() {
			if (StandaardInherstelling == null) throw new ToestelException("Het StandaardInherstelling is nog niet correct ingesteld.");
		}

		private void ControlleerIdentificatieCode(int identificatieCode) {
			if (identificatieCode < 0) throw new IdentificatieCodeException("IdentificatieCode moet groter dan 0 zijn.");
		}

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