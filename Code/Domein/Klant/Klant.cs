using System;
using System.Collections.Generic;

namespace Domein {

	public class Klant {
		public int? KlantenNummer { get; }
		public string Voornaam { get; }
		public string Achternaam { get; }
		public string Email { get; }
		public List<string> Interesses { get; }
		public DateTime GeboorteDatum { get; }
		public Adres Adres { get; }
		public TypeKlant TypeKlant { get; }

		public Klant(int? klantenNummer, string voornaam, string achternaam, string email, List<string> interesses, DateTime geboorteDatum, Adres adres, TypeKlant typeKlant) {
			CheckKlantenNummer(klantenNummer);
			CheckDatumGeboorte(geboorteDatum);
			CheckVoorNaam(voornaam);
			CheckAchterNaam(achternaam);
			CheckEmail(email);
			Interesses = CheckInteresses(interesses);

			GeboorteDatum = geboorteDatum;
			KlantenNummer = klantenNummer;
			Voornaam = voornaam;
			Achternaam = achternaam;
			Email = email;
			Adres = adres;
			TypeKlant = typeKlant;
		}

		public Klant() {
		}

		public override string ToString() {
			return $"{KlantenNummer} | {Voornaam} {Achternaam} | {Email} | {GeboorteDatum.ToString("d")} | {string.Join(" - ", Interesses)} | {TypeKlant} | {Adres.StraatNaam} {Adres.HuisNummer} {Adres.PostCode}";
		}

		private void CheckKlantenNummer(int? klantenNummer) {
			if (klantenNummer != null) {
				string _klantenNummer = klantenNummer.ToString().Trim();
				if (string.IsNullOrWhiteSpace(_klantenNummer)) throw new KlantenExeption("KlantenNummer niet leeg zijn.");
				if (klantenNummer <= 0) throw new KlantenExeption("Klanten nummer moet groter dan 0 zijn.");
			}
		}

		private void CheckDatumGeboorte(DateTime datum) {
			if (datum > DateTime.Now) throw new KlantenExeption("Geboortedatum mag niet in de toekomst zijn.");
		}

		private void CheckVoorNaam(string voorNaam) {
			voorNaam = voorNaam.Trim();
			if (string.IsNullOrEmpty(voorNaam)) throw new KlantenExeption("Voornaam mag niet leeg zijn.");
			if (voorNaam.Length <= 2) throw new KlantenExeption("Voornaam moet meer dan 2 letters bevatten.");
			foreach (char letter in voorNaam.ToCharArray()) {
				if (char.IsNumber(letter)) throw new KlantenExeption("Voornaam mag geen enkel cijfer bevatten.");
			}
		}

		private void CheckAchterNaam(string achterNaam) {
			achterNaam = achterNaam.Trim();
			if (string.IsNullOrEmpty(achterNaam)) throw new KlantenExeption("Achternaam mag niet leeg zijn.");
			if (achterNaam.Length <= 1) throw new KlantenExeption("Achternaam moet meer dan 1 letter bevatten.");
			foreach (char letter in achterNaam.ToCharArray()) {
				if (char.IsNumber(letter)) throw new KlantenExeption("Achternaam mag geen enkel cijfer bevatten.");
			}
		}

		public static void CheckEmail(string email) {
			email = email.Trim();
			if (string.IsNullOrEmpty(email)) throw new EmailExpection("Email mag niet leeg zijn.");
			try {
				var addr = new System.Net.Mail.MailAddress(email);
				if (addr.Address != email) throw new EmailExpection("Email is niet toegelaten.");
			} catch {
				throw new EmailExpection("Email is niet toegelaten.");
			}
		}

		private List<string> CheckInteresses(List<string> interesse) {
			List<string> insteresses = new();
			interesse.ForEach(i => {
				if (!string.IsNullOrEmpty(i) && i.Length > 1) {
					string interesse = i[0] + i.Substring(1);
					insteresses.Add(interesse);
				}
			});
			return insteresses;
		}
	}
}