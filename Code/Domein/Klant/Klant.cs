using System;
using System.Collections.Generic;

namespace Domein {

	public class Klant {
		public long KlantenNummer { get; }
		public string Voornaam { get; }
		public string Achternaam { get; }
		public string Email { get; }
		public List<string> Interesses { get; }
		public DateTime GeboorteDatum { get; }
		public Adres Adres { get; }
		public TypeKlant TypeKlant { get; }

		public Klant(long klantenNummer, string voornaam, string achternaam, string email, List<string> interesses, DateTime geboorteDatum, Adres adres, TypeKlant typeKlant) {
			CheckDatumGeboorteDatum(geboorteDatum);
			GeboorteDatum = geboorteDatum;
			KlantenNummer = klantenNummer;
			Voornaam = voornaam;
			Achternaam = achternaam;
			Email = email;
			Interesses = interesses;
			Adres = adres;
			TypeKlant = typeKlant;
		}

		public Klant() {
		}

		private void CheckDatumGeboorteDatum(DateTime datum) {
			if (datum > DateTime.Now) throw new Exception("Geboortedatum mag niet in de toekomst zijn.");
		}

		public override string ToString() {
			return $"{KlantenNummer} | {Voornaam} {Achternaam} | {Email} | {GeboorteDatum.ToString("d")} | {TypeKlant} | {Adres.StraatNaam} {Adres.HuisNummer} {Adres.PostCode}";
		}
	}
}