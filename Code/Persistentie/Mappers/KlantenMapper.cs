using Domein;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Persistentie {

	public class KlantenMapper {
		public List<Klant> Klanten = new();

		//public List<Klant> Klanten = new() {
		//	new Klant(1, "Stan", "Persoons", "stan.persoons@student.hogent.be",
		//		new List<string>() {
		//			"Programmeren",
		//			"Javascript",
		//			"Web Development"
		//		}, new DateTime(2002, 04, 08), new Adres("StanStraat", "75", "Oosterzele", 9860), TypeKlant.Beheerder)
		//};

		public List<Klant> GeefAlleKlanten() {
			List<Klant> klanten = new();
			return klanten;

			//try {
			//	using (SqlConnection connection = new(ConfigRepository.ConnectionString)) {
			//		connection.Open();

			//		SqlCommand command = new("SELECT * FROM Klanten;", connection);

			//		using (SqlDataReader dataReader = command.ExecuteReader()) {
			//			Console.WriteLine();
			//			if (dataReader.HasRows) {
			//				while (dataReader.Read()) {
			//					int KlantenNummer = (int)dataReader["KlantenNummer"];
			//					string Voornaam = (string)dataReader["Voornaam"];
			//					string Achternaam = (string)dataReader["Achternaam"];
			//					string Email = (string)dataReader["Email"];
			//					DateTime GeboorteDatum = (DateTime)dataReader["GeboorteDatum"];

			//					List<string> interesses = new();

			//					Adres adres = new();

			//					TypeKlant klantType = klantTypeRaw switch {
			//						"gold" => TypeKlant.Gold,
			//						"bronze" => TypeKlant.Bronze,
			//						"silver" => TypeKlant.Silver,
			//						"beheerder" => TypeKlant.Beheerder,
			//						_ => TypeKlant.Bronze,
			//					};

			//					Klant klant = new(KlantenNummer, Voornaam, Achternaam, interesses, GeboorteDatum, Adres, TypeKlant);
			//					autos.Add(auto);
			//				}
			//			} else throw new Exception("Geen data in de databank...");
			//		}
			//	}
			//} catch (Exception err) {
			//	Console.WriteLine(err.Message);
			//}
		}
	}
}