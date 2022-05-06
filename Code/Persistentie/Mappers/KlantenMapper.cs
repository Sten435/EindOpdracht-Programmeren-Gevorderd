using Domein;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Persistentie {

	public static class KlantenMapper {

		public static List<Klant> GeefAlleKlanten() {
			List<Klant> klanten = new();

			try {
				using SqlConnection connection = new(ConfigRepository.ConnectionString);
				connection.Open();

				SqlCommand command = new("SELECT KlantenNummer, Voornaam, Achternaam, Email, GeboorteDatum, Abonnement, InteresseNaam, StraatNaam, HuisNummer, Plaats, PostCode from Klanten kl join Interesses it on it.Klant_KlantenNummer = kl.KlantenNummer join Adressen ad on ad.Klant_KlantenNummer = kl.KlantenNummer;", connection);

				using SqlDataReader dataReader = command.ExecuteReader();

				if (dataReader.HasRows) {
					while (dataReader.Read()) {
						int klantenNummer = (int)dataReader["KlantenNummer"];
						string voornaam = (string)dataReader["Voornaam"];
						string achternaam = (string)dataReader["Achternaam"];
						string email = (string)dataReader["Email"];

						DateTime geboorteDatum = (DateTime)dataReader["GeboorteDatum"];
						int abonnement = int.Parse((string)dataReader["Abonnement"]);
						List<string> interesses = new() { (string)dataReader["InteresseNaam"] };

						string straatNaam = (string)dataReader["StraatNaam"];
						string huisNummer = (string)dataReader["HuisNummer"];
						string plaats = (string)dataReader["Plaats"];

						int postCode = (int)dataReader["PostCode"];

						Adres adres = new(straatNaam, huisNummer, plaats, postCode);
						TypeKlant klantType = (TypeKlant)Enum.Parse(typeof(TypeKlant), abonnement.ToString());
						Klant klant = new(klantenNummer, voornaam, achternaam, email, interesses, geboorteDatum, adres, klantType);

						if (!klanten.Any(klanten => klanten.KlantenNummer == klant.KlantenNummer)) klanten.Add(klant);
						else {
							Klant nieuweInteresseKlant = klanten.Find(klanten => klanten.KlantenNummer == klant.KlantenNummer);
							klanten.Remove(nieuweInteresseKlant);
							nieuweInteresseKlant.Interesses.Add((string)dataReader["InteresseNaam"]);
							klanten.Add(nieuweInteresseKlant);
						}
					}
				} else throw new KlantenUitDbException("Geen Klanten gevonden.");
			} catch (Exception error) {
				throw new KlantenUitDbException(error.Message);
			}

			return klanten;
		}

		public static void VoegKlantToe(Klant klant) {
			using SqlConnection connection = new(ConfigRepository.ConnectionString);
			connection.Open();

			SqlTransaction sqlTransaction = connection.BeginTransaction();

			try {
				SqlCommand commandSQL = new("INSERT INTO Klanten (Voornaam, Achternaam, Email, GeboorteDatum, ABONNEMENT) VALUES (@Voornaam, @Achternaam, @Email, @GeboorteDatum, @ABONNEMENT)SELECT SCOPE_IDENTITY()", connection, sqlTransaction);

				commandSQL.Parameters.AddWithValue("@Voornaam", klant.Voornaam);
				commandSQL.Parameters.AddWithValue("@Achternaam", klant.Achternaam);
				commandSQL.Parameters.AddWithValue("@Email", klant.Email);
				commandSQL.Parameters.AddWithValue("@GeboorteDatum", klant.GeboorteDatum);
				commandSQL.Parameters.AddWithValue("@ABONNEMENT", (int)klant.TypeKlant);

				int klantenId = Convert.ToInt32(commandSQL.ExecuteScalar());

				Adres adres = klant.Adres;

				commandSQL = new("INSERT INTO Adressen (StraatNaam, HuisNummer, Plaats, PostCode, Klant_KlantenNummer) VALUES (@StraatNaam, @HuisNummer, @Plaats, @PostCode, @Klant_KlantenNummer);", connection, sqlTransaction);

				commandSQL.Parameters.AddWithValue("@StraatNaam", klant.Adres.StraatNaam);
				commandSQL.Parameters.AddWithValue("@HuisNummer", klant.Adres.HuisNummer);
				commandSQL.Parameters.AddWithValue("@Plaats", klant.Adres.Plaats);
				commandSQL.Parameters.AddWithValue("@PostCode", klant.Adres.PostCode);
				commandSQL.Parameters.AddWithValue("@Klant_KlantenNummer", klantenId);

				commandSQL.ExecuteNonQuery();

				klant.Interesses.ForEach(i => {
					commandSQL = new("INSERT INTO Interesses (InteresseNaam, Klant_KlantenNummer) VALUES (@InteresseNaam, @Klant_KlantenNummer);", connection, sqlTransaction);

					commandSQL.Parameters.AddWithValue("@InteresseNaam", i);
					commandSQL.Parameters.AddWithValue("@Klant_KlantenNummer", klantenId);

					commandSQL.ExecuteNonQuery();
				});

				sqlTransaction.Commit();
			} catch (Exception err) {
				sqlTransaction.Rollback();
				throw new KlantToevoegenDbException(err.Message);
			}
		}
	}
}