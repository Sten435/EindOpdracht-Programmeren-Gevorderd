using System;
using System.Linq;

namespace Domein {

	public class Toestel {
		public int IdentificatieCode { get; }
		public string ToestelType { get; set; }
		public bool InHerstelling { get; set; }
		public bool Verwijderd { get; set; }

		//Config
		public static bool? StandaardInherstelling { get; set; } = false;

		public Toestel(int identificatieCode, string toestelType, bool? inHerstelling = null, bool? verwijderd = null) {
			ControlleerStandaardherstelling();
			ControlleerIdentificatieCode(identificatieCode);
			ControlleerToestelNaam(toestelType);
			IdentificatieCode = identificatieCode;
			ToestelType = toestelType;
			if (inHerstelling == null) {
				InHerstelling = (bool)StandaardInherstelling;
			} else InHerstelling = (bool)inHerstelling;
			if (verwijderd == null) {
				Verwijderd = false;
			} else Verwijderd = (bool)verwijderd;
		}

		public string ToString(bool parsed = false) {
			if (parsed) {
				return $"{IdentificatieCode}@|@{ToestelType}@|@{InHerstelling}@|@{Verwijderd}";
			}
			return $"{ToestelType} - InHerstelling: {InHerstelling}";
		}

		public static void ControlleerToestelNaam(string toestelType) {
			string toestelNaam = toestelType.Trim();
			if (string.IsNullOrEmpty(toestelNaam) || toestelNaam.ToList().Any(l => !char.IsLetter(l))) throw new ToestelException("Toesteltype mag niet leeg zijn.");
			if (toestelType.Length <= 1) throw new ToestelException("Toesteltype moet langer zijn dan 1 letter.");
		}

		private void ControlleerStandaardherstelling() {
			if (StandaardInherstelling == null) throw new ToestelException("Het StandaardInherstelling is nog niet correct ingesteld.");
		}

		private void ControlleerIdentificatieCode(int identificatieCode) {
			if (identificatieCode < 0) throw new ToestelException("IdentificatieCode moet groter dan 0 zijn.");
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