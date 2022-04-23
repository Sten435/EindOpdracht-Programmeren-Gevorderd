using Domein;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Persistentie {

	public class KlantenMapper : IKlantenRepository {

		private List<Klant> _klanten = new() {
			new Klant(1, "Stan", "Persoons", "stan.persoons@student.hogent.be",
				new List<string>() {
					"Programmeren",
					"Javascript",
					"Web Development"
				}, new DateTime(2002, 04, 08), new Adres("StanStraat", "75", "Oosterzele", 9860), TypeKlant.Beheerder)
		};

		public bool LaadKlanten() {
			string sCurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
			string sFile = Path.Combine(sCurrentDirectory, @"Data\Klanten.txt");
			string sFilePath = Path.GetFullPath(sFile);

			try {
				using TextFieldParser parser = new(sFilePath);
				parser.TextFieldType = FieldType.Delimited;
				parser.SetDelimiters(",");

				Klant klant;
				int klantenNr = (int)parser.LineNumber + 3;

				while (!parser.EndOfData) {
					List<string> fields = parser.ReadFields().ToList();
					fields = fields.Select(field => field.Replace("\'", "")).ToList();

					string klantTypeRaw = fields[6].ToLower();
					TypeKlant klantType = klantTypeRaw switch {
						"gold" => TypeKlant.Gold,
						"bronze" => TypeKlant.Bronze,
						"silver" => TypeKlant.Silver,
						"beheerder" => TypeKlant.Beheerder,
						_ => TypeKlant.Bronze,
					};

					klant = new(klantenNr++, fields[0], fields[1], fields[2], new List<string>() { fields[3] }, DateTime.Parse(fields[4]), new Adres("StanStraat", "101", fields[3], 0000), klantType);
					RegistreerKlant(klant);
				}
			} catch (Exception err) {
				return false;
			}

			return true;
		}

		public List<Klant> GeefAlleKlanten() => _klanten;

		public Klant Login(string email) {
			Klant Klant = GeefAlleKlanten().FirstOrDefault(_klant => _klant.Email.ToLower() == email);
			if (Klant == null) throw new LoginException("Email is niet correct.");
			return Klant;
		}

		public void RegistreerKlant(Klant klant) => _klanten.Add(klant);

		public void VerwijderKlant(Klant klant) => _klanten.Remove(klant);
	}
}