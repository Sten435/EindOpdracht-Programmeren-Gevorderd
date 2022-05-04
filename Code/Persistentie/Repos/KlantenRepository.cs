using Domein;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistentie {
	public class KlantenRepository : IKlantenRepository {
		private KlantenMapper _mapper = new();

		public KlantenRepository() {
			LaadKlanten();
		}

		public List<Klant> GeefAlleKlanten() => _mapper.GeefAlleKlanten();

		public void LaadKlanten() {
			string sCurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
			string sFile = Path.Combine(sCurrentDirectory, @"Data\Klanten.txt");
			string sFilePath = Path.GetFullPath(sFile);

			int klantenNr = 1;
			SqlTransaction sqlTransaction;

			try {
				using TextFieldParser parser = new(sFilePath);
				parser.TextFieldType = FieldType.Delimited;
				parser.SetDelimiters(",");

				Klant klant;

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

					klant = new(klantenNr, fields[0], fields[1], fields[2], new List<string>() { fields[3] }, DateTime.Parse(fields[4]), new Adres("StanStraat", "101", fields[3], 9999), klantType);
					RegistreerKlant(klant);

					//if (string.IsNullOrEmpty(fields[5]))
					//	fields[5] = "Geen";

					//using (SqlConnection connection = new(ConfigRepository.ConnectionString)) {
					//	connection.Open();

					//	SqlCommand commandSQL = new("INSERT INTO Interesses (InteresseNaam, Klant_KlantenNummer) VALUES (@InteresseNaam, @Klant_KlantenNummer);", connection);

					//	commandSQL.Parameters.AddWithValue("@InteresseNaam", fields[5]);
					//	commandSQL.Parameters.AddWithValue("@Klant_KlantenNummer", klantenNr);

					//	commandSQL.ExecuteNonQuery();
					//	klantenNr++;
					//}
				}
			} catch (Exception error) {
				throw new KlantenExeption(error.Message);
				//sqlTransaction.Rollback();
			}
		}

		public void InsertklantAdres() {
			//using (SqlConnection connection = new(ConfigRepository.ConnectionString)) {
			//	connection.Open();

			//	SqlCommand commandSQL = new("INSERT INTO Adressen (StraatNaam, HuisNummer, Plaats, PostCode, Klanten_KlantenNummer) VALUES (@StraatNaam, @HuisNummer, @Plaats, @PostCode, @Klanten_KlantenNummer);", connection);

			//	commandSQL.Parameters.AddWithValue("@StraatNaam", klant.Adres.StraatNaam);
			//	commandSQL.Parameters.AddWithValue("@HuisNummer", klant.Adres.HuisNummer);
			//	commandSQL.Parameters.AddWithValue("@Plaats", klant.Adres.Plaats);
			//	commandSQL.Parameters.AddWithValue("@PostCode", klant.Adres.PostCode);
			//	commandSQL.Parameters.AddWithValue("@Klanten_KlantenNummer", klantenNr);

			//	commandSQL.ExecuteNonQuery();
			//	klantenNr++;
			//}

			//using (SqlConnection connection = new(ConfigRepository.ConnectionString)) {
			//	connection.Open();
			//	sqlTransaction = connection.BeginTransaction();
			//	SqlCommand commandSQL = new("INSERT INTO Adressen (StraatNaam, HuisNummer, Plaats, PostCode, Klant_KlantenNummer) VALUES (@StraatNaam, @HuisNummer, @Plaats, @PostCode, @Klant_KlantenNummer);", connection,  sqlTransaction);

			//	commandSQL.Parameters.AddWithValue("@StraatNaam", klant.Adres.StraatNaam);
			//	commandSQL.Parameters.AddWithValue("@HuisNummer", klant.Adres.HuisNummer);
			//	commandSQL.Parameters.AddWithValue("@Plaats", klant.Adres.Plaats);
			//	commandSQL.Parameters.AddWithValue("@PostCode", klant.Adres.PostCode);
			//	commandSQL.Parameters.AddWithValue("@Klant_KlantenNummer", klantenNr);

			//	commandSQL.ExecuteNonQuery();
			//	klantenNr++;

			//	sqlTransaction.Commit();
			//}
		}

		public Klant Login(string email) {
			email = email.Trim().ToLower();
			if (string.IsNullOrEmpty(email)) throw new EmailExpection("Email mag niet leeg zijn.");
			try {
				var addr = new System.Net.Mail.MailAddress(email);
				if (addr.Address != email) throw new EmailExpection("Email is niet toegelaten.");
			} catch {
				throw new EmailExpection("Email is niet toegelaten.");
			}

			Klant Klant = GeefAlleKlanten().FirstOrDefault(_klant => _klant.Email.ToLower() == email);
			if (Klant == null) throw new LoginException("Email is niet correct.");
			return Klant;
		}

		public void RegistreerKlant(Klant klant) => _mapper.Klanten.Add(klant);

		public void VerwijderKlant(Klant klant) => _mapper.Klanten.Remove(klant);
	}
}
