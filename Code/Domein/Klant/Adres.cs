namespace Domein {

	public class Adres {
		public string StraatNaam { get; set; }
		public string HuisNummer { get; set; }
		public string Plaats { get; set; }
		public int PostCode { get; set; }

		public Adres(string straatNaam, string huisNummer, string plaats, int postCode) {
			StraatNaam = straatNaam;
			HuisNummer = huisNummer;
			Plaats = plaats;
			PostCode = postCode;
		}

		public Adres() {
		}
	}
}