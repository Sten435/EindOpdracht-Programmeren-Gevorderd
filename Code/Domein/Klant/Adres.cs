using System.Text.RegularExpressions;

namespace Domein {

	public class Adres {
		public string StraatNaam { get; set; }
		public string HuisNummer { get; set; }
		public string Plaats { get; set; }
		public int PostCode { get; set; }

		public Adres(string straatNaam, string huisNummer, string plaats, int postCode) {
			ControleStraatNaam(straatNaam);
			ControleHuisNummer(huisNummer);
			ControlePlaats(plaats);
			ControlePostCode(postCode);

			StraatNaam = straatNaam;
			HuisNummer = huisNummer;
			Plaats = plaats;
			PostCode = postCode;
		}

		public Adres() {
		}

		private void ControleStraatNaam(string straatNaam) {
			straatNaam = straatNaam.Trim();
			if (string.IsNullOrEmpty(straatNaam)) throw new KlantenNummerException("Straat naam mag niet leeg zijn.");
			if (straatNaam.Length < 3) throw new KlantenNummerException("Straat naam moet meer dan 3 letters bevatten.");
			foreach (char letter in straatNaam.ToCharArray()) {
				if (char.IsNumber(letter)) throw new KlantenNummerException("Straat naam mag geen enkel cijfer bevatten.");
			}
		}

		private void ControleHuisNummer(string huisNummer) {
			huisNummer = huisNummer.Trim();
			if (string.IsNullOrEmpty(huisNummer)) throw new HuisNummerExpection("Huisnummer mag niet leeg zijn.");
			if (huisNummer.Length > 6) throw new HuisNummerExpection("Ik denk niet dat er een huisnummer van meer dan 6 characters bestaat. ;-)");
		}

		private void ControlePlaats(string plaats) {
			plaats = plaats.Trim();
			if (string.IsNullOrEmpty(plaats)) throw new PlaatsExpection("Plaats mag niet leeg zijn.");
			if (plaats.Length <= 2) throw new PlaatsExpection("Plaats moet meer dan 2 letters bevatten.");
			foreach (char letter in plaats.ToCharArray()) {
				if (char.IsNumber(letter)) throw new PlaatsExpection("Plaats mag geen enkel cijfer bevatten.");
			}
		}

		private void ControlePostCode(int postCode) {
			if (string.IsNullOrEmpty(postCode.ToString().Trim())) throw new PostCodeExpection("PostCode mag niet leeg zijn.");
			if (postCode.ToString().Length != 4) throw new PostCodeExpection("PostCode moet 4 cijfers bevatten.");

			Regex rgx = new Regex(@"^[1-9]{1}[0-9]{3}$", RegexOptions.IgnoreCase);
			MatchCollection matches = rgx.Matches(postCode.ToString());
			if (matches.Count == 0) throw new PostCodeExpection("PostCode is fout. Let op er mogen enkel cijfers in zitten en moet tussen 1000-9999 zijn.");
		}
	}
}