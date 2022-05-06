using Domein;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Persistentie {

	public static class ReservatieMapper {
		public static List<Reservatie> GeefAlleReservaties() {
			List<Reservatie> reservaties = new();

			try {
				using SqlConnection connection = new(ConfigRepository.ConnectionString);
				connection.Open();

				SqlCommand command = new("SELECT * FROM Reservaties r join TijdSloten ts on ts.Reservatie_ReservatieNummer = ReservatieNummer;", connection);

				Reservatie reservatie;

				using SqlDataReader reader = command.ExecuteReader();
				if (reader.HasRows) {
					while (reader.Read()) {
						int reservatieNummer = (int)reader["ReservatieNummer"];
						int klantenNummer = (int)reader["Klant_KlantenNummer"];
						int toestelNummer = (int)reader["Toestel_IdentificatieCode"];
						DateTime startTijd = (DateTime)reader["StartTijd"];
						DateTime eindTijd = (DateTime)reader["EindTijd"];

						Klant klant = KlantenMapper.GeefAlleKlanten().Find(klant => klant.KlantenNummer == klantenNummer);
						Toestel toestel = ToestellenMapper.GeefToestellen().Find(toestel => toestel.IdentificatieCode == toestelNummer);
						TijdsSlot tijdsSlot = new(startTijd, eindTijd);

						reservatie = new(reservatieNummer, klant, tijdsSlot, toestel);
						reservaties.Add(reservatie);
					}
				}

			} catch (Exception error) {
				throw new ReservatieUitDbException(error.Message);
			}
			return reservaties;
		}

		public static void VerwijderReservatie(Reservatie reservatie) {
			try {
				using SqlConnection connection = new(ConfigRepository.ConnectionString);
				connection.Open();

				SqlCommand command = new("DELETE FROM Reservaties WHERE ReservatieNummer = @ReservatieNummer", connection);

				command.Parameters.AddWithValue("@ReservatieNummer", reservatie.ReservatieNummer);

				command.ExecuteNonQuery();
			} catch (Exception error) {
				throw new ReservatieVerwijderenException(error.Message);
			}
		}

		public static void VoegReservatieToe(Reservatie reservatie) {
			using SqlConnection connection = new(ConfigRepository.ConnectionString);
			connection.Open();

			SqlTransaction sqlTransaction = connection.BeginTransaction();
			try {
				SqlCommand command = new("INSERT INTO Reservaties (Klant_KlantenNummer, Toestel_IdentificatieCode) VALUES (@Klant_KlantenNummer, @Toestel_IdentificatieCode)SELECT SCOPE_IDENTITY()", connection, sqlTransaction);

				command.Parameters.AddWithValue("@Klant_KlantenNummer", reservatie.Klant.KlantenNummer);
				command.Parameters.AddWithValue("@Toestel_IdentificatieCode", reservatie.Toestel.IdentificatieCode);

				int reservatieNummer = (int)(decimal)command.ExecuteScalar();
				reservatie.ReservatieNummer = reservatieNummer;

				command = new("INSERT INTO TijdSloten (StartTijd, EindTijd, Reservatie_ReservatieNummer) VALUES (@StartTijd, @EindTijd, @Reservatie_ReservatieNummer);", connection, sqlTransaction);

				command.Parameters.AddWithValue("@StartTijd", reservatie.TijdsSlot.StartTijd);
				command.Parameters.AddWithValue("@EindTijd", reservatie.TijdsSlot.EindTijd);
				command.Parameters.AddWithValue("@Reservatie_ReservatieNummer", reservatieNummer);

				command.ExecuteNonQuery();

				sqlTransaction.Commit();
			} catch (Exception error) {
				sqlTransaction.Rollback();
				throw new ReservatieToeVoegenException(error.Message);
			}
		}
	}
}